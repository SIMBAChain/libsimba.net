using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using libSimba.Net.Exceptions;
using libSimba.Net.Models;
using libSimba.Net.Models.Swagger;
using libSimba.Net.Models.Transaction;
using libSimba.Net.Wallet;
using Newtonsoft.Json;

[assembly: InternalsVisibleTo("libSimba-Tests")]

namespace libSimba.Net.Simba
{
    /// <summary>
    ///     Base class for libsimba API Interaction implementations
    /// </summary>
    public abstract class SimbaBase
    {
        private static HttpClient _client;
        protected string ApiKey;
        protected string Endpoint;
        protected string ManagementKey;
        protected ApplicationMetadata Metadata;

        protected WalletBase Wallet;

        /// <summary>
        ///     Base class for libsimba API Interaction implementations
        /// </summary>
        /// <param name="endpoint">The endpoint of the API</param>
        /// <param name="wallet">an optional <see cref="WalletBase" /> instance</param>
        protected internal SimbaBase(string endpoint, WalletBase wallet)
        {
            if (endpoint.EndsWith("/"))
                Endpoint = endpoint;
            else
                Endpoint = endpoint + "/";

            Wallet = wallet;
        }

        protected internal static HttpClient Client
        {
            get => _client ?? (_client = new HttpClient());
            set => _client = value;
        }

        /// <summary>
        ///     Create an instance of a Simbachain API interaction class
        ///     Automatically takes care of choosing the correct implementation and running asynchronous initialisation.
        /// </summary>
        /// <param name="url">The API URL</param>
        /// <param name="wallet">The Wallet to use</param>
        /// <param name="apiKey">The API key</param>
        /// <param name="managementKey">The Management API key</param>
        /// <param name="ct"></param>
        /// <returns>An initialised instance of the API interaction class</returns>
        public static async Task<SimbaBase> GetSimbaInstance(
            string url,
            WalletBase wallet = null,
            string apiKey = null,
            string managementKey = null,
            CancellationToken ct = default)
        {
            if (url.StartsWith("https://api.simbachain.com") || url.StartsWith("http://127.0.0.1:8000"))
            {
                var simba = new SimbaChain(url, wallet);

                if (apiKey != null) simba.SetApiKey(apiKey);

                if (managementKey != null) simba.SetManagementKey(managementKey);

                await simba.Initialize();

                return simba;
            }

            return null;
        }

        /// <summary>
        ///     Perform any asynchronous actions needed to initialise this class
        /// </summary>
        /// <param name="ct">cancellation token</param>
        protected abstract Task Initialize(CancellationToken ct = default);

        /// <summary>
        ///     Call a method on the API.
        /// </summary>
        /// <param name="method">the method to call</param>
        /// <param name="parameters">the Parameters for the method</param>
        /// <param name="ct">cancellation token</param>
        /// <returns>the transaction details</returns>
        public abstract Task<Transaction> CallMethod(
            string method,
            Dictionary<string, object> parameters,
            CancellationToken ct = default);

        /// <summary>
        ///     Call a method on the API with files
        ///     Uses Streams, useful for large datasets where holding in-memory may not be feasable, or to reduce memory
        ///     consumption overall.
        /// </summary>
        /// <param name="method">the method to call</param>
        /// <param name="parameters">the Parameters for the method</param>
        /// <param name="files">the files as a List of Streams</param>
        /// <param name="ct">cancellation token</param>
        /// <returns>the transaction details</returns>
        public abstract Task<Transaction> CallMethodWithFile(
            string method,
            Dictionary<string, object> parameters,
            List<Stream> files,
            CancellationToken ct = default);

        /// <summary>
        ///     Call a method on the API with files
        ///     Uses FileData objects, allowing you to send streams, but also specify the file name and mime type
        /// </summary>
        /// <param name="method">the method to call</param>
        /// <param name="parameters">the Parameters for the method</param>
        /// <param name="files">the files as a List of FileData objects.</param>
        /// <param name="ct">cancellation token</param>
        /// <returns>the transaction details</returns>
        public abstract Task<Transaction> CallMethodWithFile(
            string method,
            Dictionary<string, object> parameters,
            List<FileData> files,
            CancellationToken ct = default);

        /// <summary>
        ///     Call a method on the API with files
        ///     Uses FileInfo, to allow libSimba to take care of file operations instead.
        /// </summary>
        /// <param name="method">the method to call</param>
        /// <param name="parameters">the Parameters for the method</param>
        /// <param name="files">the files as a List of FileInfo</param>
        /// <param name="ct">cancellation token</param>
        /// <returns>the transaction details</returns>
        public abstract Task<Transaction> CallMethodWithFile(
            string method,
            Dictionary<string, object> parameters,
            List<FileInfo> files,
            CancellationToken ct = default);

