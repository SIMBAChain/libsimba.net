using System.Collections.Generic;
using libSimba.Net.Models.Transaction;
using Newtonsoft.Json;

namespace libSimba.Net.Models.Swagger
{
    /// <summary>
    ///     ApplicationMetadata
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ApplicationMetadata : Serializable
    {
        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("network")] public string Network { get; set; }

        [JsonProperty("network_type")] public string NetworkType { get; set; }

        [JsonProperty("poa")] public bool POA { get; set; }

        [JsonProperty("simba_faucet")] public bool SimbaFaucet { get; set; }

        [JsonProperty("type")] public string Type { get; set; }

        [JsonProperty("faucet")] public string Faucet { get; set; }

        [JsonProperty("methods")] public Dictionary<string, Method> Methods { get; set; }
    }
}