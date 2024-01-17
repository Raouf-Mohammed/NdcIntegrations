using System.ComponentModel.DataAnnotations;

namespace NdcIntegrations.Core.CommonInterface
{
    public class SearchCriterionType
    {
        [Newtonsoft.Json.JsonProperty("origin", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        [MaxLength(3)]
        public string Origin { get; set; }

        [Newtonsoft.Json.JsonProperty("destination", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        [MaxLength(3)]
        public string Destination { get; set; }

        public DateTime Date { get; set; }
    }
}