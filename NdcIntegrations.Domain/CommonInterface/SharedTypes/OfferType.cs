namespace NdcIntegrations.Core.CommonInterface
{
    public class OfferType
    {
        public string OfferId { get; set; }
        public ICollection<string> OfferJourneys { get; set; }
        public ICollection<PassengerFareBreakdownType> PassengerFareBreakdown { get; set; }
        public PriceDetailsType PriceDetails { get; set; }
    }
}