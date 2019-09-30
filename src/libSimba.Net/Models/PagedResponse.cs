using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using libSimba.Net.Exceptions;
using libSimba.Net.Models.Transaction;
using libSimba.Net.Simba;
using Newtonsoft.Json;

namespace libSimba.Net.Models
{
    /// <summary>
    ///     Encapsulates a paged response from the API
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class PagedResponse<T> : Serializable
    {
        private SimbaBase _simba;
        private string _url;

        protected internal PagedResponse(
            string url,
            SimbaBase simba)
        {
            _url = url;
            _simba = simba;
        }

        [JsonProperty("results")] private T[] Results { get; set; }

        [JsonProperty("count")] public int Count { get; set; }

        [JsonProperty("next")] private string NextUrl { get; set; }

        [JsonProperty("previous")] private string PreviousUrl { get; set; }

        public T[] Data()
        {
            return Results;
        }

        public int CurrentPage()
        {
            return int.Parse(
                HttpUtility.ParseQueryString(
                        new UriBuilder(_url).Query
                    )
                    .Get("page")
            );
        }

        public async Task<PagedResponse<T>> Next(CancellationToken ct = default)
        {
            if (NextUrl == null) return null;

            var auth = _simba.ApiAuthHeaders();

            var getNext = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(NextUrl),
                Headers =
                {
                    {HttpRequestHeader.ContentType.ToString(), "application/json"},
                    {auth.Key, auth.Value}
                }
            };


            try
            {
                return await _simba.DoHttp(getNext, new PagedResponse<T>(getNext.RequestUri.AbsolutePath, _simba), ct);
            }
            catch (HttpException ex)
            {
                throw new GetPagedResponseException(ex);
            }
        }

        public async Task<PagedResponse<T>> Previous(CancellationToken ct = default)
        {
            if (PreviousUrl == null) return null;

            var auth = _simba.ApiAuthHeaders();

            var getNext = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(PreviousUrl),
                Headers =
                {
                    {HttpRequestHeader.ContentType.ToString(), "application/json"},
                    {auth.Key, auth.Value}
                }
            };


            try
            {
                return await _simba.DoHttp(getNext, new PagedResponse<T>(getNext.RequestUri.AbsolutePath, _simba), ct);
            }
            catch (HttpException ex)
            {
                throw new GetPagedResponseException(ex);
            }
        }
    }
}