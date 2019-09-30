using System;
using System.IO;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using libSimba.Net.Exceptions;
using libSimba.Net.Models.Transaction;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.KeyStore;
using Nethereum.Signer;
using Nethereum.Web3.Accounts;

namespace libSimba.Net.Wallet
{
    /// <summary>
    ///     libsimba WalletBase implementation for Web3 Secret Storage format
    ///     https://github.com/ethereum/wiki/wiki/Web3-Secret-Storage-Definition
    /// </summary>
    public class FileWallet : WalletBase
    {
        /// <summary>
        ///     libsimba WalletBase implementation for Web3 Secret Storage format
        ///     https://github.com/ethereum/wiki/wiki/Web3-Secret-Storage-Definition
        /// </summary>
        /// <param name="walletPath">Path to the wallet</param>
        public FileWallet(string walletPath)
        {
            WalletPath = walletPath;
        }

        /// <summary>
        ///     libsimba WalletBase implementation for Web3 Secret Storage format
        ///     https://github.com/ethereum/wiki/wiki/Web3-Secret-Storage-Definition
        /// </summary>
        /// <param name="walletPath">Path to the wallet</param>
        /// <param name="signingConfirmation">
        ///     A function reference to call to obtain permission to sign. Accepts an argument of type <see cref="RawPayload" />,
        ///     and returns a bool.
        /// </param>
        public FileWallet(string walletPath, ISigningConfirmation signingConfirmation) : base(signingConfirmation)
        {
            WalletPath = walletPath;
        }

        protected string WalletPath { get; set; }

        protected AccountSignerTransactionManager TransactionManager { get; private set; }

        protected BigInteger? ChainId { get; set; }

        /// <summary>
        ///     Unlock the wallet
        /// </summary>
        /// <param name="passKey">The pass key to unlock the wallet</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="WalletNotFoundException"></exception>
        public override async Task UnlockWallet(string passKey)
        {
            if (passKey == null) throw new ArgumentNullException(nameof(passKey), "passKey must not be null");

            if (!File.Exists(WalletPath)) throw new WalletNotFoundException($"Wallet not found at path {WalletPath}");

            using (var fileStream = new FileStream(WalletPath, FileMode.Open, FileAccess.Read))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                var json = await streamReader.ReadToEndAsync();
                LoadFromKeyStore(json, passKey);
            }
        }

        /// <summary>
        ///     Loads the wallet from the filesystem
        /// </summary>
        /// <param name="json">The json string representing the encrypted wallet</param>
        /// <param name="passKey">The pass key to unlock the wallet</param>
        private void LoadFromKeyStore(string json, string passKey)
        {
            var keyStoreService = new KeyStoreService();
            var privateKey = keyStoreService.DecryptKeyStoreFromJson(passKey, json);
            var key = new EthECKey(privateKey, true);

            TransactionManager = new AccountSignerTransactionManager(null, key.GetPrivateKey(), ChainId);
        }

        /// <summary>
        ///     Generate a wallet, and save to the WalletPath.
        /// </summary>
        /// <param name="passKey">The pass key to lock the wallet</param>
        public override async Task GenerateWallet(string passKey)
        {
            await Task.Run(() =>
            {
                var ecKey = EthECKey.GenerateKey();

                var address = ecKey.GetPublicAddress();
                var service = new KeyStoreService();
                var encryptedKey =
                    service.EncryptAndGenerateDefaultKeyStoreAsJson(passKey, ecKey.GetPrivateKeyAsBytes(), address);


                using (var wallet = File.CreateText(WalletPath))
                {
                    wallet.Write(encryptedKey);
                    wallet.Flush();
                }
            });

            await UnlockWallet(passKey);
        }

        /// <summary>
        ///     Delete the wallet from the file system
        /// </summary>
        public override async Task DeleteWallet()
        {
            TransactionManager = null;
            File.Delete(WalletPath);
        }

        /// <summary>
        ///     Check if a wallet exists
        /// </summary>
        /// <returns>does the wallet exist</returns>
        public override async Task<bool> WalletExists()
        {
            return await Task.FromResult(File.Exists(WalletPath));
        }

        /// <summary>
        ///     Sign a transaction payload
        /// </summary>
        /// <param name="rawTxn">The transaction to sign</param>
        /// <returns>The signed transaction</returns>
        public override async Task<string> Sign(RawPayload rawTxn)
        {
            if (!await RequestConfirmation(rawTxn)) throw new SubmitTransactionException("Signing rejected");
            return await Task.FromResult(TransactionManager.SignTransaction(rawTxn).EnsureHexPrefix());
        }

        /// <summary>
        ///     Get the wallets address
        /// </summary>
        /// <returns>the wallets address</returns>
        public override async Task<string> GetAddress()
        {
            return await Task.FromResult(TransactionManager.Account.Address);
        }
    }
}