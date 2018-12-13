using GameConcepts.Orbs;
using GameConcepts.Players;
using System.Collections.Generic;
using System.Linq;

namespace GameConcepts.Gateways
{
    public static class GatewayAssignmentLogic
    {
        public static List<PersonalGatewayAssignment> ListAssignments(GatewayAssignment assignments)
        {
            return assignments.Side.SelectMany(s => s.Value.Position.Select(p => new PersonalGatewayAssignment
            {
                Player = p.Value,
                Position = p.Key,
                Side = s.Key,
            })).ToList();
        }

        public static GatewayAssignment AssignGateways(List<Player> team)
        {
            var assignment = new GatewayAssignment();

            var warlocks = team.Where(p => p.Class == PlayerClass.Warlock).OrderBy(w => w.Name).ToList();

            foreach (var warlock in warlocks)
            {
                if (Assign(assignment, OrbSide.Left, GatewayPosition.Far, warlock))
                {
                    continue;
                }
                if (Assign(assignment, OrbSide.Right, GatewayPosition.Far, warlock))
                {
                    continue;
                }
                if (Assign(assignment, OrbSide.Left, GatewayPosition.Close, warlock))
                {
                    continue;
                }
                if (Assign(assignment, OrbSide.Right, GatewayPosition.Close, warlock))
                {
                    break;
                }
            }

            return assignment;
        }

        private static bool Assign(GatewayAssignment assignment, OrbSide side, GatewayPosition position, Player warlock)
        {
            if (assignment.Side[side].Position[position] != null) { return false; }

            assignment.Side[side].Position[position] = warlock;
            return true;
        }
    }
}
