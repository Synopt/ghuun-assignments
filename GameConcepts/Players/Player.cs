namespace GameConcepts.Players
{
    public class Player
    {
        public string Name { get; set; }
        public PlayerRole Role { get; set; }
        public PlayerClass Class { get;set; }
        public string WhisperName { get; set; }
        public string Server { get; set; }

        public override string ToString() => $"{Name} - {Class} - {Role}";

        public bool IsTeleporter => Class == PlayerClass.Monk || Class == PlayerClass.Warlock || Class == PlayerClass.Druid;
        public bool HasAoeStun => Class == PlayerClass.DemonHunter || Class == PlayerClass.Monk || Class == PlayerClass.Warlock;

        internal bool CanTeleportUp(bool monkStatueInPlace)
        {
            if(Class != PlayerClass.Druid) { return true; }
            return monkStatueInPlace;
        }

        public string FullyQualifiedName => $"{WhisperName}-{Server}";

        public int Mobility
        {
            get
            {
                switch (Class)
                {
                    case PlayerClass.DemonHunter:
                    case PlayerClass.Mage:
                    case PlayerClass.Monk:
                    case PlayerClass.Paladin:
                    case PlayerClass.Warrior:
                        return 0;
                    case PlayerClass.DeathKnight:
                    case PlayerClass.Druid:
                    case PlayerClass.Hunter:
                    case PlayerClass.Rogue:
                    case PlayerClass.Shaman:
                        return 1;
                    case PlayerClass.Priest:
                    case PlayerClass.Warlock:
                        return 2;
                    default:
                        return 2;
                }
            }
        }
    }
}
