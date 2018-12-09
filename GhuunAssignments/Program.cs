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

            var orbLogic = new OrbAssignmentLogic();
            var orbAssignments = orbLogic.AssignPlayers(team);
            await SpreadsheetService.WriteOrbAssignments(orbAssignments);
            Console.WriteLine("Orb assignments written");

            var orbAssignmentList = orbLogic.ListAssignments();

            await SpreadsheetService.WriteOrbMacros(orbAssignmentList);
            Console.WriteLine("Macros written");

            var gatewayLogic = new GatewayAssignmentLogic();
            var gatewayAssignment = gatewayLogic.AssignPlayers(orbAssignmentList);
            await SpreadsheetService.WriteGatewayAssignments(gatewayAssignment);
            Console.WriteLine("Gateway assignments written");

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
