using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using libSimba.Net.Exceptions;
using libSimba.Net.Models;
using libSimba.Net.Models.Swagger;
using libSimba.Net.Models.Transaction;
using libSimba.Net.Wallet;

[assembly: InternalsVisibleToAttribute("libSimba.Tests")]

namespace libSimba.Net.Simba
{
    /// <summary>
    ///     libsimba API Interaction implementation for Simbachain.com
    /// </summary>
    public class SimbaChain : SimbaBase
    {
        /// <summary>
        ///     Internal constructor
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="wallet"></param>
        protected internal SimbaChain(string endpoint, WalletBase wallet) : base(endpoint, wallet)
        {
        }

        /// <summary>
        ///     Internal constructor
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        protected override async Task Initialize(CancellationToken ct = default)
        {
            var getSwagger = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{Endpoint}?format=openapi"),
                Headers =
                {
                    {HttpRequestHeader.ContentType.ToString(), "application/json"}
                }
            };


            try
            {
                var swagger = await DoHttp<Swagger>(getSwagger, null, ct);
                Metadata = swagger.Info.SimbaAttrs;
            }
            catch (HttpException ex)
            {
                throw new MissingMetadataException(ex);
            }
        }

        /// <summary>
        ///     Call a method on the API.
        ///     Use this method to call a method on your smart contract. It will generate the transaction, sign the transaction,
        ///     then deploy the transaction to the blockchain.
        /// </summary>
        /// <param name="method">the method to call</param>
        /// <param name="parameters">the Parameters for the method</param>
        /// <param name="ct">cancellation token</param>
        /// <returns>the transaction details</returns>
        public override async Task<Transaction> CallMethod(
            string method,
            Dictionary<string, object> parameters,
            CancellationToken ct = default)
        {
            if (Wallet == null) throw new WalletNotFoundException("No Wallet found");

            var localParameters = new Dictionary<string, object>(parameters);
            localParameters.Add("from", await Wallet.GetAddress());

            ValidateCall(method, localParameters);

            var encodedContent = new FormUrlEncodedContent(StringifyParameters(localParameters));

            return await SendMethodRequest(method, encodedContent, ct);
        }

