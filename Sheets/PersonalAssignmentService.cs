using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GameConcepts.BurstingBoilAreas;
using GameConcepts.Gateways;
using GameConcepts.Interrupts;
using GameConcepts.Orbs;
using GameConcepts.PersonalAssignments;
using GameConcepts.PhaseThreeAreas;
using GameConcepts.Players;
using Google.Apis.Sheets.v4.Data;

namespace Sheets
{
    public class PersonalAssignmentService
    {
        private const int AssignmentTemplateId = 695235050;
        private const string PersonalAssignmentPrefix = "PA - ";

        public static async Task WritePersonalAssignments(List<PersonalAssignment> personalAssignments)
        {
            var assignments = personalAssignments.OrderByDescending(pa => pa.Player.Name);

            await DeleteAllAssignmentSpreadsheets();

            var names = assignments.Select(p => p.Player.Name).ToList();
            var sheets = await CopyAssignmentSpreadsheets(names);

            var requests = assignments.SelectMany(pa => BuildPersonalAssignment(sheets.First(s => s.Key == pa.Player.Name).Value, pa, personalAssignments));

            await ExecuteBatchRequest(requests);
        }

        private static async Task DeleteAllAssignmentSpreadsheets()
        {
            var allSheets = await SpreadsheetService.GetAllSheets();
            var allAssignmentSheets = allSheets.Where(s => s.Title.StartsWith(PersonalAssignmentPrefix));

            var requests = new List<Request>();

            foreach (var sheet in allAssignmentSheets)
            {
                var deleteRequest = new Request
                {
                    DeleteSheet = new DeleteSheetRequest
                    {
                        SheetId = sheet.SheetId
                    }
                };

                requests.Add(deleteRequest);
            }

            if (requests.Count > 0)
            {
                await ExecuteBatchRequest(requests);
            }
        }

        public static List<Request> BuildPersonalAssignment(int sheetId, PersonalAssignment assignment, List<PersonalAssignment> allAssignments)
        {
            var requests = new List<Request>();
            var sheetName = $"{PersonalAssignmentPrefix}{assignment.Player.Name}";

            requests.Add(SetNameRequest(sheetId, assignment));
            requests.AddRange(BuildSetOrbRequests(sheetId, assignment.Orb));
            requests.AddRange(BuildWarlockStuff(sheetId, assignment));
            requests.AddRange(BuildMonkStuff(sheetId, assignment));
            requests.AddRange(BuildInterruptRequests(sheetId, assignment.Interrupts));
            requests.AddRange(SetTendrilAssignments(sheetId, assignment.Tendril));
            requests.Add(SetBurstingBoilAreaRequest(sheetId, assignment.BurstingBoil));
            requests.Add(SetP3AreaRequest(sheetId, assignment.PhaseThree));

            return requests;
        }

        private static Request SetNameRequest(int sheetId, PersonalAssignment assignment)
            => SingleBoldValueRequest(sheetId, assignment.Player.Name, 0, 1);


        private static List<Request> BuildWarlockStuff(int sheetId, PersonalAssignment assignment)
        {
            var requests = new List<Request>();

            if (assignment.Player.Class != PlayerClass.Warlock)
            {
                requests.Add(SingleValueRequest(sheetId, "N/A", 16, 1));
                requests.Add(SingleValueRequest(sheetId, "", 17, 1));
                requests.Add(SingleValueRequest(sheetId, "", 18, 1));
                return requests;
            }

            var gatewaySide = SingleValueRequest(sheetId, assignment.Gateway?.Side.ToString() ?? "N/A", 16, 1);
            var gatewayPosition = SingleValueRequest(sheetId, assignment.Gateway?.Position == GatewayPosition.Close ? "Bottom (Close)" :  "Top (Far)" ?? string.Empty, 17, 1);
            var circleSide = SingleValueRequest(sheetId, assignment.Teleport?.Side.ToString() ?? "N/A", 18, 1);

            requests.Add(gatewaySide);
            requests.Add(gatewayPosition);
            requests.Add(circleSide);

            return requests;
        }

