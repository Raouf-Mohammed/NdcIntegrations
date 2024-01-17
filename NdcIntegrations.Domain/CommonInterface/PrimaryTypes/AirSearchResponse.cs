using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NdcIntegrations.Core.CommonInterface
{
    public class AirSearchResponse
    {
        public Guid ResponseId { get; set; }
        public Dictionary<string, JourneyType> Journeys { get; set; }
        public Dictionary<string, SegmentType> FlightSegments { get; set; }
        public Dictionary<string, PriceClassType> PriceClasses { get; set; }
        public Dictionary<string, BaggageDetailsType> BaggageDetails { get; set; }
        public ICollection<OfferType> Offers { get; set; }
    }
}