        /// <summary>
        ///     Common parts for CallMethod and CallMethodWithFile
        /// </summary>
        /// <param name="method"></param>
        /// <param name="content"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        private async Task<Transaction> SendMethodRequest(
            string method,
            HttpContent content,
            CancellationToken ct = default)
        {
            var auth = ApiAuthHeaders();

            //Generate a request, set auth header, and content. 
            var generateTxnRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{Endpoint}{method}/"),
                Headers =
                {
                    {auth.Key, auth.Value}
                },
                Content = content
            };


            generateTxnRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            Transaction txn;

            try
            {
                txn = await DoHttp<Transaction>(generateTxnRequest, null, ct);
            }
            catch (HttpException ex)
            {
                throw new GenerateTransactionException(ex);
            }

            ct.ThrowIfCancellationRequested();

            //Sign the raw transaction
            var signed = await Wallet.Sign(txn.Payload.Raw);

            //Generate a new HTTP request to send the signed transaction
            var encodedContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"payload", signed}
            });

            var signTxnRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{Endpoint}transaction/{txn.ID}/"),
                Headers =
                {
                    {HttpRequestHeader.ContentType.ToString(), "application/json"},
                    {auth.Key, auth.Value}
                },
                Content = encodedContent
            };

            try
            {
                await DoHttp<Transaction>(signTxnRequest, null, ct);
            }
            catch (HttpException ex)
            {
                throw new SubmitTransactionException(ex);
            }

            return txn;
        }

        /// <summary>
        ///     Call a method on the API with files
        ///     Use this method to call a method on your smart contract with offchained files. It will generate the transaction,
        ///     sign the transaction, then deploy the transaction to the blockchain.
        ///     This version accepts a List of Streams that represent the files to be sent
        /// </summary>
        /// <param name="method">the method to call</param>
        /// <param name="parameters">the Parameters for the method</param>
        /// <param name="files">the files</param>
        /// <param name="ct">cancellation token</param>
        /// <returns>the transaction details</returns>
        public override async Task<Transaction> CallMethodWithFile(
            string method,
            Dictionary<string, object> parameters,
            List<Stream> files,
            CancellationToken ct = default)
        {
            if (Wallet == null) throw new WalletNotFoundException("No Wallet found");

            if (files.Count <= 0) throw new MethodCallException("files is empty");

            var localParameters = new Dictionary<string, object>(parameters)
            {
                {"from", await Wallet.GetAddress()}
            };

            ValidateCall(method, localParameters, files);

            using (var content = new MultipartFormDataContent())
            {
                var i = 0;
                foreach (var file in files)
                {
                    var c = new StreamContent(file);
                    c.Headers.Add("Content-Type", "application/octet-stream");
                    var name = $"file[{i++}]";
                    content.Add(c, name, name);
                }

                foreach (var entry in localParameters)
                    content.Add(new StringContent(entry.Value.ToString()), entry.Key);


                return await SendMethodRequest(method, content, ct);
            }
        }

        /// <summary>
        ///     Call a method on the API with files
        ///     Use this method to call a method on your smart contract with offchained files. It will generate the transaction,
        ///     sign the transaction, then deploy the transaction to the blockchain.
        ///     This version accepts a List of <see cref="FileData" /> objects that represent the files to be sent
        /// </summary>
        /// <param name="method">the method to call</param>
        /// <param name="parameters">the Parameters for the method</param>
        /// <param name="files">the files</param>
        /// <param name="ct">cancellation token</param>
        /// <returns>the transaction details</returns>
        public override async Task<Transaction> CallMethodWithFile(
            string method,
            Dictionary<string, object> parameters,
            List<FileData> files,
            CancellationToken ct = default)
        {
            if (Wallet == null) throw new WalletNotFoundException("No Wallet found");

            if (files.Count <= 0) throw new MethodCallException("files is empty");

            var localParameters = new Dictionary<string, object>(parameters)
            {
                {"from", await Wallet.GetAddress()}
            };

            ValidateCall(method, localParameters, files);

            using (var content = new MultipartFormDataContent())
            {
                var i = 0;
                foreach (var file in files)
                {
                    var c = new StreamContent(file.Stream);
                    c.Headers.Add("Content-Type", file.MimeType);
                    content.Add(c, $"file[{i++}]", file.FileName);
                }

                foreach (var entry in localParameters)
                    content.Add(new StringContent(entry.Value.ToString()), entry.Key);


                return await SendMethodRequest(method, content, ct);
            }
        }

        /// <summary>
        ///     Call a method on the API with files
        ///     Use this method to call a method on your smart contract with offchained files. It will generate the transaction,
        ///     sign the transaction, then deploy the transaction to the blockchain.
        ///     This version accepts a List of <see cref="FileInfo" /> objects that represent the files to be sent
        /// </summary>
        /// <param name="method">the method to call</param>
        /// <param name="parameters">the Parameters for the method</param>
        /// <param name="files">the files</param>
        /// <param name="ct">cancellation token</param>
        /// <returns>the transaction details</returns>
        public override async Task<Transaction> CallMethodWithFile(
            string method,
            Dictionary<string, object> parameters,
            List<FileInfo> files,
            CancellationToken ct = default)
        {
            if (Wallet == null) throw new WalletNotFoundException("No Wallet found");

            if (files.Count <= 0) throw new MethodCallException("files is empty");

            var localParameters = new Dictionary<string, object>(parameters)
            {
                {"from", await Wallet.GetAddress()}
            };

            ValidateCall(method, localParameters, files);

            using (var content = new MultipartFormDataContent())
            {
                var i = 0;
                foreach (var file in files)
                {
                    var fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read);
                    var c = new StreamContent(fs);
                    c.Headers.Add("Content-Type", "application/octet-stream");
                    content.Add(c, $"file[{i++}]", file.Name);
                }

                foreach (var entry in localParameters)
                    content.Add(new StringContent(entry.Value.ToString()), entry.Key);


                return await SendMethodRequest(method, content, ct);
            }
        }

        /// <summary>
        ///     Call a method on the API with files
        ///     Use this method to call a method on your smart contract with offchained files. It will generate the transaction,
        ///     sign the transaction, then deploy the transaction to the blockchain.
        ///     This version accepts a List of byte arrays that represent the files to be sent
        /// </summary>
        /// <param name="method">the method to call</param>
        /// <param name="parameters">the Parameters for the method</param>
        /// <param name="files">the files</param>
        /// <param name="ct">cancellation token</param>
        /// <returns>the transaction details</returns>
        public override async Task<Transaction> CallMethodWithFile(
            string method,
            Dictionary<string, object> parameters,
            List<byte[]> files,
            CancellationToken ct = default)
        {
            if (Wallet == null) throw new WalletNotFoundException("No Wallet found");

            if (files.Count <= 0) throw new MethodCallException("files is empty");

            var localParameters = new Dictionary<string, object>(parameters)
            {
                {"from", await Wallet.GetAddress()}
            };

            ValidateCall(method, localParameters, files);

            using (var content = new MultipartFormDataContent())
            {
                var i = 0;
                foreach (var file in files)
                {
                    var c = new ByteArrayContent(file);
                    c.Headers.Add("Content-Type", "application/octet-stream");
                    var name = $"file[{i++}]";
                    content.Add(c, name, name);
                }

                foreach (var entry in localParameters)
                    content.Add(new StringContent(entry.Value.ToString()), entry.Key);


                return await SendMethodRequest(method, content, ct);
            }
        }

        /// <summary>
        ///     Gets a paged list of transactions for the method
        /// </summary>
        /// <param name="method">The method</param>
        /// <param name="parameters">The query Parameters</param>
        /// <param name="ct">cancellation token</param>
        public override async Task<PagedResponse<Transaction>> GetMethodTransactions(
            string method,
            Dictionary<string, object> parameters,
            CancellationToken ct = default)
        {
            ValidateGetCall(method, parameters);

            var auth = ApiAuthHeaders();

            var getTransactions = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{Endpoint}transaction/{method}"),
                Headers =
                {
                    {HttpRequestHeader.ContentType.ToString(), "application/json"},
                    {auth.Key, auth.Value}
                }
            };

            try
            {
                var pr = GetPagedResponseInstance<Transaction>(getTransactions.RequestUri.AbsolutePath);
                return await DoHttp(getTransactions, pr, ct);
            }
            catch (HttpException ex)
            {
                throw new GetTransactionException(ex);
            }
        }

        /// <summary>
        ///     Gets the status of a transaction by ID
        /// </summary>
        /// <param name="transactionId">a transaction ID</param>
        /// <param name="ct">cancellation token</param>
        public override async Task<TransactionStatus> CheckTransactionStatus(string transactionId,
            CancellationToken ct = default)
        {
            var txn = await GetTransaction(transactionId, ct);
            return CheckTransactionStatusFromObject(txn);
        }

        /// <summary>
        ///     Gets the status of a transaction
        /// </summary>
        /// <param name="transaction">a transaction object</param>
        protected override TransactionStatus CheckTransactionStatusFromObject(Transaction transaction)
        {
            var status = new TransactionStatus();

            if (transaction.TransactionHash != null) status.TransactionHash = transaction.TransactionHash;

            if (transaction.Error != null)
            {
                status.Status = "error";
                status.Error = transaction.Error;
                status.ErrorDetails = transaction.ErrorDetails;
            }
            else if (transaction.Receipt != null)
            {
                status.Status = "pending";
            }
            else
            {
                status.Status = "success";
            }

            return status;
        }

        /// <summary>
        ///     Check if the transaction is complete
        /// </summary>
        /// <param name="transaction">the transaction object</param>
        protected override bool CheckTransactionDone(TransactionStatus transaction)
        {
            return transaction.Status != "pending";
        }

        /// <summary>
        ///     Get the status of a transaction by ID
        /// </summary>
        /// <param name="transactionIdOrHash">the transaction ID</param>
        /// <param name="ct">cancellation token</param>
        public override async Task<Transaction> GetTransaction(string transactionIdOrHash,
            CancellationToken ct = default)
        {
            ValidateAnyGetCall();

            var auth = ApiAuthHeaders();

            var getTransaction = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{Endpoint}transaction/{transactionIdOrHash}"),
                Headers =
                {
                    {HttpRequestHeader.ContentType.ToString(), "application/json"},
                    {auth.Key, auth.Value}
                }
            };

            try
            {
                return await DoHttp<Transaction>(getTransaction, null, ct);
            }
            catch (HttpException ex)
            {
                throw new GetTransactionException(ex);
            }
        }

        /// <summary>
        ///     Gets a paged list of transactions
        /// </summary>
        /// <param name="parameters">The query Parameters</param>
        /// <param name="ct">cancellation token</param>
        public override async Task<PagedResponse<Transaction>> GetTransactions(
            Dictionary<string, object> parameters = null,
            CancellationToken ct = default)
        {
            ValidateAnyGetCall();

            var auth = ApiAuthHeaders();
            var uriBuilder = new UriBuilder($"{Endpoint}transaction/");

            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            if (parameters != null)
                foreach (var keyValuePair in parameters)
                    query.Add(keyValuePair.Key, keyValuePair.Value.ToString());

            uriBuilder.Query = query.ToString();

            var getTransactions = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = uriBuilder.Uri,
                Headers =
                {
                    {HttpRequestHeader.ContentType.ToString(), "application/json"},
                    {auth.Key, auth.Value}
                }
            };


            try
            {
                var pr = GetPagedResponseInstance<Transaction>(getTransactions.RequestUri.AbsoluteUri);
                return await DoHttp(getTransactions, pr, ct);
            }
            catch (HttpException ex)
            {
                throw new GetTransactionException(ex);
            }
        }

        /// <summary>
        ///     Gets a the bundle metadata for a transaction
        /// </summary>
        /// <param name="transactionIdOrHash">Either a transaction ID or a transaction hash</param>
        /// <param name="ct">cancellation token</param>
        public override async Task<BundleManifest> GetBundleMetadataForTransaction(string transactionIdOrHash,
            CancellationToken ct = default)
        {
            ValidateAnyGetCall();

            var auth = ApiAuthHeaders();
            var uri = new Uri($"{Endpoint}transaction/{transactionIdOrHash}/bundle_manifest/?no_files=true");

            var getManifest = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = uri,
                Headers =
                {
                    {HttpRequestHeader.ContentType.ToString(), "application/json"},
                    {auth.Key, auth.Value}
                }
            };


            try
            {
                return await DoHttp<BundleManifest>(getManifest, null, ct);
            }
            catch (HttpException ex)
            {
                throw new GetBundleMetadataException(ex);
            }
        }

        /// <summary>
        ///     Gets the bundle for a transaction
        /// </summary>
        /// <param name="transactionIdOrHash">Either a transaction ID or a transaction hash</param>
        /// <param name="stream">TODO</param>
        /// <param name="ct">cancellation token</param>
        public override async Task<Stream> GetBundleForTransaction(string transactionIdOrHash, bool stream,
            CancellationToken ct = default)
        {
            ValidateAnyGetCall();

            var auth = ApiAuthHeaders();
            var uri = new Uri($"{Endpoint}transaction/{transactionIdOrHash}/bundle_raw/");

            var getBundleRaw = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = uri,
                Headers =
                {
                    {HttpRequestHeader.ContentType.ToString(), "application/octet-stream"},
                    {auth.Key, auth.Value}
                }
            };


            try
            {
                return await DoHttpToStream(getBundleRaw, ct);
            }
            catch (HttpException ex)
            {
                throw new GetBundleMetadataException(ex);
            }
        }

        /// <summary>
        ///     Gets a file from the bundle for a transaction
        /// </summary>
        /// <param name="transactionIdOrHash">Either a transaction ID or a transaction hash</param>
        /// <param name="fileIdx">The index of the file in the bundle metadata</param>
        /// <param name="stream">TODO</param>
        /// <param name="ct">cancellation token</param>
        public override async Task<Stream> GetFileFromBundleForTransaction(string transactionIdOrHash, int fileIdx,
            bool stream,
            CancellationToken ct = default)
        {
            ValidateAnyGetCall();

            var auth = ApiAuthHeaders();
            var uri = new Uri($"{Endpoint}transaction/{transactionIdOrHash}/file/{fileIdx}/");

            var getBundleFile = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = uri,
                Headers =
                {
                    {HttpRequestHeader.ContentType.ToString(), "application/octet-stream"},
                    {auth.Key, auth.Value}
                }
            };


            try
            {
                return await DoHttpToStream(getBundleFile, ct);
            }
            catch (HttpException ex)
            {
                throw new GetBundleMetadataException(ex);
            }
        }

        /// <summary>
        ///     Gets a file from the bundle for a transaction
        /// </summary>
        /// <param name="transactionIdOrHash">Either a transaction ID or a transaction hash</param>
        /// <param name="fileName">The name of the file in the bundle metadata</param>
        /// <param name="ct">cancellation token</param>
        public override async Task<Stream> GetFileFromBundleByNameForTransaction(string transactionIdOrHash,
            string fileName, CancellationToken ct = default)
        {
            ValidateAnyGetCall();

            var auth = ApiAuthHeaders();
            var uri = new Uri($"{Endpoint}transaction/{transactionIdOrHash}/fileByName/{fileName}/");

            var getBundleFile = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = uri,
                Headers =
                {
                    {HttpRequestHeader.ContentType.ToString(), "application/octet-stream"},
                    {auth.Key, auth.Value}
                }
            };


            try
            {
                return await DoHttpToStream(getBundleFile, ct);
            }
            catch (HttpException ex)
            {
                throw new GetBundleMetadataException(ex);
            }
        }

        /// <summary>
        ///     Get the balance for the attached Wallet
        /// </summary>
        /// <param name="ct">cancellation token</param>
        public override async Task<Balance> GetBalance(CancellationToken ct = default)
        {
            ValidateAnyGetCall();


            if (Wallet == null) throw new WalletNotFoundException("No Wallet found");

            if (Metadata.POA)
                return new Balance
                {
                    Amount = -1,
                    Currency = "",
                    POA = true
                };

            var address = await Wallet.GetAddress();

            var auth = ApiAuthHeaders();
            var uri = new Uri($"{Endpoint}balance/{address}/");

            var getBalance = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = uri,
                Headers =
                {
                    {HttpRequestHeader.ContentType.ToString(), "application/json"},
                    {auth.Key, auth.Value}
                }
            };


            try
            {
                return await DoHttp<Balance>(getBalance, null, ct);
            }
            catch (HttpException ex)
            {
                throw new GetBundleMetadataException(ex);
            }
        }

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
        public override async Task<Funds> AddFunds(CancellationToken ct = default)
        {
            ValidateAnyGetCall();


            if (Wallet == null) throw new WalletNotFoundException("No Wallet found");

            if (Metadata.POA)
                return new Funds
                {
                    POA = true
                };

            if (string.IsNullOrEmpty(Metadata.Faucet))
                return new Funds
                {
                    FaucetUrl = Metadata.Faucet
                };

            var address = await Wallet.GetAddress();

            var auth = ApiAuthHeaders();
            var uri = new Uri($"{Endpoint}balance/{address}/");

            //Generate a new HTTP request to send the signed transaction
            var encodedContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"account", address},
                {"value", "1"},
                {"currency", "ether"}
            });

            var addFunds = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = uri,
                Headers =
                {
                    {HttpRequestHeader.ContentType.ToString(), "application/json"},
                    {auth.Key, auth.Value}
                },
                Content = encodedContent
            };


            try
            {
                return await DoHttp<Funds>(addFunds, null, ct);
            }
            catch (HttpException ex)
            {
                throw new GetBundleMetadataException(ex);
            }
        }
    }
}