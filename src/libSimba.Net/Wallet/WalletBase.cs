using System.Threading.Tasks;
using libSimba.Net.Models.Transaction;

namespace libSimba.Net.Wallet
{
    /// <summary>
    ///     Base class for libsimba WalletBase implementations
    /// </summary>
    public abstract class WalletBase
    {
        private readonly ISigningConfirmation SigningConfirmation;

        /// <summary>
        ///     Base class for libsimba WalletBase implementations
        /// </summary>
        /// <param Name="signingConfirmation">
        ///     an optional callback for requesting user permission to sign a transaction.
        ///     Should resolve a promise with true for accept, and false (or reject) for reject.
        /// </param>
        /// <param name="signingConfirmation"></param>
        protected WalletBase(ISigningConfirmation signingConfirmation = null)
        {
            SigningConfirmation = signingConfirmation;
        }

        protected async Task<bool> RequestConfirmation(RawPayload p)
        {
            return SigningConfirmation == null || await SigningConfirmation.RequestSigningConfirmation(p);
        }

        /// <summary>
        ///     Unlock the wallet
        /// </summary>
        /// <param name="passKey">The pass key to unlock the wallet</param>
        public abstract Task UnlockWallet(string passKey);

        /// <summary>
        ///     Generate a wallet
        /// </summary>
        /// <param name="passKey">The pass key to lock the wallet</param>
        public abstract Task GenerateWallet(string passKey);

        /// <summary>
        ///     Delete the wallet
        /// </summary>
        public abstract Task DeleteWallet();

        /// <summary>
        ///     Check if a wallet exists
        /// </summary>
        /// <returns>does the wallet exist</returns>
        public abstract Task<bool> WalletExists();

        /// <summary>
        ///     Sign a transaction payload
        /// </summary>
        /// <param name="rawTxn">The transaction to sign</param>
        public abstract Task<string> Sign(RawPayload rawTxn);

        /// <summary>
        ///     Get the wallets address
        /// </summary>
        /// <returns>the wallets address</returns>
        public abstract Task<string> GetAddress();
    }
}