        private static List<Request> BuildMonkStuff(int sheetId, PersonalAssignment assignment)
        {
            var requests = new List<Request>();

            if (assignment.Player.Class != PlayerClass.Monk)
            {
                requests.Add(SingleValueRequest(sheetId, "N/A", 16, 4));
                requests.Add(SingleValueRequest(sheetId, "", 17, 4));
                return requests;
            }

            var transcendenceSide = SingleValueRequest(sheetId, assignment.Teleport?.Side.ToString() ?? "N/A", 16, 4);
            var statueSide = SingleValueRequest(sheetId, assignment.Statue?.Side.ToString() ?? "N/A", 17, 4);

            requests.Add(transcendenceSide);
            requests.Add(statueSide);

            return requests;
        }

        private static List<Request> BuildInterruptRequests(int sheetId, List<PersonalInterruptAssignment> assignment)
        {
            var requests = new List<Request>();

            requests.AddRange(BuildInterruptSet(sheetId, 1, assignment.FirstOrDefault(a => a.SetNumber == 1)));
            requests.AddRange(BuildInterruptSet(sheetId, 2, assignment.FirstOrDefault(a => a.SetNumber == 2)));
            requests.AddRange(BuildInterruptSet(sheetId, 3, assignment.FirstOrDefault(a => a.SetNumber == 3)));
            requests.AddRange(BuildInterruptSet(sheetId, 4, assignment.FirstOrDefault(a => a.SetNumber == 4)));

            return requests;
        }

        private static List<Request> BuildInterruptSet(int sheetId, int setNumber, PersonalInterruptAssignment assignment)
        {
            var requests = new List<Request>();

            var columnNumber = 1 + (3 * (setNumber - 1));

            if (assignment == null)
            {
                requests.Add(SingleValueRequest(sheetId, "N/A", 21, columnNumber));
                requests.Add(SingleValueRequest(sheetId, string.Empty, 22, columnNumber));
                requests.Add(SingleValueRequest(sheetId, string.Empty, 23, columnNumber));
                return requests;
            }

            requests.Add(SingleValueRequest(sheetId, assignment.AddNumber.ToString(), 21, columnNumber));
            requests.Add(SingleBoldValueRequest(sheetId, assignment.Partner.Name, 22, columnNumber));
            requests.Add(SingleValueRequest(sheetId, assignment.Order == 1 ? "First" : "Second", 23, columnNumber));

            return requests;
        }

        private static List<Request> SetTendrilAssignments(int sheetId, PersonalTendrilAssignment assignment)
        {
            var requests = new List<Request>();

            if (assignment == null)
            {
                requests.Add(SingleValueRequest(sheetId, "N/A", 26, 1));
                requests.Add(SingleValueRequest(sheetId, string.Empty, 27, 1));
                return requests;
            }
            
            requests.Add(SingleValueRequest(sheetId, assignment.Order == 1 ? "First" : "Second", 26, 1));
            requests.Add(SingleValueRequest(sheetId, assignment.Partner.Name, 27, 1));
            return requests;
        }

        private static Request SetBurstingBoilAreaRequest(int sheetId, PersonalBurstingBoilAssignment assignment)
            => SingleValueRequest(sheetId, assignment.Area.ToString(), 30, 1);
        private static Request SetP3AreaRequest(int sheetId, PersonalP3AreaAssignment assignment)
            => SingleValueRequest(sheetId, ToSentenceCase(assignment.Area.ToString()), 33, 1);

        private static List<Request> BuildSetOrbRequests(int sheetId, OrbAssignment assignment)
        {
            var requests = new List<Request>();

            var groupNumber = SingleValueRequest(sheetId, assignment.Set.ToString(), 3, 1);
            var direction = SingleValueRequest(sheetId, assignment.Side.ToString(), 4, 1);
            var role = SingleValueRequest(sheetId, assignment.Role == OrbRole.Catcher ? "Catcher (2nd Person)" : "Thrower (1st Person)", 5, 1);
            var bestBuddy = SingleBoldValueRequest(sheetId, assignment.Parner.Name, 6, 1);
            var orbSets = SingleValueRequest(sheetId, OrbSets(assignment.Set), 7, 1);
            var macro = SetMacro(sheetId, assignment.Macro);
            var whisperer = SingleBoldValueRequest(sheetId, assignment.Whisperer.Name, 13, 1);

            requests.Add(groupNumber);
            requests.Add(direction);
            requests.Add(role);
            requests.Add(bestBuddy);
            requests.Add(orbSets);
            requests.AddRange(macro);
            requests.Add(whisperer);

            return requests;
        }

