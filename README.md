# PCL to .NET Standard Library

Visual Studio Extension for converting old portable class libraries to .NET Standard 2.0 projects automatically.

![screenshot](https://raw.githubusercontent.com/tafuji/Pcl-To-NetStandard/master/Screenshots/Screenshot.png)

## How to Install

### Automatic

You can install the extension via "Extensions and Updates" window on Visual Studio 2017.

### Manual

You can install the extension by downloading the VSIX file on isual Studio Marketplace.

- Please download the latest version of VSIX file form [PCL To .Net Standard](https://marketplace.visualstudio.com/items?itemName=tafuji.pcl2netstandard)
- Double click the VSIX file and install it.

## Usage

Right click on a portable class library, and select ```Convert to .NET Standard```.

## Remarks

- Please make sure you have a back up of your codes before using this extension.
- Package and project references are migrated from your portable projects. Please keep in mind that there might be some errors  after using this extension because NuGet packages in your projects are not supported by .NET Standard 2.0.

## Details

This extension provides the folowing features:

- Convert portable class library project to .NET Standard 2.0 project
    - Ii creates a new csproj file for .NET Standard 2.0
    - It migrates package references, which were referenced in portable class library, to new csproj file
    - It also migrates project references, which were used in portable class library, to new csproj file
- Backup files which were used only in portable project library
    - AssemblyInfo.cs and old version csproj file

## License

Please see the [License](https://github.com/tafuji/Pcl-To-NetStandard/blob/master/LICENSE).
