namespace Apex.RaspberryPiServer
{
    public class RaspberryPiSettings
    {
        public int RedLedPin { get; set; }
        public int BlueLedPin { get; set; }
        public int ReadingDelay { get; set; }
        public int LedDimmingSteps { get; set; }
        public int DimmingDelay { get; set; }
        public int FlashingDelay { get; set; }
        public int InfraredDistance { get; set; }
        public int ProximityTriggerPin { get; set; }
        public int ProximityEchoPin { get; set; }
        public int ProximityMaxDistance { get; set; }
        public int ProximityDistance { get; set; }
    }
}
