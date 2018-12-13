using GameConcepts.Orbs;
using System.Collections.Generic;
using System.Linq;

namespace GameConcepts.BurstingBoilAreas
{
    public static class BurstingBoilAssignmentLogic
    {
        private const int NumStars = 6;
        private const int MaxMoon = 7;
        private const int DiamondMax = 7;

        public static List<PersonalBurstingBoilAssignment> ListAssignments(BurstingBoilAssignment assignments)
        {
            return assignments.Sides.SelectMany(s => s.Value.Select(p => new PersonalBurstingBoilAssignment
            {
                Area = s.Key,
                Player = p
            })).ToList();
        }

        public static BurstingBoilAssignment AssignBurstingBoilAreas(List<OrbAssignment> orbAssignments)
        {
            var playersLeft = orbAssignments;
            var boilAssignments = new BurstingBoilAssignment();

            playersLeft = AssignLeftThrowers(playersLeft, boilAssignments);
            playersLeft = AssignRightThrowers(playersLeft, boilAssignments);
            playersLeft = AssignHealers(playersLeft, boilAssignments);
            AssignRest(playersLeft, boilAssignments);

            return boilAssignments;
        }

        private static List<OrbAssignment> AssignLeftThrowers(List<OrbAssignment> playersLeft, BurstingBoilAssignment boilAssignments)
        {
            var leftThrowers = playersLeft.Where(a => a.Side == OrbSide.Left && a.Role == OrbRole.Thrower).ToList();
            boilAssignments.Sides[BurstingBoilArea.Moon] = leftThrowers.Select(a => a.Player).ToList();

            return playersLeft.Except(leftThrowers).ToList();
        }

        private static List<OrbAssignment> AssignRightThrowers(List<OrbAssignment> playersLeft, BurstingBoilAssignment boilAssignments)
        {
            var rightThrowers = playersLeft.Where(a => a.Side == OrbSide.Right && a.Role == OrbRole.Thrower).ToList();
            boilAssignments.Sides[BurstingBoilArea.Diamond] = rightThrowers.Select(a => a.Player).ToList();

            return playersLeft.Except(rightThrowers).ToList();
        }

        private static List<OrbAssignment> AssignHealers(List<OrbAssignment> playersLeft, BurstingBoilAssignment boilAssignments)
        {
            var numMoonHealers = boilAssignments.Sides[BurstingBoilArea.Moon].Count(p => p.Role == Players.PlayerRole.Healer);
            var numDiamondHealers = boilAssignments.Sides[BurstingBoilArea.Diamond].Count(p => p.Role == Players.PlayerRole.Healer);
            var healersLeft = playersLeft.Where(a => a.Player.Role == Players.PlayerRole.Healer).ToList();

            foreach (var healer in healersLeft)
            {
                if(numMoonHealers < 2 && healer.Side == OrbSide.Left)
                {
                    boilAssignments.Sides[BurstingBoilArea.Moon].Add(healer.Player);
                    numMoonHealers++;
                    continue;
                }
                if (numDiamondHealers < 2 && healer.Side == OrbSide.Right)
                {
                    boilAssignments.Sides[BurstingBoilArea.Diamond].Add(healer.Player);
                    numDiamondHealers++;
                    continue;
                }
                boilAssignments.Sides[BurstingBoilArea.Star].Add(healer.Player);
            }

            return playersLeft.Except(healersLeft).ToList();
        }

        private static void AssignRest(List<OrbAssignment> playersLeft, BurstingBoilAssignment boilAssignments)
        {
            var numMoon = boilAssignments.Sides[BurstingBoilArea.Moon].Count();
            var numDiamond = boilAssignments.Sides[BurstingBoilArea.Diamond].Count();

            foreach (var assignment in playersLeft)
            {
                if (numMoon < MaxMoon && assignment.Side == OrbSide.Left)
                {
                    boilAssignments.Sides[BurstingBoilArea.Moon].Add(assignment.Player);
                    numMoon++;
                    continue;
                }
                if (numDiamond < DiamondMax && assignment.Side == OrbSide.Right)
                {
                    boilAssignments.Sides[BurstingBoilArea.Diamond].Add(assignment.Player);
                    numDiamond++;
                    continue;
                }
                boilAssignments.Sides[BurstingBoilArea.Star].Add(assignment.Player);
            }
        }
    }
}
