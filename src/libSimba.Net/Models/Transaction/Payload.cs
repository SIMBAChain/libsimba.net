using System.Collections.Generic;
using Newtonsoft.Json;

namespace libSimba.Net.Models.Transaction
{
    /// <summary>
    ///     Payload
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Payload
    {
        [JsonProperty("raw")] public RawPayload Raw { get; set; }

        [JsonProperty("inputs")] public Dictionary<string, string> Inputs { get; set; }

        [JsonProperty("method")] public string Method { get; set; }
    }
}