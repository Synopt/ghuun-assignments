using GameConcepts.Players;
using System.Collections.Generic;

namespace GameConcepts.BurstingBoilAreas
{
    public class BurstingBoilAssignment
    {
        public Dictionary<BurstingBoilArea, List<Player>> Sides { get; set; }

        public BurstingBoilAssignment()
        {
            Sides = new Dictionary<BurstingBoilArea, List<Player>>
            {
                { BurstingBoilArea.Diamond, new List<Player>() },
                { BurstingBoilArea.Moon, new List<Player>() },
                { BurstingBoilArea.Star, new List<Player>() }
            };
        }
    }
}