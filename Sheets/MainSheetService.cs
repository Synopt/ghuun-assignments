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
using GameConcepts.Orbs;
using GameConcepts.Gateways;
using GameConcepts.Teleports;
using GameConcepts.Players;
using GameConcepts.Statues;
using GameConcepts.Interrupts;
using GameConcepts.BurstingBoilAreas;
using GameConcepts.PhaseThreeAreas;

namespace Sheets
{
    public static class MainSheetService
    {

        public static async Task WriteOrbAssignments(List<OrbAssignment> orbAssignments)
        {
            var values = new List<IList<object>> { };

            foreach (var orbSets in orbAssignments.GroupBy(a => a.Set).OrderBy(g => g.Key))
            {
                foreach (var sides in orbSets.GroupBy(o => o.Side))
                {
                    var thrower = sides.Select(g => g).FirstOrDefault(g => g.Role == OrbRole.Thrower);
                    var catcher = sides.Select(g => g).FirstOrDefault(g => g.Role == OrbRole.Catcher);

                    values.Add(new List<object> { thrower?.Player?.Name ?? string.Empty, catcher?.Player?.Name ?? string.Empty });
                }
                values.Add(new List<object>());
            }

            await SpreadsheetService.UpdateSpreadsheet("G'huun Mythic Assignments!E4:F18", values);
        }

        public static async Task WriteOrbMacros(List<OrbAssignment> orbAssignments)
        {
            var values = new List<IList<object>> { };

            foreach (var assignment in orbAssignments)
            {
                if (assignment.Role == OrbRole.Thrower) { continue; }

                values.Add(new List<object> { assignment.Player.Name });
                var oppositeSide = assignment.Side == OrbSide.Left ? OrbSide.Right : OrbSide.Left;
                var otherCatcher = orbAssignments.FirstOrDefault(a => a.Set == assignment.Set && a.Side == oppositeSide && a.Role == OrbRole.Catcher);

                var nextSet = assignment.Set == 5 ? 1 : assignment.Set + 1;
                var nextCatcher = orbAssignments.FirstOrDefault(a => a.Set == nextSet && a.Side == assignment.Side && a.Role == OrbRole.Catcher);
                var nextThrower = orbAssignments.FirstOrDefault(a => a.Set == nextSet && a.Side == assignment.Side && a.Role == OrbRole.Thrower);

                if (otherCatcher?.Player?.Name != null)
                {
                    values.Add(new List<object> { $"/w {otherCatcher.Player.FullyQualifiedName} orb ready" });
                }

                if (nextCatcher?.Player?.Name != null)
                {
                    values.Add(new List<object> { $"/w {nextCatcher.Player.FullyQualifiedName} youre next" });
                }

                if (nextThrower?.Player?.Name != null)
                {
                    values.Add(new List<object> { $"/w {nextThrower.Player.FullyQualifiedName} youre next" });
                }
            }

            await SpreadsheetService.UpdateSpreadsheet("Macros!A1:A", values);
        }

        public static async Task WriteStatueAssignments(IEnumerable<StatueAssignment> statueAssignment)
        {
            var values = new List<IList<object>> { };

            var assignments = statueAssignment.ToList();

            values.Add(new List<object> { statueAssignment.FirstOrDefault(a => a.Side == OrbSide.Left)?.Player?.Name ?? string.Empty });
            values.Add(new List<object> { statueAssignment.FirstOrDefault(a => a.Side == OrbSide.Right)?.Player?.Name ?? string.Empty });

            await SpreadsheetService.UpdateSpreadsheet("G'huun Mythic Assignments!F22:F23", values);
        }

        public static async Task WriteTeleportAssignments(IEnumerable<TeleportAssignment> teleportAssignments)
        {
            var values = new List<IList<object>> { };

            var assignments = teleportAssignments.ToList();

            var leftAssignments = teleportAssignments.Where(a => a.Side == OrbSide.Left).Select(a => a.Player.Name).PadTo(5);
            var rightAssignments = teleportAssignments.Where(a => a.Side == OrbSide.Right).Select(a => a.Player.Name).PadTo(5);

            values.Add(new List<object>(leftAssignments));
            values.Add(new List<object>(rightAssignments));

            await SpreadsheetService.UpdateSpreadsheet("G'huun Mythic Assignments!D26:H27", values);
        }

