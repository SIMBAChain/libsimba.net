using System.Numerics;
using libSimba.Net.Models.Transaction;
using Newtonsoft.Json;

namespace libSimba.Net.Models
{
    /// <summary>
    ///     Represents the response to a GetBalance call
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Balance : Serializable
    {
        [JsonProperty("amount")] public BigInteger Amount { get; set; }

        [JsonProperty("currency")] public string Currency { get; set; }

        public bool POA { get; set; }
    }
}