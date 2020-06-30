# excel-file-service ![.NET Core](https://github.com/leo-oliveira-eng/excel-file-service/workflows/.NET%20Core/badge.svg)  [![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.md) [![NuGet](https://img.shields.io/nuget/vpre/Excel.File.Service)](https://www.nuget.org/packages/Excel.File.Service)

Package that makes easier to import and export data from Excel spreadsheets for .Net Core applications. 

After having to implement the import of spreadsheets in 3 different projects, I couldn't avoid creating a package to encapsulate these operations.

This package uses [ExcelDataReader](https://github.com/ExcelDataReader/ExcelDataReader) as base, which is a lightweight and fast library for reading Microsoft Excel files. Presently, it is used for more than 3700 repositories on GitHub.

The idea is removing from the developer the repetitive and monotonous process of reading the spreadsheet or the process of exporting.

Currently, a preview version has been published that already supports reading and importing files from Stream, IFormFile and base64. Today, the export only supports txt files in base64 and Stream format.

Still in development, the package also proposes to deliver exporting in csv, xls and xlsx in addition to supporting password protected files.

Suggestions are welcome.

## Installation

Excel-File-Service is available on [NuGet](https://www.nuget.org/packages/Excel.File.Service/).  You can find the raw NuGet file [here](https://www.nuget.org/api/v2/package/Excel.File.Service/1.0.0-preview-1) or install it by the commands below depending on your platform:

 - Package Manager
```
pm> Install-Package Excel.File.Service -Version 1.0.0-preview-1
```

 - via the .NET Core CLI:
```
> dotnet add package Excel.File.Service --version 1.0.0-preview-1
```

 - PackageReference
```
<PackageReference Include="Excel.File.Service" Version="1.0.0-preview-1" />
```

 - PaketCLI
```
> paket add Excel.File.Service --version 1.0.0-preview-1
```

## Setup

To perform the configuration and ensure proper dependency injection just include the following code on the startup of your project or in the class you are using for this purpose:
```csharp
public void ConfigureServices(IServiceCollection services)
{
  ...
  services.AddExcelReader();
  ...
}
```

## How to Use

### importing

The spreadsheet can be read from base64, IFormFile and Stream. It will be converted to a list of registers of the class you target in your code.

Example:
Let's assume that we want to import the fictional sales spreadsheet below.
| SalesTransId | County Code | Product Id  |Sum of Sales ($ milions)|
| ------------ | ----------- | ----------- | ---------------------- |
| 1307         | USA         | 31322121    | 10.5                   |      
| 1308         | CAN         | 1266        | 159.0                  |
| 1309         | CAN         | 5498        | 14.12                  |
| 1310         | SWI         | 3300        | 358.5                  |
| 1311         | BEL         | 159483      | 20.2                   |
| 1312         | USA         | 9873        | 1500.1                 |
| 1313         | BRA         | 77658       | 135.6                  |

The DTO class for this spreadsheet would be generated as below. It is important that the order of the columns be the same as the order of the properties of the class.

```csharp
namespace Your.Namespace
{
    public class SalesDto
    {
        public int SalesTransId { get; set; }

        public string CoutryCode { get; set; }

        public long ProductId { get; set; }

        public decimal Value { get; set; }
    }
}
```

Initialize the instance of the IImportService in the class constructor where the conversion will take place. Then just call the service method passing its DTO class as the method's return type.

```csharp

namespace Any.Namespace
{
  public class SomeService : ISomeService
    {
        #region ' Properties '
        
        ...

        IImportService ImportService { get; }
        
        ...
        
        #endregion
        
        #region ' Constructors '
        
        public SomeService( ..., IImportService importService, ...)
        {
          ...
          ImportService = importService;
          ...
        }
        
        #endregion
        
        #region ' Methods '
        
        public async Task<ImportResponseMessage> ImportFileAsync(IFormFile requestMessage)
        {
           ...
           List<SalesDto> sales = await ImportService.ReadAsync<SalesDto>(requestMessage);
           ....
        }
}
```


### exporting

Exporting is as simple as the import process.

Initialize the IExporService instance in the class's constructor and then call the method passing as arguments the list of records you want to export and the characters that will be used as cell delimiters.

For now, only txt export is implemented and you can return the file as a stream or base64 encoded.

```csharp

namespace Other.Namespace
{
  public class AnotherService : IAnotherService
    {
        #region ' Properties '
        
        ...

        IExportService ExportService { get; }
        
        ...
        
        #endregion
        
        #region ' Constructors '
        
        public AnotherService( ..., IExportService ExportService, ...)
        {
          ...
          ExportService = exportService;
          ...
        }
        
        #endregion
        
        #region ' Methods '
        
        public async Task<string> GenerateFileAsync(List<Register> registers)
        {
           ...
           string sales = await ExportService.ExportToBase64TxtAsync(registers);
           ....
        }
}
```


## License [![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2Fleo-oliveira-eng%2Fexcel-file-service.svg?type=shield)](https://app.fossa.com/projects/git%2Bgithub.com%2Fleo-oliveira-eng%2Fexcel-file-service?ref=badge_shield)
The project is under MIT License, so it grants you permission to use, copy, and modify a piece of this software free of charge, as is, without restriction or warranty.

[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2Fleo-oliveira-eng%2Fexcel-file-service.svg?type=large)](https://app.fossa.com/projects/git%2Bgithub.com%2Fleo-oliveira-eng%2Fexcel-file-service?ref=badge_large)