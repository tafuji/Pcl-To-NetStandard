# PCL to .NET Standard

Visual Studio Extension for converting old portable class libraries to .NET Standard projects automatically.

![screenshot](https://raw.githubusercontent.com/tafuji/Pcl-To-NetStandard/master/Screenshots/Screenshot.png)

## Installation

T.B.D

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

```
The MIT License (MIT)

Copyright (c) 2018 Takeshi Fujimoto

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```
