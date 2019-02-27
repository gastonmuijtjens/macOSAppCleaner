# macOSAppCleaner
Small command line tool for cleaning up old application files after deleting of an application. Takes a keyword and searches for files or folders in the appropriate folders in the Library folder of the current user. 
In case a file or folder is found, the user has the options to delete it.

## System requirements
- macOS 10.14 or higher
- .NET Core SDK 2.2 or higher

## Installation
In order to build and publish this application, run the following commands in the given order:
- `dotnet restore`
- `dotnet build`
- `dotnet publish -r osx.10.14-x64 -c Release`

## License
- MIT

