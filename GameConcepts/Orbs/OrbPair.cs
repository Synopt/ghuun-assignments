using GameConcepts.Players;
using System.Collections.Generic;

namespace GameConcepts.Orbs
{
    public class OrbPair
    {
        public Dictionary<OrbRole, Player> Positions { get; set; } = new Dictionary<OrbRole, Player>();

        public OrbPair()
        {
            Positions.Add(OrbRole.Catcher, null);
            Positions.Add(OrbRole.Thrower, null);
        }

        public Player Catcher
        {
            get
            {
                return Positions[OrbRole.Catcher];
            }
            set
            {
                Positions[OrbRole.Catcher] = value;
            }
        }

        public Player Thrower
        {
            get
            {
                return Positions[OrbRole.Thrower];
            }
            set
            {
                Positions[OrbRole.Thrower] = value;
            }
        }

        public Player this[OrbRole index]
        {
            get
            {
                return Positions[index];
            }
            set
            {
                Positions[index] = value;
            }
        }
    }
}