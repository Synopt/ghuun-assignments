using GameConcepts.Orbs;
using GameConcepts.Players;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameConcepts.PhaseThreeAreas
{
    public static class PhaseThreeAreaAssignmentLogic
    {

        public static List<PersonalP3AreaAssignment> ListAssignments(PhaseThreeAreaAssignment assignments)
        {
            return assignments.Areas.SelectMany(a => a.Value.Select(p => new PersonalP3AreaAssignment
            {
                Area = a.Key,
                Player = p
            })).ToList();
        }

        public static PhaseThreeAreaAssignment AssignPhaseThreeAreas(List<OrbAssignment> orbAssignments)
        {
            var assignments = new PhaseThreeAreaAssignment();
            var playersLeft = orbAssignments;

            playersLeft = AssignHealers(assignments, orbAssignments);
            playersLeft = AssignMelee(assignments, playersLeft);
            AssignRest(assignments, playersLeft);

            return assignments;
        }

        private static List<OrbAssignment> AssignHealers(PhaseThreeAreaAssignment assignments, List<OrbAssignment> orbAssignments)
        {
            var area = (PhaseThreeArea)0;
            var healers = orbAssignments.Where(a => a.Player.Role == PlayerRole.Healer);

            foreach (var healer in healers.OrderBy(Positions))
            {
                assignments.Areas[area].Add(healer.Player);
                area = area == (PhaseThreeArea)4 ? 0 : area + 1;
            }

            return orbAssignments.Except(healers).ToList();
        }

        private static List<OrbAssignment> AssignMelee(PhaseThreeAreaAssignment assignments, List<OrbAssignment> orbAssignments)
        {
            var area = (PhaseThreeArea)0;
            var meleeDps = orbAssignments.Where(a => a.Player.Role == PlayerRole.MeleeDps);

            foreach (var melee in meleeDps.OrderBy(Positions))
            {
                assignments.Areas[area].Add(melee.Player);
                area = area == (PhaseThreeArea)4 ? 0 : area + 1;
            }

            return orbAssignments.Except(meleeDps).ToList();
        }


        private static void AssignRest(PhaseThreeAreaAssignment assignments, List<OrbAssignment> orbAssignments)
        {
            foreach (var orbAssignment in orbAssignments.OrderBy(Positions))
            {
                foreach (PhaseThreeArea area in Enum.GetValues(typeof(PhaseThreeArea)))
                {
                    if (assignments.Areas[area].Count() < 4)
                    {
                        assignments.Areas[area].Add(orbAssignment.Player);
                        break;
                    }
                }
            }
        }

        private static int Positions(OrbAssignment assignment)
        {
            if (assignment.Role == OrbRole.Thrower)
            {
                return assignment.Side == OrbSide.Left ? 0 : 2;
            }

            return 1;
        }
    }
}
