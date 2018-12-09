using System.Linq;
using System.Collections.Generic;
using GameConcepts.Players;

namespace GameConcepts.Orbs
{
    public class OrbAssignmentLogic
    {
        private Dictionary<int, OrbSet> Assignments { get; set; }
        private bool MonkStatueOnRightSide = false;
        private bool MonkStatueOnLeftSide = false;

        public OrbAssignmentLogic()
        {
            Assignments = new Dictionary<int, OrbSet>
            {
                {1, new OrbSet() },
                {2, new OrbSet() },
                {3, new OrbSet() },
                {4, new OrbSet() },
                {5, new OrbSet() },
            };
        }

        public Dictionary<int, OrbSet> AssignPlayers(List<Player> team)
        {
            AssignTanks(team);
            AssignHealers(team);
            AssignWarlocks(team);
            AssignTeleportingDps(team);
            AssignRest(team);

            return Assignments;
        }

        public List<OrbAssignment> ListAssignments()
        {
            return Assignments.SelectMany(i => i.Value.Pairs.SelectMany(pa => pa.Value.Positions.Select(po => new OrbAssignment
            {
                Set = i.Key,
                Side = pa.Key,
                Role = po.Key,
                Player = po.Value
            }))).ToList();
        }

        private void AssignTanks(List<Player> team)
        {
            var tanks = team.OrderBy(p => p.Name).Where(p => p.Role == PlayerRole.Tank);
            var setsRemaining = new List<int> { 2, 3 };

            foreach (var tank in tanks)
            {
                var set = setsRemaining.FirstOrDefault();
                if(set == default(int)) { break; }

                var leftSide = set % 2 == 1;
                var pair = leftSide ? Assignments[set].LeftPair : Assignments[set].RightPair;
                if (tank.IsTeleporter) { pair.Catcher = tank; }
                
                else { pair.Thrower = tank; }

                setsRemaining.RemoveAt(0);
            }
        }

        private void AssignHealers(List<Player> team)
        {
            var healers = team.OrderBy(p => p.Name).Where(p => p.Role == PlayerRole.Healer);
            var setsRemaining = new List<int> { 1, 2, 3, 4, 5 };
            
            foreach (var healer in healers)
            {
                var set = setsRemaining.FirstOrDefault();
                if (set == default(int)) { break; }

                var leftSide = set % 2 == 0;
                var pair = leftSide ? Assignments[set].LeftPair : Assignments[set].RightPair;
                if (healer.IsTeleporter) { pair.Catcher = healer; }
                else { pair.Thrower = healer; }

                if (healer.Class == PlayerClass.Monk)
                {
                    if (leftSide)
                    {
                        MonkStatueOnLeftSide = true;
                    }
                    else
                    {
                        MonkStatueOnRightSide = true;
                    }
                }

                setsRemaining.RemoveAt(0);
            }
        }

        private void AssignWarlocks(List<Player> team)
        {
            var warlocks = team.OrderBy(p => p.Name).Where(p => p.Class == PlayerClass.Warlock).ToList();

            var toggle = OrbSide.Left;

            foreach (var warlock in warlocks)
            {
                foreach (var assignment in Assignments)
                {
                    if(toggle == OrbSide.Left)
                    {
                        if (assignment.Value.LeftPair.Catcher == null)
                        {
                            assignment.Value.LeftPair.Catcher = warlock;
                            break;
                        }
                    }
                    else
                    {
                        if (assignment.Value.RightPair.Catcher == null)
                        {
                            assignment.Value.RightPair.Catcher = warlock;
                            break;
                        }
                    }
                }

                toggle = toggle == OrbSide.Left ? OrbSide.Right : OrbSide.Left;
            }
        }

        private void AssignTeleportingDps(List<Player> team)
        {
            var dpses = team.OrderBy(p => p.Class)
                            .ThenBy(p => p.Name)
                            .Where(p => p.IsTeleporter)
                            .Where(p => p.Class != PlayerClass.Warlock)
                            .Where(p => p.Role == PlayerRole.MeleeDps || p.Role == PlayerRole.RangedDps)
                            .ToList();

            foreach (var dps in dpses)
            {
                foreach (var assignment in Assignments)
                {
                    if(assignment.Value.LeftPair.Catcher == null)
                    {
                        if (dps.CanTeleportUp(MonkStatueOnLeftSide))
                        {
                            assignment.Value.LeftPair.Catcher = dps;
                            break;
                        }                        
                    }
                    if(assignment.Value.RightPair.Catcher == null)
                    {
                        if (dps.CanTeleportUp(MonkStatueOnRightSide))
                        {
                            assignment.Value.RightPair.Catcher = dps;
                            break;
                        }
                    }
                }
            }
        }
        
        private void AssignRest(List<Player> team)
        {
            var unassignedPlayers = team.OrderBy(p => p.Name).Where(p => !IsAssigned(p));

            foreach (var player in unassignedPlayers)
            {
                foreach (var assignment in Assignments)
                {
                    var leftPair = assignment.Value.LeftPair;
                    if(leftPair.Catcher == null)
                    {
                        leftPair.Catcher = player;
                        break;
                    }
                    if(leftPair.Thrower == null)
                    {
                        leftPair.Thrower = player;
                        break;
                    }
                    var rightPair = assignment.Value.RightPair;
                    if (rightPair.Catcher == null)
                    {
                        rightPair.Catcher = player;
                        break;
                    }
                    if (rightPair.Thrower == null)
                    {
                        rightPair.Thrower = player;
                        break;
                    }

                }
            }
        }

        private bool IsAssigned(Player player)
        {
            foreach (var assignment in Assignments)
            {
                var leftPair = assignment.Value.LeftPair;
                if(leftPair.Catcher == player || leftPair.Thrower == player)
                {
                    return true;
                }

                var rightPair = assignment.Value.RightPair;
                if (rightPair.Catcher == player || rightPair.Thrower == player)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
