using GameConcepts.Players;

namespace GameConcepts.Interrupts
{
    public class PersonalInterruptAssignment
    {
        public int SetNumber { get; set; }
        public int AddNumber { get; set; }
        public int Order { get; set; }
        public Player Player { get; set; }
        public Player Partner { get; set; }
        public PersonalTendrilAssignment Tendril { get; set; }
    }
}
