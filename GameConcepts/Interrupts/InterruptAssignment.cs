using System.Collections.Generic;

namespace GameConcepts.Interrupts
{
    public class InterruptAssignment
    {
        public Dictionary<int, InterruptSet> Sets { get; set; }
        public InterruptAdd Tendrils { get; set; }

        public InterruptAssignment()
        {
            Sets = new Dictionary<int, InterruptSet>
            {
                { 1, new InterruptSet() },
                { 2, new InterruptSet() },
                { 3, new InterruptSet() },
                { 4, new InterruptSet() }
            };

            Tendrils = new InterruptAdd();
        }
    }
}
