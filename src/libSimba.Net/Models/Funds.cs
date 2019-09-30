using Newtonsoft.Json;

namespace libSimba.Net.Models
{
    /// <summary>
    ///     Represents the response from the AddFunds request
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Funds
    {
        [JsonProperty("txnId")] public string TransactionID { get; set; }

        public bool POA { get; set; }
        public string FaucetUrl { get; set; } = "";
    }
}