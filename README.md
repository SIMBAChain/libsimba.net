<h1 align="center">Welcome to libsimba.net</h1>
<p>
  <a href="https://www.nuget.org/packages/libsimba.net">
    <img alt="npm" src="https://img.shields.io/nuget/dt/libsimba.net?style=flat">  
  </a>
  <a href="https://simbachain.github.io/libsimba.net">
    <img alt="Documentation" src="https://img.shields.io/badge/documentation-yes-brightgreen.svg?style=flat" target="_blank" />
  </a>
  <a href="https://github.com/SIMBAChain/libsimba.net/blob/master/LICENSE">
    <img alt="License: MIT" src="https://img.shields.io/badge/License-MIT-yellow.svg?style=flat" target="_blank" />
  </a>
  <img alt="azure" src="https://dev.azure.com/SimbaChain/libSimba/_apis/build/status/SIMBAChain.libsimba.net-develop?branchName=develop">
</p>

> libsimba.net is a library simplifying the use of SIMBAChain APIs. We aim to abstract away the various blockchain concepts, reducing the necessary time needed to get to working code.

### [🏠 Homepage](https://github.com/simbachain/libsimba.net)
### [📝 Documentation](https://simbachain.github.io/libsimba.net)

## Install

#### .NET CLI
```ps
dotnet add package LibSimba.Net
```

#### Nuget Package Manger
```ps
Install-Package LibSimba.Net
```

### Basic usage
#### Get an instance of `FileWallet`
```c#
var wallet = new FileWallet("wallet.json", null);
if (await Wallet.WalletExists())
{
    await Wallet.UnlockWallet("test1234");
}
else
{
    await Wallet.GenerateWallet("test1234");
}
```

#### Get an instance of `Simbachain`
```c#
var simba = await SimbaBase.GetSimbaInstance(
    "https://api.simbachain.com/v1/libSimba-SimbaChat-Quorum/",
    wallet,
    "04d1729f7144873851a745d2ae85639f55c8e3de5aea626a2bcd0055c01ba6fc");
```

#### Call a method
```c#
var parameters = new Dictionary<string, object>()
{
    {"assetId", "0x00"},
    {"name", "C# Test Room"},
    {"createdBy", "Kieran Evans"}
};

var txn = await Simba.CallMethod("createRoom", parameters);

Console.WriteLine($"Transaction ID {txn.ID}");

//Wait for the transaction to deploy to the blockchain
var deployedTxn = await Simba.WaitForSuccessOrError(txn.ID);

Console.WriteLine($"Transaction Hash {deployedTxn .TransactionHash}");
```

## Examples

See [here](https://github.com/SIMBAChain/libsimba.net/src/libSimba.Example)

## Contributing

Contributions, issues and feature requests are welcome!<br />Feel free to check [issues page](https://github.com/simbachain/libsimba.net/issues).

## License

Copyright © 2019 [SIMBAChain Inc](https://simbachain.com/).<br />
This project is [MIT](https://github.com/SIMBAChain/libsimba.net/blob/master/LICENSE) licensed.