using System.Collections.Generic;

namespace GameConcepts.Orbs
{
    public class OrbSet
    {
        public Dictionary<OrbSide, OrbPair> Pairs { get; set; } = new Dictionary<OrbSide, OrbPair>();

        public OrbSet()
        {
            Pairs.Add(OrbSide.Left, new OrbPair());
            Pairs.Add(OrbSide.Right, new OrbPair());
        }

        public OrbPair LeftPair
        {
            get
            {
                return Pairs[OrbSide.Left];
            }
            set
            {
                Pairs[OrbSide.Left] = value;
            }
        }

        public OrbPair RightPair
        {
            get
            {
                return Pairs[OrbSide.Right];
            }
            set
            {
                Pairs[OrbSide.Right] = value;
            }
        }

        public OrbPair this[OrbSide index]
        {
            get
            {
                return Pairs[index];
            }
            set
            {
                Pairs[index] = value;
            }
        }
    }
}
