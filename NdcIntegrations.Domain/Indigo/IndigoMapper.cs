using NdcIntegrations.Domain.CommonInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NdcIntegrations.Domain.Indigo
{
    public class IndigoMapper
    {
		 public static AirSearchResponse? IndigoMapperSearchResponse(AirService.LowFareSearchRsp lowFareSearchRsp)
         {
            if (lowFareSearchRsp == null) return null;
            AirSearchResponse airSearchResponse = new AirSearchResponse();
            airSearchResponse.Journeys = new Dictionary<string, JourneyType>();
            airSearchResponse.FlightSegments = new Dictionary<string, SegmentType>();
            airSearchResponse.PriceClasses = new Dictionary<string, PriceClassType>();
            airSearchResponse.Offers = new List<OfferType>();
            var flightDetails = lowFareSearchRsp.FlightDetailsList;
            var fareInfoList = lowFareSearchRsp.FareInfoList;
            var routeList = lowFareSearchRsp.RouteList[0].Leg;
            var airPriceSolution = Array.ConvertAll(lowFareSearchRsp.Items, prop => (AirService.AirPricingSolution)prop);
            foreach (var segment in lowFareSearchRsp.AirSegmentList)
            {
                var segmentType = new SegmentType()
                {
                    Origin = segment.Origin,
                    Destination = segment.Destination,
                    DepartureDateTime =Convert.ToDateTime(segment.DepartureTime),
                    ArrivalDateTime = Convert.ToDateTime(segment.ArrivalTime),
                    DepartureTerminal = flightDetails.FirstOrDefault(f => f.Key == segment.FlightDetailsRef[0].Key).OriginTerminal,
                    ArrivalTerminal = flightDetails.FirstOrDefault(f => f.Key == segment.FlightDetailsRef[0].Key).DestinationTerminal,
                    FlightTime = int.Parse(segment.FlightTime),
                    OperatingFlightNumber=segment.FlightNumber,
                    OperatingCarrierCode=segment.Carrier,
                    MarketingFlightNumber=segment.FlightNumber,
                    MarketingCarrierCode=segment.Carrier,
                    Equipment=segment.Equipment,  
                };
                airSearchResponse.FlightSegments.TryAdd(segment.Key,segmentType);
            }
            foreach (var brand in lowFareSearchRsp.BrandList)
            {
                var priceClass = new PriceClassType()
                {
                    PriceClassName = brand.Name,
                    FareDescription = "public fare",
                    RulesAndPenalties = brand.Text.Select(x => x.Value).ToList(),

                };
                airSearchResponse.PriceClasses.TryAdd(brand.BrandID, priceClass);
            }
            foreach (var solution in airPriceSolution)
            {
                var offer = new OfferType()
                {
                    OfferId = solution.Key,
                    PriceDetails = new PriceDetailsType()
                    {
                        TotalAmount = decimal.Parse(solution.TotalPrice.Skip(3).ToString()),
                        TotalTaxAmount = decimal.Parse(solution.Taxes.Skip(3).ToString()),
                        TotalBaseAmount = decimal.Parse(solution.BasePrice.Skip(3).ToString()),
                        Currency = solution.TotalPrice.Take(3).ToString(),
                        TaxesAndFees = solution.TaxInfo.Select(x => new TaxType()
                        {
                            Code = x.Category,
                            Amount = decimal.Parse(x.Amount.Skip(3).ToString()),
                        }).ToList()

                    },
                    PassengerFareBreakdown = new List<PassengerFareBreakdownType>() ,
                    OfferJourneys = new List<string>(),
                   
                };
                for (int i = 0; i < solution.Journey.Length; i++) 
                {
                    var journeyType = new JourneyType()
                    {
                        Origin = routeList.FirstOrDefault(x => x.Key == solution.LegRef[i].Key).Origin,
                        Destination = routeList.FirstOrDefault(x => x.Key == solution.LegRef[i].Key).Destination,
                        NumberOfStops = solution.Journey[i].AirSegmentRef.Length - 1,
                        SegmentRefIds = solution.Journey[i].AirSegmentRef.Select(x => x.Key).ToList(),
                    };
                    var joureyId = string.Join("?", journeyType.SegmentRefIds);

                    airSearchResponse.Journeys.TryAdd(joureyId, journeyType);
                    offer.OfferJourneys.Add(joureyId);
                }
                foreach (var airPricingInfo in solution.AirPricingInfo)
                {
                    var passengerFareBreakdown = new PassengerFareBreakdownType()
                    {
                        PassengerTypeCode = airPricingInfo.PassengerType[0].Code,
                        PaxTotalTaxAmount = decimal.Parse(airPricingInfo.Taxes.Skip(3).ToString()),
                        PaxBaseAmount = decimal.Parse(airPricingInfo.BasePrice.Skip(3).ToString()),
                        CurrencyCode = airPricingInfo.Taxes.Take(3).ToString(),
                        TaxesAndFees = airPricingInfo.TaxInfo.Select(x => new TaxType()
                        {
                            Code = x.Category,
                            Amount = decimal.Parse(x.Amount.Skip(3).ToString()),
                        }).ToList(),
                        SegmentDetails = airPricingInfo.BookingInfo.Select(x => new SegmentDetailsType()
                        {
                            SegmentRefId=x.SegmentRef,
                            CabinCode =x.CabinClass,
                            RBD=x.BookingCode,
                            PriceClassRefId=fareInfoList.FirstOrDefault(y=>y.Key ==x.FareInfoRef).Brand.BrandID,
                            BaggageDetailsRefId="",

                        }).ToList()
                       
                    };
                    offer.PassengerFareBreakdown.Add(passengerFareBreakdown);
                }
                airSearchResponse.Offers.Add(offer);
            }
            
            return airSearchResponse;
         }
	}
}
