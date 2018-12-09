using GameConcepts.Orbs;
using GameConcepts.Players;
using System.Collections.Generic;
using System.Linq;

namespace GameConcepts.Gateways
{
    public class GatewayAssignmentLogic
    {
        private GatewayAssignment _assignment = new GatewayAssignment();

        public GatewayAssignment AssignPlayers(List<OrbAssignment> orbAssignments)
        {
            var warlocks = orbAssignments.Where(a => a.Player.Class == PlayerClass.Warlock).ToList();
            var teleportingLocks = warlocks.Where(w => w.Role == OrbRole.Catcher).ToList();

            var leftOnlyLocks = teleportingLocks.Where(w => w.Side == OrbSide.Left).ToList();
            var rightOnlyLocks = teleportingLocks.Where(w => w.Side == OrbSide.Right).ToList();
            var flexibleLocks = warlocks.Where(w => w.Role == OrbRole.Thrower).ToList();

            AssignLeftSide(leftOnlyLocks.Union(flexibleLocks));
            AssignRightSide(rightOnlyLocks.Union(flexibleLocks));

            return _assignment;
        }

        private void AssignLeftSide(IEnumerable<OrbAssignment> warlocks)
        {
            foreach (var warlock in warlocks)
            {
                if (IsAssigned(warlock.Player)) { continue; }

                if (_assignment.FarLeft == null)
                {
                    _assignment.FarLeft = warlock.Player;
                    continue;
                }
                if (_assignment.CloseLeft == null)
                {
                    _assignment.CloseLeft = warlock.Player;
                    break;
                }
            }
        }

        private void AssignRightSide(IEnumerable<OrbAssignment> warlocks)
        {
            foreach (var warlock in warlocks)
            {
                if (IsAssigned(warlock.Player)) { continue; }

                if (_assignment.FarRight == null)
                {
                    _assignment.FarRight = warlock.Player;
                    continue;
                }
                if (_assignment.CloseRight == null)
                {
                    _assignment.CloseRight = warlock.Player;
                    break;
                }
            }
        }

        private bool IsAssigned(Player player)
        {
            return _assignment.CloseLeft == player || _assignment.CloseRight == player || _assignment.FarLeft == player || _assignment.FarRight == player;
        }
    }
}
