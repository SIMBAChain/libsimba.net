# Getting Started with `libSimba.net`


## Getting Started
### Installation

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