# macOSAppCleaner
Small command line tool for cleaning up old application files after deletion of an application on macOS. This tool will search for these files with a provided keyword.

## System requirements
- macOS 10.14 or higher
- [.NET Core SDK 2.2](https://dotnet.microsoft.com/download) or higher

## Installation
In order to build and publish this application, run the following commands in the project folder in the given order:
- `dotnet restore`
- `dotnet build`
- `dotnet publish -r osx.10.14-x64 -c Release`

## Usage
To run the application after installation, execute `./ApplicationCleaner [keyword]` in the `publish` directory (defaults to `bin/Release/netcoreapp2.2/osx.10.14-x64/publish/`). The keyword (case insensitive) will be used to search for matching files or folders within the configured `Library` folders and must have a length of at least 3 by default. It is required to precede the command with `sudo` in order to search and delete files or folders located in the root `Library` folder.

## Example
An example concerning deleting the `GitHub Desktop` application would be:
- `sudo ./ApplicationCleaner github`

Note that if `sudo` is omitted, only the user `Library` folder will be searched.

## Configuration
The locations within the `Library` folders where the application will search for files and folders can be configured by adding or removing entries in the [appsettings.json](ApplicationCleaner/appsettings.json) file. It is possible to configure the required minimum length of the keyword as well.

## License
- [MIT](LICENSE)

