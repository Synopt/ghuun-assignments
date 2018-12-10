using GameConcepts.Orbs;
using System.Collections.Generic;

namespace GameConcepts.Gateways
{
    public class GatewayAssignment
    {
        public Dictionary<OrbSide, GatewaySet> Side { get; set; }

        public GatewayAssignment()
        {
            Side = new Dictionary<OrbSide, GatewaySet>
            {
                { OrbSide.Left, new GatewaySet() },
                { OrbSide.Right, new GatewaySet() }
            };
        }
    }
}
