using GameConcepts.Orbs;
using GameConcepts.Players;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameConcepts.Interrupts
{
    public static class InterruptAssignmentLogic
    {
        public static InterruptAssignment AssignInterrupts(List<OrbAssignment> orbAssignments)
        {
            var interruptAssignment = new InterruptAssignment();

            AssignFirstSet(interruptAssignment, orbAssignments);
            AssignSecondSet(interruptAssignment, orbAssignments);
            AssignThirdSet(interruptAssignment, orbAssignments);
            AssignFourthSet(interruptAssignment, orbAssignments);
            AssignTendrils(interruptAssignment, orbAssignments);

            return interruptAssignment;
        }

        private static void AssignFirstSet(InterruptAssignment interruptAssignment, List<OrbAssignment> orbAssignments)
        {
            var firstSet = interruptAssignment.Sets[1];

            var validPlayersFirst3 = orbAssignments.Where(a => a.Set != 1).OrderBy(a => PreferMelee(a.Player)).ThenBy(p => p.Player.Name).ToList();

            var aoeStunners = validPlayersFirst3.Where(p => p.Player.HasAoeStun);
            foreach (var stunner in aoeStunners)
            {
                if(firstSet.Adds[2].Interrupts[1] == null)
                {
                    firstSet.Adds[2].Interrupts[1] = stunner.Player;
                    firstSet.Adds[3].Interrupts[1] = stunner.Player;
                    continue;
                }
                if (firstSet.Adds[2].Interrupts[2] == null)
                {
                    firstSet.Adds[2].Interrupts[2] = stunner.Player;
                    firstSet.Adds[3].Interrupts[2] = stunner.Player;
                    break;
                }
            }

            AssignRest(validPlayersFirst3, firstSet.Adds[1], firstSet);
            AssignRest(validPlayersFirst3, firstSet.Adds[2], firstSet);
            AssignRest(validPlayersFirst3, firstSet.Adds[3], firstSet);
            
            var validPlayersLast2 = orbAssignments.Where(a => a.Set != 2 && a.Set != 1).OrderBy(a => PreferRanged(a.Player)).ThenBy(p => p.Player.Name).ToList();
            
            AssignRest(validPlayersLast2, firstSet.Adds[4], firstSet);
            AssignRest(validPlayersLast2, firstSet.Adds[5], firstSet);
        }

        private static void AssignSecondSet(InterruptAssignment interruptAssignment, List<OrbAssignment> orbAssignments)
        {
            var secondSet = interruptAssignment.Sets[2];
            var validPlayers = orbAssignments.Where(a => a.Set != 2).OrderBy(a => PreferRanged(a.Player)).ThenBy(p => p.Player.Name).ToList();
            
            AssignRest(validPlayers, secondSet.Adds[1], secondSet);
            AssignRest(validPlayers, secondSet.Adds[2], secondSet);
            AssignRest(validPlayers, secondSet.Adds[3], secondSet);
        }

        private static void AssignThirdSet(InterruptAssignment interruptAssignment, List<OrbAssignment> orbAssignments)
        {
            var thirdSet = interruptAssignment.Sets[3];
            var validPlayers = orbAssignments.Where(a => a.Set != 3).OrderBy(a => PreferRanged(a.Player)).ThenBy(p => p.Player.Name).ToList();

            AssignRest(validPlayers, thirdSet.Adds[1], thirdSet);
            AssignRest(validPlayers, thirdSet.Adds[2], thirdSet);
            AssignRest(validPlayers, thirdSet.Adds[3], thirdSet);
        }

        private static void AssignFourthSet(InterruptAssignment interruptAssignment, List<OrbAssignment> orbAssignments)
        {
            var fourthSet = interruptAssignment.Sets[4];
            var validPlayers = orbAssignments.Where(a => a.Set != 3).OrderBy(a => PreferRanged(a.Player)).ThenBy(p => p.Player.Name).ToList();

            AssignRest(validPlayers, fourthSet.Adds[1], fourthSet);
            AssignRest(validPlayers, fourthSet.Adds[2], fourthSet);
            AssignRest(validPlayers, fourthSet.Adds[3], fourthSet);
        }

        private static void AssignTendrils(InterruptAssignment interruptAssignment, List<OrbAssignment> orbAssignments)
        {
            var validPlayers = orbAssignments.Where(a => a.Set != 3).OrderBy(a => PreferMelee(a.Player)).ThenBy(p => p.Player.Name).ToList();

            foreach (var player in validPlayers)
            {
                if(IsAssigned(player.Player, interruptAssignment.Sets[3]) || IsAssigned(player.Player, interruptAssignment.Sets[4]))
                {
                    continue;
                }

                if(interruptAssignment.Tendrils.Interrupts[1] == null)
                {
                    interruptAssignment.Tendrils.Interrupts[1] = player.Player;
                    continue;
                }

                if (interruptAssignment.Tendrils.Interrupts[2] == null)
                {
                    interruptAssignment.Tendrils.Interrupts[2] = player.Player;
                    break;
                }
            }
        }

        private static void AssignRest(List<OrbAssignment> validChoices, InterruptAdd interruptAdd, InterruptSet set)
        {
            foreach (var assignment in validChoices)
            {
                if(IsAssigned(assignment.Player, set)) { continue; }

                if(interruptAdd.Interrupts[1] == null)
                {
                    interruptAdd.Interrupts[1] = assignment.Player;
                    continue;
                }
                if (interruptAdd.Interrupts[2] == null)
                {
                    interruptAdd.Interrupts[2] = assignment.Player;
                    break;
                }
            }
        }

        private static int PreferMelee(Player player)
        {
            switch (player.Role)
            {
                case PlayerRole.MeleeDps:
                    return 0;
                case PlayerRole.RangedDps:
                    return 1;
                case PlayerRole.Healer:
                    return 2;
                default:
                    return 3;
            }
        }

        private static int PreferRanged(Player player)
        {
            switch (player.Role)
            {
                case PlayerRole.RangedDps:
                    return 0;
                case PlayerRole.MeleeDps:
                    return 1;
                case PlayerRole.Healer:
                    return 2;
                default:
                    return 3;
            }
        }
        
        private static bool IsAssigned(Player player, InterruptSet set)
        {
            return set.Adds.Any(a => a.Value.Interrupts.Any(i => i.Value == player));
        }
    }
}
