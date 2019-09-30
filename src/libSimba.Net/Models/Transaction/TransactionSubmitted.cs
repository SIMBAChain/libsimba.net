using Newtonsoft.Json;

namespace libSimba.Net.Models.Transaction
{
    /// <summary>
    ///     Transaction Submitted
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class TransactionSubmitted : Serializable
    {
        [JsonProperty("status")] public string Status { get; set; }
    }
}