namespace NdcIntegrations.Domain.CommonInterface
{
    public class PassengerFareBreakdownType
    {
        public string PassengerTypeCode { get; set; }
        public decimal PaxTotalTaxAmount { get; set; }
        public decimal PaxBaseAmount { get; set; }
        public string CurrencyCode { get; set; }
        public ICollection<TaxType> TaxesAndFees { get; set; }
        public ICollection<SegmentDetailsType> SegmentDetails { get; set; }
    }
}