using Newtonsoft.Json;

namespace libSimba.Net.Models.Transaction
{
    /// <summary>
    ///     Transaction
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Transaction : Serializable
    {
        [JsonProperty("id")] public string ID { get; set; }

        [JsonProperty("submitted")] public bool Submitted { get; set; }

        [JsonProperty("payload")] public Payload Payload { get; set; }

        [JsonProperty("timestamp")] public string Timestamp { get; set; }

        [JsonProperty("smart_contract_id")] public string SmartContractID { get; set; }

        [JsonProperty("bytes_stored_on_blockchain")]
        public int BytesStoredOnBlockchain { get; set; }

        [JsonProperty("application_id")] public string ApplicationID { get; set; }

        [JsonProperty("organisation_id")] public string OrganisationID { get; set; }

        [JsonProperty("bundle_id")] public string BundleId { get; set; }

        [JsonProperty("bytes_stored_on_datastore")]
        public int BytesStoredOnDatastore { get; set; }

        [JsonProperty("adapter_id")] public string AdapterID { get; set; }

        [JsonProperty("data_store_id")] public string DataStoreID { get; set; }

        [JsonProperty("method_id")] public string MethodID { get; set; }

        [JsonProperty("parent_id")] public string ParentID { get; set; }

        [JsonProperty("is_asset")] public bool IsAsset { get; set; }

        [JsonProperty("user_id")] public int UserId { get; set; }

        [JsonProperty("receipt")] public Receipt Receipt { get; set; }

        [JsonProperty("group_id")] public string GroupID { get; set; }

        [JsonProperty("transaction_hash")] public string TransactionHash { get; set; }

        [JsonProperty("error")] public string Error { get; set; }

        [JsonProperty("error_details")] public ErrorDetails[] ErrorDetails { get; set; }
    }
}