        /// <summary>
        ///     Call a method on the API with files
        ///     Uses byte arrays to contain the file data.
        /// </summary>
        /// <param name="method">the method to call</param>
        /// <param name="parameters">the Parameters for the method</param>
        /// <param name="files">the files as a List of byte arrays</param>
        /// <param name="ct">cancellation token</param>
        /// <returns>the transaction details</returns>
        public abstract Task<Transaction> CallMethodWithFile(
            string method,
            Dictionary<string, object> parameters,
            List<byte[]> files,
            CancellationToken ct = default);

        /// <summary>
        ///     Gets a paged list of transactions for the method
        /// </summary>
        /// <param name="method">The method</param>
        /// <param name="parameters">The query Parameters</param>
        /// <param name="ct">cancellation token</param>
        public abstract Task<PagedResponse<Transaction>> GetMethodTransactions(
            string method,
            Dictionary<string, object> parameters,
            CancellationToken ct = default);

        /// <summary>
        ///     Gets the status of a transaction by ID
        /// </summary>
        /// <param name="transactionId">a transaction ID</param>
        /// <param name="ct">cancellation token</param>
        public abstract Task<TransactionStatus> CheckTransactionStatus(
            string transactionId,
            CancellationToken ct = default);

        /// <summary>
        ///     Gets the status of a transaction
        /// </summary>
        /// <param name="transaction">a transaction object</param>
        protected abstract TransactionStatus CheckTransactionStatusFromObject(Transaction transaction);

        /// <summary>
        ///     Check if the transaction is complete
        /// </summary>
        /// <param name="transaction">the transaction object</param>
        protected abstract bool CheckTransactionDone(TransactionStatus transaction);

        /// <summary>
        ///     Get the status of a transaction by ID
        /// </summary>
        /// <param name="transactionIdOrHash">the transaction ID</param>
        /// <param name="ct">cancellation token</param>
        public abstract Task<Transaction> GetTransaction(
            string transactionIdOrHash,
            CancellationToken ct = default);

        /// <summary>
        ///     Gets a paged list of transactions
        /// </summary>
        /// <param name="parameters">The query Parameters</param>
        /// <param name="ct">cancellation token</param>
        public abstract Task<PagedResponse<Transaction>> GetTransactions(
            Dictionary<string, object> parameters = null,
            CancellationToken ct = default);

        /// <summary>
        ///     Create an instance of PagedResponse tied to this instance of SimbaChain
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        protected internal PagedResponse<T> GetPagedResponseInstance<T>(string url)
        {
            return new PagedResponse<T>(url, this);
        }

        /// <summary>
        ///     Gets a the bundle metadata for a transaction
        /// </summary>
        /// <param name="transactionIdOrHash">Either a transaction ID or a transaction hash</param>
        /// <param name="ct">cancellation token</param>
        public abstract Task<BundleManifest> GetBundleMetadataForTransaction(
            string transactionIdOrHash,
            CancellationToken ct = default);

        /// <summary>
        ///     Gets the bundle for a transaction
        /// </summary>
        /// <param name="transactionIdOrHash">Either a transaction ID or a transaction hash</param>
        /// <param name="stream">TODO</param>
        /// <param name="ct">cancellation token</param>
        public abstract Task<Stream> GetBundleForTransaction(
            string transactionIdOrHash,
            bool stream,
            CancellationToken ct = default);

        /// <summary>
        ///     Gets a file from the bundle for a transaction
        /// </summary>
        /// <param name="transactionIdOrHash">Either a transaction ID or a transaction hash</param>
        /// <param name="fileIdx">The index of the file in the bundle metadata</param>
        /// <param name="stream">TODO</param>
        /// <param name="ct">cancellation token</param>
        public abstract Task<Stream> GetFileFromBundleForTransaction(
            string transactionIdOrHash,
            int fileIdx,
            bool stream,
            CancellationToken ct = default);

        /// <summary>
        ///     Gets a file from the bundle for a transaction
        /// </summary>
        /// <param name="transactionIdOrHash">Either a transaction ID or a transaction hash</param>
        /// <param name="fileName">The name of the file in the bundle metadata</param>
        /// <param name="ct">cancellation token</param>
        public abstract Task<Stream> GetFileFromBundleByNameForTransaction(
            string transactionIdOrHash,
            string fileName,
            CancellationToken ct = default);

        /// <summary>
        ///     Get the balance for the attached Wallet
        /// </summary>
        /// <param name="ct">cancellation token</param>
        public abstract Task<Balance> GetBalance(CancellationToken ct = default);

