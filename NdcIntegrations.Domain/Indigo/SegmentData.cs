namespace NdcIntegrations.Domain.Indigo
{
    public class SegmentData
    {
         public string AirSegmentKey { get; set; }
         public int Group { get; set; }
         public string Carrier { get; set; }
         public string FlightNumber { get; set; }
         public string Origin { get; set; }
         public string Destination { get; set; }
         public string DepartureTime { get; set; }
         public string ArrivalTime { get; set; }
         public string ProviderCode { get; set; }
         public string HostTokenRef { get; set; }
         public string HostTokenValue { get; set; }
        public string FareBasisCode { get; set; }

    }
}