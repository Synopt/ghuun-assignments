using GameConcepts.Players;

namespace GameConcepts.Orbs
{
    public class OrbAssignment
    {
        public Player Player { get; set; }
        public int Set { get; set; }
        public OrbSide Side { get; set; }
        public OrbRole Role { get; set; }
        public string Macro { get; set; }
        public Player Whisperer { get; set; }
        public Player Parner { get; set; }
    }
}