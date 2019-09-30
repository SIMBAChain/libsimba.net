using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using libSimba.Net.Models;
using libSimba.Net.Models.Transaction;
using libSimba.Net.Simba;
using libSimba.Net.Wallet;

namespace libSimba.Example
{
    internal class Program : ISigningConfirmation
    {
        public FileWallet Wallet { get; set; }
        public SimbaBase Simba { get; set; }

        /// <summary>
        ///     This acts as a callback to porentially ask user permission before signing a transaction.
        ///     Must implement/match the WalletBase.SigningConfirmationDelegate delegate.
        /// </summary>
        /// <param name="txnToSign">The transaction that will be signed</param>
        /// <returns>A boolean indicating whether or not to proceed with signing</returns>
        public Task<bool> RequestSigningConfirmation(RawPayload txnToSign)
        {
            Console.WriteLine($"Signing transaction from {txnToSign.From} to {txnToSign.To}");
            return Task.FromResult(true);
        }

        private static void Main(string[] args)
        {
            //Init and run the examples
            var program = new Program();
            program.Run().GetAwaiter().GetResult();
        }

        public async Task Run()
        {
            //Run the examples async
            await InitWallet();
            await InitSimba();
            var noFileTxn = await CallMethod();
            var filesTxn = await CallMethodWithFile();
            await GetBundleMetadata(filesTxn);
            await GetFileFromBundleByNameForTransaction(filesTxn);
        }

        /// <summary>
        ///     Initialise the wallet
        ///     If the file already exists, we try to open and decrypt it
        ///     If the file does not exist, we generate a new wallet file
        /// </summary>
        public async Task InitWallet()
        {
            Wallet = new FileWallet("wallet.json", this);
            if (await Wallet.WalletExists())
                await Wallet.UnlockWallet("test1234");
            else
                await Wallet.GenerateWallet("test1234");
        }

        /// <summary>
        ///     Retreive a new instance of SimbaBase correct for the API url
        /// </summary>
        public async Task InitSimba()
        {
            Simba = await SimbaBase.GetSimbaInstance(
                "https://api.simbachain.com/v1/libSimba-SimbaChat-Quorum/",
                Wallet,
                "04d1729f7144873851a745d2ae85639f55c8e3de5aea626a2bcd0055c01ba6fc");
        }

        /// <summary>
        ///     Calls a method on the blockchain and waits for it to complete
        /// </summary>
        /// <returns>The completed Transaction</returns>
        public async Task<Transaction> CallMethod()
        {
            var parameters = new Dictionary<string, object>
            {
                {"assetId", "0x00"},
                {"name", "C# Test Room"},
                {"createdBy", "Kieran Evans"}
            };

            var txn = await Simba.CallMethod("createRoom", parameters);

            Console.WriteLine($"Transaction ID {txn.ID}");

            var deployedTxn = await Simba.WaitForSuccessOrError(txn.ID);
            Console.WriteLine($"Transaction Hash {deployedTxn.TransactionHash}");

            return deployedTxn;
        }

        /// <summary>
        ///     Calls a method (with files) on the blockchain and waits for it to complete
        /// </summary>
        /// <returns>The completed Transaction</returns>
        public async Task<Transaction> CallMethodWithFile()
        {
            var parameters = new Dictionary<string, object>
            {
                {"assetId", "0x00"},
                {"message", "C# Test"},
                {"chatRoom", "C# Test Room"},
                {"sentBy", "Kieran Evans"}
            };

            Transaction txn;

            var byteArray = Encoding.ASCII.GetBytes("Testing 1-2-3");

            var rnd = new Random();
            var b = new byte[1024];
            rnd.NextBytes(b);

            using (var stringStream = new MemoryStream(byteArray))
            using (Stream byteStream = new MemoryStream(b))
            using (var sr = new StreamReader("TextFile1.txt"))
            {
                var files = new List<FileData>
                {
                    new FileData("stringstream.txt", "text/ascii", stringStream),
                    new FileData("bytestream.dat", byteStream),
                    new FileData("TextFile1.txt", "text/ascii", sr.BaseStream)
                };

                txn = await Simba.CallMethodWithFile("sendMessage", parameters, files);
            }

            Console.WriteLine($"Transaction ID {txn.ID}");

            var deployedTxn = await Simba.WaitForSuccessOrError(txn.ID);
            Console.WriteLine($"Transaction Hash {deployedTxn.TransactionHash}");

            return deployedTxn;
        }

        /// <summary>
        ///     Gets the bundle metadata for the files in the transaction
        /// </summary>
        /// <param name="txn">the transaction</param>
        /// <returns>the bundle metadata for the files in the transaction</returns>
        public async Task GetBundleMetadata(Transaction txn)
        {
            var bundleMeta = await Simba.GetBundleMetadataForTransaction(txn.TransactionHash);
            Console.WriteLine($"files count {bundleMeta.Files.Length}");
        }

        public async Task GetFileFromBundleByNameForTransaction(Transaction txn)
        {
            using (var stream = await Simba.GetFileFromBundleByNameForTransaction(txn.TransactionHash, "TextFile1.txt"))
            using (var output = File.OpenWrite("TextFile1.txt"))
            {
                stream.CopyTo(output);
            }
        }
    }
}