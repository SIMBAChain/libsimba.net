using System.Collections.Generic;
using Newtonsoft.Json;

namespace libSimba.Net.Models.Swagger
{
    /// <summary>
    ///     Method
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Method
    {
        [JsonProperty("parameters")] public Dictionary<string, Parameter> Parameters { get; set; }
    }
}