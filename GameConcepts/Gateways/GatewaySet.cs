using GameConcepts.Players;
using System.Collections.Generic;

namespace GameConcepts.Gateways
{
    public class GatewaySet
    {
        public Dictionary<GatewayPosition, Player> Position { get; set; }

        public GatewaySet()
        {
            Position = new Dictionary<GatewayPosition, Player>
            {
                { GatewayPosition.Close, null },
                { GatewayPosition.Far, null }
            };
        }
    }
}
