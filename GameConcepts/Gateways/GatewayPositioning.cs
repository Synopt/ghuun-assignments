using GameConcepts.Players;
using System.Collections.Generic;

namespace GameConcepts.Gateways
{
    public class GatewaySet
    {
        public Dictionary<GatewayPosition, Player> Positioning { get; set; }
    }
}
