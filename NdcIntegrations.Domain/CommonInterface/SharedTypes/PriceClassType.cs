namespace NdcIntegrations.Domain.CommonInterface
{
    public class PriceClassType
    {
        public string PriceClassName { get; set; }
        public string FareDescription { get; set; }
        public ICollection<string> RulesAndPenalties { get; set; }
    }
}