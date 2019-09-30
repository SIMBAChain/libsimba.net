using System.Numerics;
using Newtonsoft.Json;

namespace libSimba.Net.Models.Transaction
{
    /// <summary>
    ///     Receipt
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Receipt
    {
        [JsonProperty("to")] public string To { get; set; }

        [JsonProperty("from")] public string From { get; set; }

        [JsonProperty("logs")] public string[] Logs { get; set; }

        [JsonProperty("status")] public bool Status { get; set; }

        [JsonProperty(PropertyName = "gasUsed")]
        public BigInteger GasUsed { get; set; }

        [JsonProperty("blockHash")] public string BlockHash { get; set; }

        [JsonProperty("logsBloom")] public string LogsBloom { get; set; }

        [JsonProperty(PropertyName = "blockNumber")]
        public BigInteger BlockNumber { get; set; }

        [JsonProperty("contractAddress")] public string ContractAddress { get; set; }

        [JsonProperty("transactionHash")] public string TransactionHash { get; set; }

        [JsonProperty(PropertyName = "transactionIndex")]
        public BigInteger TransactionIndex { get; set; }

        [JsonProperty(PropertyName = "cumulativeGasUsed")]
        public BigInteger CumulativeGasUsed { get; set; }
    }
}