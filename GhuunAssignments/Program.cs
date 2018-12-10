using Sheets;
using System.Threading.Tasks;
using System;
using GameConcepts.Orbs;
using GameConcepts.Gateways;
using GameConcepts.Teleports;
using GameConcepts.Statues;
using GameConcepts.Interrupts;
using GameConcepts.BurstingBoilAreas;

namespace GhuunAssignments
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var team = await SpreadsheetService.GetTeam();
            
            var gatewayAssignment = GatewayAssignmentLogic.AssignGateways(team);
            await SpreadsheetService.WriteGatewayAssignments(gatewayAssignment);
            Console.WriteLine("Gateway assignments written");

            var orbLogic = new OrbAssignmentLogic(gatewayAssignment);
            var orbAssignmentList = orbLogic.AssignPlayers(team);
            await SpreadsheetService.WriteOrbAssignments(orbAssignmentList);
            Console.WriteLine("Orb assignments written");

            await SpreadsheetService.WriteOrbMacros(orbAssignmentList);
            Console.WriteLine("Macros written");
            
            var teleportAssignments = TeleportAssignmentLogic.GetTeleportAssignments(orbAssignmentList);
            await SpreadsheetService.WriteTeleportAssignments(teleportAssignments);
            Console.WriteLine("Teleport assignments written");
            
            var statueAssignments = StatueAssignmentLogic.GetStatueAssignment(orbAssignmentList);
            await SpreadsheetService.WriteStatueAssignments(statueAssignments);
            Console.WriteLine("Statue assignments written");

            var interruptAssignments = InterruptAssignmentLogic.AssignInterrupts(orbAssignmentList);
            await SpreadsheetService.WriteInterruptAssignments(interruptAssignments);
            Console.WriteLine("Interrupt assignments written");
            await SpreadsheetService.WriteTendrilAssignments(interruptAssignments);
            Console.WriteLine("Tendril assignments written");

            var burstingBoilAssignments = BurstingBoilAssignmentLogic.AssignBurstingBoilAreas(orbAssignmentList);
            await SpreadsheetService.WriteBurstingBoilAssignments(burstingBoilAssignments);
            Console.WriteLine("Bursting boil assignments written");

            Console.ReadKey();
        }
    }
}
