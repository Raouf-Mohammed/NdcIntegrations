using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NdcIntegrations.Domain.CommonInterface
{
    public class AirSearchRequest
    {
        [Newtonsoft.Json.JsonProperty("searchCriteria", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public ICollection<SearchCriterionType> SearchCriteria { get; set; }

        [Newtonsoft.Json.JsonProperty("passengers", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public ICollection<SearchPassengerType> Passengers { get; set; }
    }
}
