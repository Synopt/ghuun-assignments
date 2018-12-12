using Sheets;
using System.Threading.Tasks;
using System;
using GameConcepts.Orbs;
using GameConcepts.Gateways;
using GameConcepts.Teleports;
using GameConcepts.Statues;
using GameConcepts.Interrupts;
using GameConcepts.BurstingBoilAreas;
using GameConcepts.PhaseThreeAreas;

namespace GhuunAssignments
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var team = await SpreadsheetService.GetTeam();

            var gatewayAssignment = GatewayAssignmentLogic.AssignGateways(team);
            await MainSheetService.WriteGatewayAssignments(gatewayAssignment);
            Console.WriteLine("Gateway assignments written");

            var orbLogic = new OrbAssignmentLogic(gatewayAssignment);
            var orbAssignmentList = orbLogic.AssignPlayers(team);
            await MainSheetService.WriteOrbAssignments(orbAssignmentList);
            Console.WriteLine("Orb assignments written");

            await MainSheetService.WriteOrbMacros(orbAssignmentList);
            Console.WriteLine("Macros written");
            
            var teleportAssignments = TeleportAssignmentLogic.GetTeleportAssignments(orbAssignmentList);
            await MainSheetService.WriteTeleportAssignments(teleportAssignments);
            Console.WriteLine("Teleport assignments written");
            
            var statueAssignments = StatueAssignmentLogic.GetStatueAssignment(orbAssignmentList);
            await MainSheetService.WriteStatueAssignments(statueAssignments);
            Console.WriteLine("Statue assignments written");

            var interruptAssignments = InterruptAssignmentLogic.AssignInterrupts(orbAssignmentList);
            await MainSheetService.WriteInterruptAssignments(interruptAssignments);
            Console.WriteLine("Interrupt assignments written");
            await MainSheetService.WriteTendrilAssignments(interruptAssignments);
            Console.WriteLine("Tendril assignments written");

            var burstingBoilAssignments = BurstingBoilAssignmentLogic.AssignBurstingBoilAreas(orbAssignmentList);
            await MainSheetService.WriteBurstingBoilAssignments(burstingBoilAssignments);
            Console.WriteLine("Bursting boil assignments written");

            var p3Assignments = PhaseThreeAreaAssignmentLogic.AssignPhaseThreeAreas(orbAssignmentList);
            await MainSheetService.WriteP3Assignments(p3Assignments);
            Console.WriteLine("Phase three assignments written");
            
            Console.ReadKey();
        }
    }
}
