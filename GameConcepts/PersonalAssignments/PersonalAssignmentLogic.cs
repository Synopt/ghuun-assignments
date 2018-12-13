using System.Collections.Generic;
using System.Linq;

namespace GameConcepts.PersonalAssignments
{
    public static class PersonalAssignmentLogic
    {
        public static List<PersonalAssignment> ListAssignments(PersonalAssignmentInput input)
        {
            var personalAssignments = new List<PersonalAssignment>();

            foreach (var orbAssignment in input.Orbs)
            {
                var player = orbAssignment.Player;

                var personalAssignment = new PersonalAssignment
                {
                    Player = player,
                    Orb = orbAssignment,
                    Interrupts = input.Interrupts.Where(i => i.Player == player).ToList(),
                    Tendril = input.Tendrils.FirstOrDefault(t => t.Player == player),
                    Gateway = input.Gateways.FirstOrDefault(g => g.Player == player),
                    Statue = input.Statues.FirstOrDefault(s => s.Player == player),
                    Teleport = input.Teleports.FirstOrDefault(t => t.Player == player),
                    BurstingBoil = input.BurstingBoils.FirstOrDefault(b => b.Player == player),
                    PhaseThree = input.PhaseThreeAreas.FirstOrDefault(p => p.Player == player)
                };

                personalAssignments.Add(personalAssignment);
            }

            return personalAssignments;
        }
    }
}
