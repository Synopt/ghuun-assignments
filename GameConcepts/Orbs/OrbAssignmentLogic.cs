﻿using System.Linq;
using System.Collections.Generic;
using GameConcepts.Players;
using GameConcepts.Gateways;

namespace GameConcepts.Orbs
{
    public class OrbAssignmentLogic
    {
        private Dictionary<int, OrbSet> Assignments { get; set; }
        private GatewayAssignment GatewayAssignment { get; set; }
        private bool MonkStatueOnRightSide = false;
        private bool MonkStatueOnLeftSide = false;

        public OrbAssignmentLogic(GatewayAssignment gatewayAssignment)
        {
            Assignments = new Dictionary<int, OrbSet>
            {
                {1, new OrbSet() },
                {2, new OrbSet() },
                {3, new OrbSet() },
                {4, new OrbSet() },
                {5, new OrbSet() },
            };

            GatewayAssignment = gatewayAssignment;
        }

        public List<OrbAssignment> AssignPlayers(List<Player> team)
        {
            AssignTanks(team);
            AssignHealers(team);
            AssignGatewayLocks(team);
            AssignTeleportingDps(team);

            var assignmentsList = ListAssignments();
            AssignRest(team, assignmentsList);

            return assignmentsList;
        }

        private List<OrbAssignment> ListAssignments()
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
                if (set == default(int)) { break; }

                var leftSide = set % 2 == 0;
                var pair = leftSide ? Assignments[set].LeftPair : Assignments[set].RightPair;
                if (tank.IsTeleporter) { pair.Catcher = tank; }

                else { pair.Thrower = tank; }

                setsRemaining.RemoveAt(0);
            }
        }

        private void AssignHealers(List<Player> team)
        {
            var healers = team.OrderBy(p => p.Mobility).ThenBy(p => p.Name).Where(p => p.Role == PlayerRole.Healer);
            var setsRemaining = new List<int> { 1, 2, 3, 4, 5 };

            foreach (var healer in healers)
            {
                var set = setsRemaining.OrderBy(s => HardestSets(s)).FirstOrDefault();
                if (set == default(int)) { break; }

                var leftSide = set % 2 == 1;
                var pair = leftSide ? Assignments[set].LeftPair : Assignments[set].RightPair;
                if (healer.IsTeleporter) { pair.Catcher = healer; }
                else { pair.Thrower = healer; }

                if (healer.Class == PlayerClass.Monk)
                {
                    if (leftSide) { MonkStatueOnLeftSide = true; }
                    else { MonkStatueOnRightSide = true; }
                }

                setsRemaining.Remove(set);
            }
        }

        private int HardestSets(int s)
        {
            var gateOnCloseLeft = GatewayAssignment.Side[OrbSide.Left].Position[GatewayPosition.Close] != null;
            var gateOnCloseRight = GatewayAssignment.Side[OrbSide.Right].Position[GatewayPosition.Close] != null;

            if (IsLeftSide(s))
            {
                return gateOnCloseLeft ? 1 : 0;
            }
            else
            {
                return gateOnCloseRight ? 1 : 0;
            }
        }

        private object HardestRuns(OrbAssignment assignment)
        {
            var gateOnCloseLeft = GatewayAssignment.Side[OrbSide.Left].Position[GatewayPosition.Close] != null;
            var gateOnCloseRight = GatewayAssignment.Side[OrbSide.Right].Position[GatewayPosition.Close] != null;

            if(assignment.Side == OrbSide.Left)
            {
                return gateOnCloseLeft ? 1 : 0;
            }
            else
            {
                return gateOnCloseRight ? 1 : 0;
            }
        }

        private bool IsLeftSide(int s) => s % 2 == 1;

        private void AssignGatewayLocks(List<Player> team)
        {
            foreach (var gatewaySide in GatewayAssignment.Side)
            {
                foreach (var gatewayPosition in gatewaySide.Value.Position)
                {
                    if (gatewaySide.Key == OrbSide.Left)
                    {
                        foreach (var assignment in Assignments)
                        {
                            if (assignment.Value.LeftPair.Catcher == null)
                            {
                                assignment.Value.LeftPair.Catcher = gatewayPosition.Value;
                                break;
                            }
                        }
                    }
                    else
                    {
                        foreach (var assignment in Assignments)
                        {
                            if (assignment.Value.RightPair.Catcher == null)
                            {
                                assignment.Value.RightPair.Catcher = gatewayPosition.Value;
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void AssignTeleportingDps(List<Player> team)
        {
            var dpses = team.OrderBy(p => p.Class)
                            .ThenBy(p => p.Name)
                            .Where(p => p.IsTeleporter)
                            .Where(p => !IsAssigned(p))
                            .ToList();

            foreach (var dps in dpses)
            {
                foreach (var assignment in Assignments)
                {
                    if (assignment.Value.LeftPair.Catcher == null)
                    {
                        if (dps.CanTeleportUp(MonkStatueOnLeftSide))
                        {
                            assignment.Value.LeftPair.Catcher = dps;
                            break;
                        }
                    }
                    if (assignment.Value.RightPair.Catcher == null)
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

        private void AssignRest(List<Player> team, List<OrbAssignment> assignmentsList)
        {
            var unassignedPlayers = team.OrderBy(p => p.Mobility).ThenBy(p => p.Name).Where(p => !IsAssigned(p));
            var unassignedOrbs = assignmentsList.Where(a => a.Player == null).OrderBy(a => HardestRuns(a)).ToList();
            
            foreach (var player in unassignedPlayers)
            {
                var assignment = unassignedOrbs.First();
                assignment.Player = player;
                unassignedOrbs.Remove(assignment);
            }
        }

        private bool IsAssigned(Player player)
        {
            foreach (var assignment in Assignments)
            {
                var leftPair = assignment.Value.LeftPair;
                if (leftPair.Catcher == player || leftPair.Thrower == player)
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
