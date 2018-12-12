using GameConcepts.Players;
using System.Collections.Generic;

namespace GameConcepts.PhaseThreeAreas
{
    public class PhaseThreeAreaAssignment
    {
        public Dictionary<PhaseThreeArea, List<Player>> Areas { get; set; }

        public PhaseThreeAreaAssignment()
        {
            Areas = new Dictionary<PhaseThreeArea, List<Player>>
            {
                { PhaseThreeArea.NorthEast, new List<Player>() },
                { PhaseThreeArea.NorthWest, new List<Player>() },
                { PhaseThreeArea.South, new List<Player>() },
                { PhaseThreeArea.SouthEast, new List<Player>() },
                { PhaseThreeArea.SouthWest, new List<Player>() },
            };
        }
    }
}
