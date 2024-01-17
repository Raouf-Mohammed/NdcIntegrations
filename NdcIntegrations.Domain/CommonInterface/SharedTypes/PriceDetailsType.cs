namespace NdcIntegrations.Core.CommonInterface
{
    public class PriceDetailsType
    {
        public decimal TotalAmount { get; set; }
        public decimal TotalTaxAmount { get; set; }
        public decimal TotalBaseAmount { get; set; }
        public string Currency { get; set; }
        public ICollection<TaxType> TaxesAndFees { get; set; }
    }
}