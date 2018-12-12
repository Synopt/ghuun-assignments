using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Linq;
using GameConcepts.Players;

namespace Sheets
{
    public static class SpreadsheetService
    {
        private static string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private static string ApplicationName = "Ghuun Assignments";
        private const string SpreadsheetId = "1ggJaUmCahwZ0lE6EyKd_Iz20wgZSykG5RUJcxpIDSbU";
        private const int AssignmentTemplateId = 695235050;

        public static async Task CopyAssignmentSpreadsheet(string newName)
        {
            var service = await GetSheetsService();

            var newSheet = new Request
            {
                DuplicateSheet = new DuplicateSheetRequest
                {
                    SourceSheetId = AssignmentTemplateId,
                    NewSheetName = $"A - {newName}",
                    InsertSheetIndex = 99
                }
            };

            var y = new BatchUpdateSpreadsheetRequest { Requests = new List<Request> { newSheet } };
            var response = await service.Spreadsheets.BatchUpdate(y, SpreadsheetId).ExecuteAsync();
        }

        public static async Task<IEnumerable<SheetProperties>> GetAllSheets()
        {
            var service = await GetSheetsService();
            var sheets = await service.Spreadsheets.Get(SpreadsheetId).ExecuteAsync();
            return sheets.Sheets.Select(s => s.Properties);
        }

        public static async Task<List<Player>> GetTeam()
        {
            var service = await GetSheetsService();
            var range = "Roster!A2:F";

            var request = service.Spreadsheets.Values.Get(SpreadsheetId, range);

            var response = await request.ExecuteAsync();
            var values = response.Values;
            var team = new List<Player>();

            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    if (((string)row[5]).ToLower() == "yes")
                    {
                        team.Add(new Player
                        {
                            Name = (string)row[0],
                            Role = ParseRole((string)row[1]),
                            Class = ParseClass((string)row[2]),
                            WhisperName = (string)row[3],
                            Server = (string)row[4]
                        });
                    }
                }
            }

            if (team.Count() > 20)
            {
                throw new InvalidOperationException("Can't have more than 20 people in the team");
            }

            return team;
        }

        internal static async Task<SheetsService> GetSheetsService()
        {
            UserCredential credentials;
            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credentials = await GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(stream).Secrets, Scopes, "user", CancellationToken.None, new FileDataStore(credPath, true));
            }

            return new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credentials,
                ApplicationName = ApplicationName,
            });
        }

        internal static async Task UpdateSpreadsheet(string range, List<IList<object>> values)
        {
            var service = await GetSheetsService();

            var valueRange = new ValueRange();
            valueRange.Values = values;

            var updateRequest = service.Spreadsheets.Values.Update(valueRange, SpreadsheetId, range);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            updateRequest.Execute();
        }

        internal static PlayerClass ParseClass(string input)
        {
            if (Enum.TryParse<PlayerClass>(input.Replace(" ", ""), out var result))
            {
                return result;
            };
            throw new ArgumentOutOfRangeException(nameof(input), $"{input} is not a valid {nameof(PlayerClass)}");
        }

        internal static PlayerRole ParseRole(string input)
        {
            if (Enum.TryParse<PlayerRole>(input.Replace(" ", ""), out var result))
            {
                return result;
            };
            throw new ArgumentOutOfRangeException(nameof(input), $"{input} is not a valid {nameof(PlayerRole)}"); ;
        }
    }
}
