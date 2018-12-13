using GameConcepts.BurstingBoilAreas;
using GameConcepts.Gateways;
using GameConcepts.Interrupts;
using GameConcepts.Orbs;
using GameConcepts.PhaseThreeAreas;
using GameConcepts.Statues;
using GameConcepts.Teleports;
using System.Collections.Generic;

namespace GameConcepts.PersonalAssignments
{
    public class PersonalAssignmentInput
    {
        public List<OrbAssignment> Orbs { get; set; }
        public List<StatueAssignment> Statues { get; set; }
        public List<TeleportAssignment> Teleports { get; set; }
        public List<PersonalBurstingBoilAssignment> BurstingBoils { get; set; }
        public List<PersonalInterruptAssignment> Interrupts { get; set; }
        public List<PersonalTendrilAssignment> Tendrils { get; set; }
        public List<PersonalGatewayAssignment> Gateways { get; set; }
        public List<PersonalP3AreaAssignment> PhaseThreeAreas { get; set; }
    }
}
