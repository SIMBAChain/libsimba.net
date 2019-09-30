namespace libSimba.Net.Models.Transaction
{
    /// <summary>
    ///     Transaction Status
    /// </summary>
    public class TransactionStatus
    {
        public string Status { get; set; }
        public string TransactionHash { get; set; }
        public string Error { get; set; }
        public ErrorDetails[] ErrorDetails { get; set; }
    }
}