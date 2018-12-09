using GameConcepts.Players;
using System.Collections.Generic;

namespace GameConcepts.BurstingBoilAreas
{
    public class BurstingBoilAssignment
    {
        public Dictionary<BurstingBoilArea, List<Player>> Sides { get; set; }

        public BurstingBoilAssignment()
        {
            Sides = new Dictionary<BurstingBoilArea, List<Player>>();
        }
    }
}