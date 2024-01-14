namespace NdcIntegrations.Domain.CommonInterface
{
    public class SegmentType
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public string DepartureTerminal { get; set; }
        public string ArrivalTerminal { get; set; }
        public int FlightTime { get; set; }
        public string OperatingCarrierCode { get; set; }
        public string OperatingFlightNumber { get; set; }
        public string MarketingCarrierCode { get; set; }
        public string MarketingFlightNumber { get; set; }
        public string Equipment { get; set; }
    }
}