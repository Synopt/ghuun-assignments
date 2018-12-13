using GameConcepts.Orbs;
using GameConcepts.Players;

namespace GameConcepts.Gateways
{
    public class PersonalGatewayAssignment
    {
        public GatewayPosition Position { get; set; }
        public OrbSide Side { get; set; }
        public Player Player { get; set; }
    }
}
