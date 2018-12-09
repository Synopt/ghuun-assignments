using GameConcepts.Orbs;
using System.Collections.Generic;

namespace GameConcepts.Teleports
{
    public static class TeleportAssignmentLogic
    {
        public static IEnumerable<TeleportAssignment> GetTeleportAssignments(List<OrbAssignment> orbAssignments)
        {
            foreach (var orbAssignment in orbAssignments)
            {
                if(orbAssignment.Role == OrbRole.Catcher && orbAssignment.Player.IsTeleporter)
                {
                    yield return new TeleportAssignment { Side = orbAssignment.Side, Player = orbAssignment.Player };
                }
            }
        }
    }
}