        /// <summary>
        ///     Add funds to the attached Wallet.
        ///     Please check the output of this method. It is of the form
        ///     <code>
        /// {
        ///  txnId: null,
        ///  faucet_url: null,
        ///  poa: true
        /// }
        ///  </code>
        ///     If successful, txnId will be populated.
        ///     If the network is PoA, then poa will be true, and txnId will be null
        ///     If the faucet for the network is external (e.g. Rinkeby, Ropsten, etc), then txnId will be null,
        ///     and faucet_url will be populated with a URL. You should present this URL to your users to direct them
        ///     to request funds there.
        /// </summary>
        /// <param name="ct">cancellation token</param>
        public abstract Task<Funds> AddFunds(CancellationToken ct = default);

        /// <summary>
        ///     TODO
        /// </summary>
        /// <param name="transactionIdOrHash"></param>
        /// <param name="pollInterval"></param>
        /// <param name="ct">cancellation token</param>
        public async Task<Transaction> WaitForSuccessOrError(
            string transactionIdOrHash,
            int pollInterval = 5000,
            CancellationToken ct = default)
        {
            Transaction txn;
            do
            {
                await Task.Delay(pollInterval, ct);
                ct.ThrowIfCancellationRequested();
                txn = await GetTransaction(transactionIdOrHash, ct);
            } while (CheckTransactionDone(CheckTransactionStatusFromObject(txn)));

            return txn;
        }

        /// <summary>
        ///     Set the wallet
        /// </summary>
        /// <param name="wallet">the wallet</param>
        public void SetWallet(WalletBase wallet)
        {
            Wallet = wallet;
        }

        /// <summary>
        ///     Set the API Key to authenticate calls
        /// </summary>
        /// <param name="key">the API Key</param>
        public void SetApiKey(string key)
        {
            ApiKey = key;
        }

        /// <summary>
        ///     Set the API Key to authenticate management calls
        /// </summary>
        /// <param name="key">the management API Key</param>
        public void SetManagementKey(string key)
        {
            ManagementKey = key;
        }

        /// <summary>
        ///     Get API Call auth headers
        /// </summary>
        /// <returns></returns>
        protected internal KeyValuePair<string, string> ApiAuthHeaders()
        {
            return new KeyValuePair<string, string>("APIKEY", ApiKey);
        }

        /// <summary>
        ///     Get management API Call auth headers
        /// </summary>
        /// <returns></returns>
        protected internal KeyValuePair<string, string> ManagementAuthHeaders()
        {
            return new KeyValuePair<string, string>("APIKEY", ManagementKey);
        }

        /// <summary>
        ///     Validate the method call against the app metadata
        /// </summary>
        /// <param name="methodName">the Methods Name</param>
        /// <param name="parameters">the Parameters for the method call</param>
        /// <returns></returns>
        /// <exception cref="MissingMetadataException"></exception>
        /// <exception cref="BadMetadataException"></exception>
        /// <exception cref="MethodCallException"></exception>
        protected bool ValidateCall(string methodName, Dictionary<string, object> parameters)
        {
            if (Metadata == null) throw new MissingMetadataException("App Metadata not yet retrieved");


            if (Metadata.Methods == null) throw new BadMetadataException("App Metadata doesn't have Methods!");

            if (!Metadata.Methods.TryGetValue(methodName, out var methodMeta))
                throw new MethodCallException($"Method {methodName} not found");

            var paramNames = parameters.Keys;

            if (!paramNames.Any(key => key == "from"))
                throw new MethodCallException($"Parameter [from] is not present for method '{methodName}'");

            var invalid = paramNames
                .Where(key => !(methodMeta.Parameters.ContainsKey(key) || key == "_files" || key == "from")).ToArray();

            if (invalid.Length > 0)
                throw new MethodCallException(
                    $"Parameters [{string.Join(",", invalid)}] are not valid for method '{methodName}'");

            var missing = methodMeta.Parameters.Keys.Where(key => !(paramNames.Contains(key) || key == "_files"))
                .ToArray();

            if (missing.Length > 0)
                throw new MethodCallException(
                    $"Parameters [{string.Join(",", missing)}] not present for method '{methodName}'");


            return true;
        }


