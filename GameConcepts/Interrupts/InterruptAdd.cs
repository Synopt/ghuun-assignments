using GameConcepts.Players;
using System.Collections.Generic;

namespace GameConcepts.Interrupts
{
    public class InterruptAdd
    {
        public Dictionary<int, Player> Interrupts { get; set; }

        public InterruptAdd()
        {
            Interrupts = new Dictionary<int, Player>
            {
                { 1, null },
                { 2, null }
            };
        }
    }
}