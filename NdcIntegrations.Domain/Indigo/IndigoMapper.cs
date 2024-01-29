using AirService;
using Microsoft.Extensions.Configuration;
using NdcIntegrations.Core.CommonInterface;
using NdcIntegrations.Core.Indigo;
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
        private static IConfigurationRoot IndigoConfiguration = new ConfigurationBuilder()
                                .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "IndigoConfig.json"), optional: false, reloadOnChange: true)
                                .Build();
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
            airPriceSolution = airPriceSolution.Where(x => x.CompleteItinerary = true).ToArray();
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
                    OfferJourneys = new List<string>(),
                    PassengerFareBreakdown = new List<PassengerFareBreakdownType>() ,
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
                            PriceClassRefId=fareInfoList.FirstOrDefault(y=>y.Key == x.FareInfoRef).Brand.BrandID,
                            CabinCode =x.CabinClass,
                            RBD=x.BookingCode,
                            BaggageDetailsRefId="",

                        }).ToList()
                       
                    };
                    offer.PassengerFareBreakdown.Add(passengerFareBreakdown);
                }
                airSearchResponse.Offers.Add(offer);
            }
            
            return airSearchResponse;
         }
        public static AirService.LowFareSearchReq? IndigoMapperSearchRequest(AirSearchRequest airSearchRequest)
        {
            if(airSearchRequest == null) return null;
            AirService.LowFareSearchReq lowFareSearchReq = new AirService.LowFareSearchReq();
            lowFareSearchReq.TraceId = IndigoConfiguration["TraceId"];
            lowFareSearchReq.TargetBranch = IndigoConfiguration["TargetBranch"];
            lowFareSearchReq.AuthorizedBy = IndigoConfiguration["AuthorizedBy"];
            lowFareSearchReq.SolutionResult = IndigoConfiguration["SolutionResult"] == "true" ? true:false;
            lowFareSearchReq.ReturnUpsellFare = IndigoConfiguration["ReturnUpsellFare"] =="true" ? true:false;
            lowFareSearchReq.BillingPointOfSaleInfo.OriginApplication = IndigoConfiguration["OriginApplication"];
                               
            lowFareSearchReq.Items = new SearchAirLeg[] { };
            for(int i=0; i< airSearchRequest.SearchCriteria.Count; i++)
            {
                var searchLeg = new SearchAirLeg()
                {
                    SearchOrigin = new typeSearchLocation[]
                    {
                        new typeSearchLocation
                        {
                            Item = new CityOrAirport
                            {
                                Code = airSearchRequest.SearchCriteria.ToList()[i].Origin,
                                PreferCity = true,
                            }
                        }
                    },
                    SearchDestination = new typeSearchLocation[]
                    {
                        new typeSearchLocation
                        {
                            Item = new CityOrAirport
                            {
                                Code = airSearchRequest.SearchCriteria.ToList()[i].Destination,
                                PreferCity = true,
                            }
                        }
                    },
                    Items = new typeTimeSpec[]
                    {
                        new typeTimeSpec
                        {
                             PreferredTime = airSearchRequest.SearchCriteria.ToList()[i].Date.ToString("yyyy-MM-dd")
                        }
                    }
                };
                lowFareSearchReq.Items = lowFareSearchReq.Items.Append(searchLeg).ToArray();
            }
            lowFareSearchReq.AirSearchModifiers.PreferredProviders[0].Code = "ACH";
            lowFareSearchReq.SearchPassenger = new SearchPassenger[] { };
            for (int i = 0; i < airSearchRequest.Passengers.Count; i++) 
            {
                SearchPassenger searchPassenger = new SearchPassenger
                {
                    Code = airSearchRequest.Passengers.ToList()[i].PassengerTypeCode,
                };
                lowFareSearchReq.SearchPassenger = lowFareSearchReq.SearchPassenger.Append(searchPassenger).ToArray();
            }

            return lowFareSearchReq;
        }
        public static List<  Dictionary<string, FareConfirmData>> GetFareCofirnmCachingData(AirService.LowFareSearchRsp lowFareSearchRsp)
        {
            var fareConfirmCachingData = new List<Dictionary<string,FareConfirmData>>();
            var airPriceSolution = Array.ConvertAll(lowFareSearchRsp.Items, prop => (AirService.AirPricingSolution)prop);
            airPriceSolution = airPriceSolution.Where(x => x.CompleteItinerary = true).ToArray();
            foreach (var solution in airPriceSolution)
            {
                var solutionCachingData = new Dictionary<string, FareConfirmData>();
                var fareConfirmData = new FareConfirmData ();
                fareConfirmData.SegmentsData = new List<SegmentData>();
                foreach (var bookingInfo in solution.AirPricingInfo[0].BookingInfo)
                {
                    var segment = lowFareSearchRsp.AirSegmentList.FirstOrDefault(x => x.Key == bookingInfo.SegmentRef);

                    var segmentData = new SegmentData
                    {
                        AirSegmentKey = bookingInfo.SegmentRef,
                        Group = segment.Group,
                        Carrier = segment.Carrier,
                        FlightNumber = segment.FlightNumber,
                        Origin = segment.Origin,
                        Destination = segment.Destination,
                        DepartureTime = segment.DepartureTime,
                        ArrivalTime = segment.ArrivalTime,
                        ProviderCode = segment.AirAvailInfo[0].ProviderCode,
                        HostTokenRef = bookingInfo.HostTokenRef,
                        HostTokenValue=lowFareSearchRsp.HostTokenList.FirstOrDefault(x=>x.Key== bookingInfo.HostTokenRef).Value,
                        FareBasisCode =lowFareSearchRsp.FareInfoList.FirstOrDefault(c => c.Key == bookingInfo.FareInfoRef).FareBasis   
                    };
                    fareConfirmData.SegmentsData.Add(segmentData);
                }
                fareConfirmData.Passanager = new Dictionary<string,int>();

                foreach(var airPricingInfo in solution.AirPricingInfo)
                {
                    foreach (var passangerType in airPricingInfo.PassengerType)
                    {
                        if (fareConfirmData.Passanager.ContainsKey(passangerType.Code) == true)
                        {
                            fareConfirmData.Passanager[passangerType.Code]++;
                        }
                        else
                        {
                            fareConfirmData.Passanager.TryAdd(passangerType.Code, 1);

                        }

                    }
                }
                solutionCachingData.TryAdd(solution.Key, fareConfirmData);
                fareConfirmCachingData.Add(solutionCachingData);   
            }
            return fareConfirmCachingData;
        }
    }   
}