        /// <summary>
        ///     Validate the method call against the app metadata
        /// </summary>
        /// <param name="methodName">the Methods Name</param>
        /// <param name="parameters">the Parameters for the method call</param>
        /// <param name="files">Optional array of files</param>
        /// <returns></returns>
        /// <exception cref="MissingMetadataException"></exception>
        /// <exception cref="BadMetadataException"></exception>
        /// <exception cref="MethodCallException"></exception>
        protected bool ValidateCall<T>(string methodName, Dictionary<string, object> parameters, List<T> files = null)
        {
            if (Metadata == null) throw new MissingMetadataException("App Metadata not yet retrieved");


            if (Metadata.Methods == null) throw new BadMetadataException("App Metadata doesn't have Methods!");

            if (!Metadata.Methods.TryGetValue(methodName, out var methodMeta))
                throw new MethodCallException($"Method {methodName} not found");

            if (files != null)
                if (!methodMeta.Parameters.ContainsKey("_files"))
                    throw new MethodCallException($"Method '{methodName}'' does not accept files");

//                for (var i = 0; i < files.Count; i++)
//                {
//
//                    //TODO: check file is of appropriate Type
//                }

            var paramNames = parameters.Keys;

            if (!paramNames.Any(key => key == "from"))
                throw new MethodCallException($"Parameter [from] is not present for method '{methodName}'");

            var invalid = paramNames
                .Where(key => !(methodMeta.Parameters.ContainsKey(key) || key == "_files" || key == "from")).ToArray();

            if (invalid.Length > 0)
                throw new MethodCallException(
                    $"Parameters [{string.Join(",", invalid)}] are not valid for method '{methodName}'");

            var missing = methodMeta.Parameters.Keys.Where(key => !(paramNames.Contains(key) || key == "_files"))
                .ToArray();

            if (missing.Length > 0)
                throw new MethodCallException(
                    $"Parameters [{string.Join(",", missing)}] not present for method '{methodName}'");


            return true;
        }

        /// <summary>
        ///     Validate the method call against the app metadata
        /// </summary>
        /// <param name="methodName">the Methods Name</param>
        /// <param name="parameters">the Parameters for the method call</param>
        /// <returns></returns>
        /// <exception cref="MissingMetadataException"></exception>
        /// <exception cref="BadMetadataException"></exception>
        /// <exception cref="MethodCallException"></exception>
        protected bool ValidateGetCall(string methodName, Dictionary<string, object> parameters)
        {
            if (Metadata == null) throw new MissingMetadataException("App Metadata not yet retrieved");


            if (Metadata.Methods == null) throw new BadMetadataException("App Metadata doesn't have Methods!");

            if (!Metadata.Methods.ContainsKey(methodName))
                throw new MethodCallException($@"Method {methodName} not found");

            return true;
        }

        /// <summary>
        ///     Validate the transaction list call against the app metadata
        /// </summary>
        /// <returns></returns>
        /// <exception cref="MissingMetadataException"></exception>
        /// <exception cref="BadMetadataException"></exception>
        protected bool ValidateAnyGetCall()
        {
            if (Metadata == null) throw new MissingMetadataException("App Metadata not yet retrieved");


            if (Metadata.Methods == null) throw new BadMetadataException("App Metadata doesn't have Methods!");

            return true;
        }

        /// <summary>
        ///     Convert the values in a dict to string (with .ToString())
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected Dictionary<string, string> StringifyParameters(Dictionary<string, object> parameters)
        {
            return parameters.ToDictionary(kp => kp.Key, kp => kp.Value.ToString());
        }

        /// <summary>
        ///     Perform HTTP Requests and Deserialize JSON to classes.
        /// </summary>
        /// <typeparam name="TSerializeTo"></typeparam>
        /// <param name="request">The HttpRequestMessage object</param>
        /// <param name="existingObject">If present, deserializes the response into this object</param>
        /// <param name="ct">cancellation token</param>
        /// <returns></returns>
        protected internal async Task<TSerializeTo> DoHttp<TSerializeTo>(
            HttpRequestMessage request,
            TSerializeTo existingObject,
            CancellationToken ct = default)
        {
            using (var response = await Client.SendAsync(request, ct))
            {
                //If bad, exception
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    throw new GenerateTransactionException(responseContent);
                }

                //If good, parse out the response to the Transaction object
                using (var responseContent = await response.Content.ReadAsStreamAsync())
                using (var sr = new StreamReader(responseContent))
                using (var jr = new JsonTextReader(sr))
                {
                    var serializer = new JsonSerializer();
                    if (existingObject == null) return serializer.Deserialize<TSerializeTo>(jr);

                    serializer.Populate(jr, existingObject);
                    return existingObject;
                }
            }
        }

        /// <summary>
        ///     Perform HTTP Requests and Deserialize JSON to classes.
        /// </summary>
        /// <param name="request">The HttpRequestMessage object</param>
        /// <param name="ct">cancellation token</param>
        /// <returns>A stream</returns>
        protected internal async Task<Stream> DoHttpToStream(
            HttpRequestMessage request,
            CancellationToken ct = default)
        {
            var response = await Client.SendAsync(request, ct);
            //If good, return the stream
            if (response.StatusCode == HttpStatusCode.OK) return await response.Content.ReadAsStreamAsync();

            //If bad, exception
            var responseContent = await response.Content.ReadAsStringAsync();
            throw new GenerateTransactionException(responseContent);
        }
    }
}