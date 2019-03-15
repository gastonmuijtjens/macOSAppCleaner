# macOSAppCleaner
Small command line tool for cleaning up old application files after deletion of an application on macOS. Takes a keyword and searches for files or folders in the appropriate folders in the Library folder of the current user. 
In case a file or folder is found, the application will provide the user with an option to delete it.

## System requirements
- macOS 10.14 or higher
- .NET Core SDK 2.2 or higher

## Configuration
The folders in which the application will search for files and folders can be configured by adding or removing entries in the `appsettings.json` file.

## Installation
In order to build and publish this application, run the following commands in the given order:
- `dotnet restore`
- `dotnet build`
- `dotnet publish -r osx.10.14-x64 -c Release`

## Usage
In order to run the application, just run `./ApplicationCleaner [keyword]` in the publish directory. The keyword is optional and can be specified as a second argument.

## License
- MIT

