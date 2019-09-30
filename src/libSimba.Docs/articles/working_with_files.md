# Files in `libSimba.net`


## Calling methods
Several options are available for interacting with files in `libSimba.net`.

#### Byte Arrays
This method passes a simple `List` of in memory `byte[]` to the call. 
```c#
byte[] byteArray = Encoding.ASCII.GetBytes("Testing 1-2-3");

var files = new List<byte[]>();
files.Add(byteArray);

var parameters = new Dictionary<string, object>()
{
    {"assetId", "0x00"},
    {"message", "C# Test"},
    {"chatRoom", "C# Test Room"},
    {"sentBy", "Kieran Evans"}
};

var txn = await Simba.CallMethodWithFile("createRoom", parameters, files);

//Wait for the transaction to deploy to the blockchain
var deployedTxn = await Simba.WaitForSuccessOrError(txn.ID);
Console.WriteLine($"Transaction Hash {deployedTxn .TransactionHash}");
```


#### FileInfo
This method passes a `List` of  `FileInfo` to the call. This leaves the library take care of opening and reading the files.
```c#
var files = new List<FileInfo>();
files.Add(new FileInfo("path/to/file.txt"));

...

var txn = await Simba.CallMethodWithFile("createRoom", parameters, files);

...
```


#### Streams
This method passes a `List` of  `Stream` to the call. This can be useful e.g. if you were receiving the file from elsewhere, such as a HTTP request, saving you from having to store the file, or host it in memory. 
```c#
var files = new List<Stream>();
files.Add(new StreamReader("path/to/file.txt"));

...

var txn = await Simba.CallMethodWithFile("createRoom", parameters, files);

...
```


#### FileData
This method passes a `List` of  `FileData` objects to the call. This gives a little more control of the the `Stream` method, allowing you to additionally specify file names, and mime types.
```c#
var files = new List<FileData>();
files.Add(new FileData("file.dat", "application/octet-stream", aStreamInstance));
...

var txn = await Simba.CallMethodWithFile("createRoom", parameters, files);

...
```

## Retrieving files

Calling `simba.GetBundleMetadata` allows you to retrieve a list of files, their names, and their mime types for a transaction.

```c#
var bundleMeta = await Simba.GetBundleMetadataForTransaction(txn.TransactionHash);
Console.WriteLine($"files count {bundleMeta.Files.Length}");
Console.WriteLine($"first file name {bundleMeta.Files[0].Name}");
``` 

Once you have the file's name you can retrieve the file with `simba.GetFileFromBundleByNameForTransaction`

```c#
using(var stream = await Simba.GetFileFromBundleByNameForTransaction(txn.TransactionHash, "file.dat"))
using (var 
output = File.OpenWrite("file.dat"))
{
	stream.CopyTo(output);
}
``` 