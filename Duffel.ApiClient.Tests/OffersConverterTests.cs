using System.Collections.Generic;
using System.Linq;
using Duffel.ApiClient.Converters;
using Duffel.ApiClient.Interfaces.Models;
using Duffel.ApiClient.Interfaces.Models.Requests;
using Duffel.ApiClient.Interfaces.Models.Responses;
using NFluent;
using NUnit.Framework;

namespace Duffel.ApiClient.Tests
{
    public class OffersConverterTests
    {
        [Test]
        public void CanSerializeOffersRequest()
        {
            var request = new OffersRequest
            {
                Passengers = new List<Passenger>
                {
                    new Passenger {PassengerType = PassengerType.Adult},
                    new Passenger {PassengerType = PassengerType.Child}
                },
                RequestedSources = new List<string> { "united" },
                Slices = new List<Interfaces.Models.Requests.Slice>
                {
                    new Interfaces.Models.Requests.Slice
                    {
                        Origin = "SFO",
                        Destination = "LAX",
                        DepartureDate = "2020-01-01"
                    }
                }
            };

            var result = OffersConverter.Serialize(request);
            Check.That(result).Equals("{\"data\":{\"passengers\":[{\"type\":\"adult\"},{\"type\":\"child\"}],\"slices\":[{\"origin\":\"SFO\",\"destination\":\"LAX\",\"departure_date\":\"2020-01-01\"}],\"requested_sources\":[\"united\"]}}");
        }
        
        [Test]
        public void CanDeserializeOffersResponse()
        {
            var offersResponse = OffersConverter.Deserialize(JsonFixture.Load("offers_response_full_ow_sfo_jfk.json"));
            Check.That(offersResponse).IsNotNull().And.IsInstanceOf<OffersResponse>();
            
            AssertSlicesDataCorrect(offersResponse.Slices!.ToList());
            AssertOffersDataCorrect(offersResponse.Offers!.ToList());
        }

        private static void AssertOffersDataCorrect(List<Offer> offers)
        {
            Check.That(offers).HasSize(4);
            var offer = offers.First();
            
            Check.That(offer.Id).IsEqualTo("off_0000AFANuyPmZYc2j0aMj7");
            Check.That(offer.LiveMode).IsFalse();
            Check.That(offer.BaseCurrency).Equals("GBP");
            Check.That(offer.BaseAmount).Equals("431.66");
            
            Check.That(offer.Slices).HasSize(1);
            var slice = offer.Slices.First();
            
            Check.That(slice.FareBrandName).IsEqualTo("Refundable Main Cabin");
            Check.That(slice.Duration).IsEqualTo("PT9H26M");
            Check.That(slice.Id).IsEqualTo("sli_0000AFANuyQ8YEtck6keHI");
            Check.That(slice.Origin).IsInstanceOf<Airport>().And.IsNotNull();
            Check.That(slice.Destination).IsInstanceOf<Airport>().And.IsNotNull();
            
            // TODO: add segments
        }

        private static void AssertSlicesDataCorrect(List<Interfaces.Models.Responses.Slice> slices)
        {
            Check.That(slices).HasSize(1);

            var origin = slices.First().Origin;
            var destination = slices.First().Destination;

            Check.That(origin).IsInstanceOf<Airport>();
            Check.That(destination).IsInstanceOf<Airport>();
            
            Check.That(slices.First().DepartureDate).IsEqualTo("2022-02-05");
        }
    }
}