        private static string OrbSets(int set)
        {
            switch (set)
            {
                case 1: return "1 (1st set, 1st) + 6 (2nd set, 3rd)";
                case 2: return "2 (1st set, 2nd) + 7 (3rd set, 1st)";
                case 3: return "3 (1st set, 3rd) + 8 (3rd set, 2nd)";
                case 4: return "4 (2nd set, 1st) + 9 (3rd set, 3rd)";
                case 5: return "5 (2nd set, 2nd)";
                default: return "";
            }
        }

        private static string ToSentenceCase(string str) =>  Regex.Replace(str, "[a-z][A-Z]", m => $"{m.Value[0]} {m.Value[1]}");

        private static List<Request> SetMacro(int sheetId, string macro)
        {
            var requests = new List<Request>();

            if (string.IsNullOrEmpty(macro))
            {
                requests.Add(SingleValueRequest(sheetId, "N/A", 9, 1));
                requests.Add(SingleValueRequest(sheetId, "", 10, 1));
                requests.Add(SingleValueRequest(sheetId, "", 11, 1));
                return requests;
            }

            return macro.Split('\n').Select((m, i) => SingleConsolasValueRequest(sheetId, m, 9 + i, 1)).ToList();
        }

        private static Request SingleValueRequest(int sheetId, string value, int rowIndex, int columnIndex) => BuildUpdateCellRequest(sheetId, SingleValue(value), rowIndex, columnIndex);
        private static Request SingleBoldValueRequest(int sheetId, string value, int rowIndex, int columnIndex) => BuildUpdateCellRequest(sheetId, SingleBoldValue(value), rowIndex, columnIndex);
        private static Request SingleConsolasValueRequest(int sheetId, string value, int rowIndex, int columnIndex) => BuildUpdateCellRequest(sheetId, SingleConsolasValue(value), rowIndex, columnIndex);

        private static List<RowData> SingleBoldValue(string value) => SingleValue(value, true, "Arial");
        private static List<RowData> SingleValue(string value) => SingleValue(value, false, "Arial");
        private static List<RowData> SingleConsolasValue(string value) => SingleValue(value, false, "Consolas");

        private static List<RowData> SingleValue(string value, bool bold, string fontFamily)
        {
            return new List<RowData>
            {
                new RowData
                {
                    Values = new List<CellData>
                    {
                        new CellData
                        {
                            UserEnteredValue = new ExtendedValue { StringValue = value },
                            UserEnteredFormat = new CellFormat
                            {
                                HorizontalAlignment = "Left",
                                TextFormat = new TextFormat
                                {
                                    Bold = bold,
                                    FontFamily = fontFamily
                                }
                            }
                        }
                    }
                }
            };
        }

        private static Request BuildUpdateCellRequest(int sheetId, List<RowData> data, int rowIndex, int columnIndex)
        {
            return new Request
            {
                UpdateCells = new UpdateCellsRequest
                {
                    Fields = "*",
                    Rows = data,
                    Start = new GridCoordinate
                    {
                        RowIndex = rowIndex,
                        ColumnIndex = columnIndex,
                        SheetId = sheetId
                    }
                }
            };
        }

        private static async Task<Dictionary<string, int>> CopyAssignmentSpreadsheets(List<string> names)
        {
            var requests = new List<Request>();

            foreach (var name in names)
            {
                var duplicateSheetRequest = new Request
                {
                    DuplicateSheet = new DuplicateSheetRequest
                    {
                        SourceSheetId = AssignmentTemplateId,
                        NewSheetName = $"{PersonalAssignmentPrefix}{name}",
                        InsertSheetIndex = 4
                    }
                };

                requests.Add(duplicateSheetRequest);
            }

            var response = await ExecuteBatchRequest(requests);

            var result = new Dictionary<string, int>();
            for (int i = 0; i < names.Count; i++)
            {
                result.Add(names[i], response.Replies[i].DuplicateSheet.Properties.SheetId.Value);
            }

            return result;
        }

        private static async Task<BatchUpdateSpreadsheetResponse> ExecuteBatchRequest(IEnumerable<Request> requests)
        {
            var service = await SpreadsheetService.GetSheetsService();

            var batchUpdateRequest = new BatchUpdateSpreadsheetRequest { Requests = requests.ToList() };
            return await service.Spreadsheets.BatchUpdate(batchUpdateRequest, SpreadsheetService.SpreadsheetId).ExecuteAsync();
        }
    }
}
