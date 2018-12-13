using GameConcepts.BurstingBoilAreas;
using GameConcepts.Gateways;
using GameConcepts.Interrupts;
using GameConcepts.Orbs;
using GameConcepts.PhaseThreeAreas;
using GameConcepts.Players;
using GameConcepts.Statues;
using GameConcepts.Teleports;
using System.Collections.Generic;

namespace GameConcepts.PersonalAssignments
{
    public class PersonalAssignment
    {
        public Player Player { get; set; }
        public OrbAssignment Orb { get; set; }
        public List<PersonalInterruptAssignment> Interrupts { get; set; }
        public PersonalTendrilAssignment Tendril { get; set; }
        public PersonalGatewayAssignment Gateway { get; set; }
        public StatueAssignment Statue { get; set; }
        public TeleportAssignment Teleport { get; set; }
        public PersonalBurstingBoilAssignment BurstingBoil {get;set;}
        public PersonalP3AreaAssignment PhaseThree { get; set; }
    }
}
