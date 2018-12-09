using System.Collections.Generic;

namespace GameConcepts.Interrupts
{
    public class InterruptSet
    {
        public Dictionary<int, InterruptAdd> Adds { get; set; }

        public InterruptSet()
        {
            Adds = new Dictionary<int, InterruptAdd>
            {
                { 1, new InterruptAdd() },
                { 2, new InterruptAdd() },
                { 3, new InterruptAdd() },
                { 4, new InterruptAdd() },
                { 5, new InterruptAdd() }
            };
        }
    }
}