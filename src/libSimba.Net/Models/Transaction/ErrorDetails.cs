using Newtonsoft.Json;

namespace libSimba.Net.Models.Transaction
{
    /// <summary>
    ///     Error Details
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ErrorDetails
    {
        [JsonProperty("Message")] public string Message { get; set; }

        [JsonProperty("name")] public string Name { get; set; }
    }
}