        public static async Task WriteGatewayAssignments(GatewayAssignment gatewayAssignment)
        {
            var values = new List<IList<object>> { };

            values.Add(new List<object> { gatewayAssignment.Side[OrbSide.Left].Position[GatewayPosition.Close]?.Name ?? "", gatewayAssignment.Side[OrbSide.Left].Position[GatewayPosition.Far]?.Name ?? "" });
            values.Add(new List<object> { gatewayAssignment.Side[OrbSide.Right].Position[GatewayPosition.Close]?.Name ?? "", gatewayAssignment.Side[OrbSide.Right].Position[GatewayPosition.Far]?.Name ?? "" });

            await SpreadsheetService.UpdateSpreadsheet("G'huun Mythic Assignments!D22:E23", values);
        }

        public static async Task WriteInterruptAssignments(InterruptAssignment interruptAssignments)
        {
            var values = new List<IList<object>> { };

            values.Add(WriteInterruptSet(interruptAssignments.Sets[1], 1, 5));
            values.Add(WriteInterruptSet(interruptAssignments.Sets[1], 2, 5));
            values.Add(WriteInterruptSet(interruptAssignments.Sets[2], 1, 3));
            values.Add(WriteInterruptSet(interruptAssignments.Sets[2], 2, 3));
            values.Add(WriteInterruptSet(interruptAssignments.Sets[3], 1, 3));
            values.Add(WriteInterruptSet(interruptAssignments.Sets[3], 2, 3));
            values.Add(WriteInterruptSet(interruptAssignments.Sets[4], 1, 3));
            values.Add(WriteInterruptSet(interruptAssignments.Sets[4], 2, 3));

            await SpreadsheetService.UpdateSpreadsheet("G'huun Mythic Assignments!E40:I47", values);
        }

        public static async Task WriteTendrilAssignments(InterruptAssignment interruptAssignments)
        {
            var values = new List<IList<object>> { };

            values.Add(new List<object> { "Tendrils" });
            values.Add(new List<object> { interruptAssignments.Tendrils.Interrupts[1]?.Name ?? string.Empty });
            values.Add(new List<object> { interruptAssignments.Tendrils.Interrupts[2]?.Name ?? string.Empty });
            values.Add(new List<object> { interruptAssignments.Tendrils.Interrupts[1]?.Name ?? string.Empty });
            values.Add(new List<object> { interruptAssignments.Tendrils.Interrupts[2]?.Name ?? string.Empty });

            await SpreadsheetService.UpdateSpreadsheet("G'huun Mythic Assignments!I43:I47", values);
        }

        private static IList<object> WriteInterruptSet(InterruptSet interruptSet, int interruptNumber, int padTo)
        {
            return new List<object>(interruptSet.Adds.Select(a => a.Value.Interrupts[interruptNumber]?.Name ?? string.Empty).PadTo(padTo));
        }

        public static async Task WriteBurstingBoilAssignments(BurstingBoilAssignment burstingBoilAssignments)
        {
            var values = new List<IList<object>> { };

            values.Add(new List<object>(burstingBoilAssignments.Sides[BurstingBoilArea.Star].Select(p => p.Name).PadTo(7)));
            values.Add(new List<object>(burstingBoilAssignments.Sides[BurstingBoilArea.Moon].Select(p => p.Name).PadTo(7)));
            values.Add(new List<object>(burstingBoilAssignments.Sides[BurstingBoilArea.Diamond].Select(p => p.Name).PadTo(7)));

            await SpreadsheetService.UpdateSpreadsheet("G'huun Mythic Assignments!C112:I114", values);
        }

        public static async Task WriteP3Assignments(PhaseThreeAreaAssignment p3Assignments)
        {
            var values = new List<IList<object>> { };

            foreach (var area in p3Assignments.Areas)
            {
                values.Add(new List<object>(area.Value.OrderBy(SpreadsheetOrder).Select(p => p.Name).PadTo(4)));
            }

            int SpreadsheetOrder(Player player) => player.Role == PlayerRole.MeleeDps ? 0 : player.Role == PlayerRole.RangedDps || player.Role == PlayerRole.Tank ? 1 : 2;

            await SpreadsheetService.UpdateSpreadsheet("G'huun Mythic Assignments!C140:F144", values);
        }
    }
}
