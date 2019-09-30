using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using libSimba.Net.Exceptions;
using libSimba.Net.Simba;
using libSimba.Tests.Properties;
using Xunit;

namespace libSimba.Tests
{
    public class SimbaBaseTests
    {
        [Theory(DisplayName = "GetSimbaInstance behaves correctly in response to differing inputs.")]
        [InlineData("https://api.simbachain.com/v1-doesnt-exists/libSimba-SimbaChat-Quorum-doesnt-exist'/", true, false,
            typeof(SimbaChain))]
        [InlineData("https://api.simbachain.com/v1/libSimba-SimbaChat-Quorum/", false, false, typeof(SimbaChain))]
        [InlineData("https://scaas.example.com/", false, true, null)]
        public async Task GetSimbaInstanceReturnsCorrectType(string url, bool throws, bool isNull, Type reType)
        {
            var client = new HttpClient(new HttpMessageHandlerStub(async (request, cancellationToken) =>
            {
                if (request.RequestUri.AbsoluteUri ==
                    "https://api.simbachain.com/v1/libSimba-SimbaChat-Quorum/?format=openapi")
                {
                    var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(Resources.simbachat_swagger_json),
                        Headers =
                        {
                            {HttpResponseHeader.ContentType.ToString(), "application/json"}
                        }
                    };


                    return await Task.FromResult(responseMessage);
                }
                else
                {
                    var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent("Not Found")
                    };


                    return await Task.FromResult(responseMessage);
                }
            }));

            try
            {
                SimbaBase.Client = client;
                var simba = await SimbaBase.GetSimbaInstance(url);

                if (simba == null)
                {
                    Assert.True(isNull);
                }
                else
                {
                    Assert.False(isNull);
                    Assert.False(throws);
                    Assert.NotNull(simba);

                    if (reType == typeof(SimbaChain)) Assert.IsAssignableFrom<SimbaChain>(simba);
                }
            }
            catch (Exception e)
            {
                Assert.True(throws);
                Assert.IsAssignableFrom<MissingMetadataException>(e);
            }
        }

        [Fact(DisplayName = "GetSimbaInstance returns an instance of SimbaChain for a .com URL")]
        public async Task SimbaInstance()
        {
            var client = new HttpClient(new HttpMessageHandlerStub(async (request, cancellationToken) =>
            {
                if (request.RequestUri.AbsoluteUri ==
                    "https://api.simbachain.com/v1/libSimba-SimbaChat-Quorum/?format=openapi")
                {
                    var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(Resources.simbachat_swagger_json),
                        Headers =
                        {
                            {HttpResponseHeader.ContentType.ToString(), "application/json"}
                        }
                    };


                    return await Task.FromResult(responseMessage);
                }
                else
                {
                    var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent("Not Found")
                    };


                    return await Task.FromResult(responseMessage);
                }
            }));

            SimbaBase.Client = client;

            var simba = await SimbaBase.GetSimbaInstance("https://api.simbachain.com/v1/libSimba-SimbaChat-Quorum/");
            Assert.IsAssignableFrom<SimbaBase>(simba);
            Assert.IsAssignableFrom<SimbaChain>(simba);
        }
    }
}