using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json;

namespace libSimba.Net.Models.Transaction
{
    /// <summary>
    ///     Raw Payload
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class RawPayload : TransactionInput
    {
    }
}