# macOSAppCleaner
One of the abilities that the macOS operation system currently lacks is a user-friendly way to delete applications from the system without leaving unnecessary files behind. A lot of files often remain after simply dragging an application to the trash can, leaving clutter and occupying unnecessary space. Manually searching for those files is a time-consuming and potentially risky process. This tool aims to solve this problem by providing an easy-to-use command line interface for searching and optionally removing these files.

## System requirements
- macOS 10.15 or higher
- [.NET Core SDK 3.1](https://dotnet.microsoft.com/download) or higher

## Installation
To build and publish, run the following commands within the project folder respectively:
- `dotnet restore`
- `dotnet build`
- `dotnet publish -r osx.10.15-x64 -c Release`

## Usage
To run the application after installation, execute `./ApplicationCleaner [keyword]` in the `publish` directory (defaults to `bin/Release/netcoreapp3.1/osx.10.15-x64/publish/`). The keyword (case insensitive) will be used to search for matching files or folders within the configured `Library` folders and must have a minimum length of at least 3 by default. It is required to precede the command with `sudo` in order to search and delete files or folders located in the root `Library` folder.

If any matching file or folder is found, the user will be prompted to delete it. Press `Y` to accept and `N` to cancel and the application will continue to execute.

## Example
As an example, run the following command to search for files associated with the `GitHub Desktop` application:
- `sudo ./ApplicationCleaner github`

Note that if `sudo` is omitted, only the user `Library` folder will be searched.

## Configuration
The locations within the `Library` folders where the application will search for files and folders can be configured by adding or removing entries in the [appsettings.json](ApplicationCleaner/appsettings.json) file. It is possible to configure the required minimum length of the keyword as well.

## License
- [MIT](LICENSE)