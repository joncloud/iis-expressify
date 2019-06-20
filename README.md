# IISExpressify
[![NuGet](https://img.shields.io/nuget/v/IISExpressify.svg)](https://www.nuget.org/packages/IISExpressify/)

## Description
IISExpressify is a simple wrapper for running IIS Express.

## Licensing
Released under the MIT License.  See the [LICENSE][] file for further details.

[license]: LICENSE.md

## Installation
In the Package Manager Console execute

```powershell
Install-Package IISExpressify
```

Or update `*.csproj` to include a dependency on

```xml
<ItemGroup>
  <PackageReference Include="IISExpressify" Version="0.1.0-*" />
</ItemGroup>
```

## Usage
Sample HTTP server running on 8080:
```csharp
using IISExpressify;
using System.IO;

static async Task Main(string[] args) {
  var path = Environment.CurrentDirectory;
  var file = Path.Combine(path, "test.txt");
  File.WriteAllText(file, "lorem ipsum");

  using (var iisExpress = IISExpress.Http().PhysicalPath(path).Port(8080).Start())
  using (var http = iisExpress.CreateHttpClient()) {
    var contents = await http.GetStringAsync("/test.txt");
    Console.WriteLine(contents);
  }
}
```

Running this will respond with
```PowerShell
> dotnet MyProgram.dll
lorem ipsum
```

For additional usage see [Tests][].

[Tests]: tests/IISExpressify.Tests
