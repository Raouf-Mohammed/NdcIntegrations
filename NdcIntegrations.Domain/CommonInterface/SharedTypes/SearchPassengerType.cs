using System.ComponentModel.DataAnnotations;

namespace NdcIntegrations.Domain.CommonInterface
{
    public class SearchPassengerType
    {
        [Newtonsoft.Json.JsonProperty("passengerTypeCode", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        [StringLength(3,MinimumLength = 3, ErrorMessage = "Passenger type code must have a length of 3")]
        public string PassengerTypeCode { get; set; }

        [Newtonsoft.Json.JsonProperty("count", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        [Range(1,9,ErrorMessage = "Passenger count must be between 1 and 9")]
        public int Count { get; set; }
    }
}