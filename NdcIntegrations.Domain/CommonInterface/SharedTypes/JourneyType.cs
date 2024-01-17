namespace NdcIntegrations.Core.CommonInterface
{
    public class JourneyType
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public int NumberOfStops { get; set; }
        public ICollection<string> SegmentRefIds { get; set; }
    }
}