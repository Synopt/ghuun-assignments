using GameConcepts.Orbs;
using System.Collections.Generic;
using System.Linq;

namespace GameConcepts.BurstingBoilAreas
{
    public static class BurstingBoilAssignmentLogic
    {
        private const int NumStars = 6;

        public static BurstingBoilAssignment AssignBurstingBoilAreas(List<OrbAssignment> orbAssignments)
        {
            var assignment = new BurstingBoilAssignment();

            var leftThrowers = orbAssignments.Where(a => a.Side == OrbSide.Left && a.Role == OrbRole.Thrower).Select(a => a.Player);
            var rightThrowers = orbAssignments.Where(a => a.Side == OrbSide.Right && a.Role == OrbRole.Thrower).Select(a => a.Player);
            var catchers = orbAssignments.Where(a => a.Role == OrbRole.Catcher).Select(a => a.Player).ToList();

            assignment.Sides[BurstingBoilArea.Moon] = leftThrowers.ToList();
            assignment.Sides[BurstingBoilArea.Diamond] = rightThrowers.ToList();
            assignment.Sides[BurstingBoilArea.Star] = catchers.Take(NumStars).ToList();

            var toggle = BurstingBoilArea.Moon;
            foreach (var player in catchers.Skip(NumStars))
            {
                assignment.Sides[toggle].Add(player);

                toggle = toggle == BurstingBoilArea.Moon ? BurstingBoilArea.Diamond : BurstingBoilArea.Moon;
            }

            return assignment;
        }
    }
}
