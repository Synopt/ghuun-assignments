using GameConcepts.Orbs;
using GameConcepts.Players;
using System.Collections.Generic;
using System.Linq;

namespace GameConcepts.Statues
{
    public static class StatueAssignmentLogic
    {
        public static IEnumerable<StatueAssignment> GetStatueAssignment(List<OrbAssignment> orbAssignments)
        {
            foreach (var orbAssignment in orbAssignments)
            {
                if (orbAssignment.Player.Class == PlayerClass.Monk && orbAssignment.Player.Role == PlayerRole.Healer)
                {
                    if (!IsDruidAssignedAsThrowerToThatSide(orbAssignments, orbAssignment.Side)) { continue; }
                    yield return new StatueAssignment { Side = orbAssignment.Side, Player = orbAssignment.Player };
                }
            }
        }

        private static bool IsDruidAssignedAsThrowerToThatSide(List<OrbAssignment> orbAssignments, OrbSide side)
        {
            return orbAssignments.Any(a => a.Player.Class == PlayerClass.Druid && a.Role == OrbRole.Catcher && a.Side == side);
        }
    }
}
