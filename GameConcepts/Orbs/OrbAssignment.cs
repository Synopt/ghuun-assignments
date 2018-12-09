﻿using GameConcepts.Players;

namespace GameConcepts.Orbs
{
    public class OrbAssignment
    {
        public Player Player { get; set; }
        public int Set { get; set; }
        public OrbSide Side { get; set; }
        public OrbRole Role { get; set; }
    }
}