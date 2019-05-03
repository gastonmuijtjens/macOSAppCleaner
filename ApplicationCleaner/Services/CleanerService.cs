using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApplicationCleaner.Models;
using ApplicationCleaner.Utils;
using Microsoft.Extensions.Options;

namespace ApplicationCleaner.Services
{
	public interface ICleanerService
	{
		Task ExecuteAsync(CancellationToken token = default);
		Task StopAsync(CancellationToken token = default);
	}

	public class CleanerService : ICleanerService
	{
		private readonly CleanerConfiguration _config;
		private readonly IUserInput _userInput;

		public CleanerService(IOptions<CleanerConfiguration> options, IUserInput userInput)
		{
			_config = options.Value;
			_userInput = userInput;
		}

		public Task ExecuteAsync(CancellationToken token = default)
		{
			var arguments = _userInput.Arguments.ToList();
			return arguments.Any() ? Search(arguments.First(), token) : Search(token);
		}

		public Task StopAsync(CancellationToken token = default)
		{
			Console.WriteLine("Stopping application...");
			return Task.CompletedTask;
		}

		private Task Search(CancellationToken token)
		{
			var keyword = GetUserInput("Type in a keyword to search for: ");
			return Search(keyword, token);
		}

		private async Task Search(string keyword, CancellationToken token)
		{
			var keywordLength = _config.KeywordLength;
			if (string.IsNullOrWhiteSpace(keyword) || keyword.Length < keywordLength)
			{
				WriteConsoleError($"Keyword must have a length of at least {keywordLength}");
				WriteConsoleSeparator();
				await Search(token);
				return;
			}

			await SearchUserLibraryFolders(keyword, token);
			WriteConsoleSeparator();

			var doSearchSystemFolders = PromptUser("Do you want to search in the root Library folder as well? [Y/n] ");
			if (doSearchSystemFolders)
			{
				WriteConsoleSeparator();
				await SearchRootLibraryFolders(keyword, token);
			}

			var doPerformAnotherSearch = PromptUser("Do you want to perform another search? [Y/n] ");
			if (doPerformAnotherSearch)
			{
				WriteConsoleSeparator();
				await Search(token);
				return;
			}

			await StopAsync(token);
		}

		private async Task SearchUserLibraryFolders(string keyword, CancellationToken token)
		{
			foreach (var folder in _config.UserLibraryFolders)
			{
				await ListUserLibraryFiles(folder, keyword, token);
			}
		}

		private async Task SearchRootLibraryFolders(string keyword, CancellationToken token)
		{
			if (!UnixUtils.IsRoot)
			{
				WriteConsoleError("In order to search into system folders, this command has to be run as root or with 'sudo'. Please rerun the application and try again.");
				return;
			}

			foreach (var folder in _config.RootLibraryFolders)
			{
				await ListRootLibraryFiles(folder, keyword, token);
			}
		}

		private async Task ListUserLibraryFiles(string folder, string keyword, CancellationToken token)
		{
			var files = await SearchUserLibraryFiles(folder, keyword, token);
			ListLibraryFiles(files, folder);
		}

		private async Task ListRootLibraryFiles(string folder, string keyword, CancellationToken token)
		{
			var files = await SearchRootLibraryFiles(folder, keyword, token);
			ListLibraryFiles(files, folder);
		}

		private static void ListLibraryFiles(IEnumerable<string> files, string folder)
		{
			WriteConsoleSeparator();
			Console.WriteLine($"Matching files in \u001b[35m\u001b[1m{folder}\u001b[0m\u001b[0m folder:");
			ListFiles(files);
		}

		private static void ListFiles(IEnumerable<string> files)
		{
			foreach (var file in files)
			{
				Console.WriteLine(file);
				var shouldDelete = PromptUser("Delete? [Y/n] ");
				if (shouldDelete)
				{
					var fileAttributes = File.GetAttributes(file);
					if (fileAttributes.HasFlag(FileAttributes.Directory))
					{
						Directory.Delete(file, true);
					}
					else
					{
						File.Delete(file);
					}

					Console.WriteLine("File or folder was deleted");
				}
			}
		}

		private static IEnumerable<string> SearchFiles(string keyword, string folder)
		{
			return Directory.EnumerateFileSystemEntries(folder)
				.Where(m => m.ToLower().Contains(keyword.ToLower()));
		}

		private static IEnumerable<string> SearchFiles(string keyword, params string[] folderPaths)
		{
			var searchFolder = Path.Combine(folderPaths);
			return SearchFiles(keyword, searchFolder);
		}

		private async Task<IEnumerable<string>> SearchUserLibraryFiles(string subFolder, string keyword, CancellationToken token)
		{
			var homeFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			var libraryFolder = Path.Combine(homeFolder, _config.UserLibraryFolder);
			if (Directory.Exists(libraryFolder))
			{
				return SearchFiles(keyword, libraryFolder, subFolder);
			}

			return await ShowLibraryConfigurationError($"The Library folder of the current user does not exist at {libraryFolder}. Please change the configuration and try again.", token);
		}

		private async Task<IEnumerable<string>> SearchRootLibraryFiles(string subFolder, string keyword, CancellationToken token)
		{
			var rootLibraryFolder = Path.Combine("/", _config.RootLibraryFolder);
			if (Directory.Exists(rootLibraryFolder))
			{
				return SearchFiles(keyword, rootLibraryFolder, subFolder);
			}

			return await ShowLibraryConfigurationError($"The root Library folder does not exist at {rootLibraryFolder}. Please change the configuration and try again.", token);
		}

		private async Task<IEnumerable<string>> ShowLibraryConfigurationError(string message, CancellationToken token)
		{
			WriteConsoleError(message);
			await StopAsync(token);
			return new List<string>();
		}

		private static string GetUserInput(string text)
		{
			WriteConsoleQuestion(text);
			return Console.ReadLine()?.ToLower();
		}

		private static bool PromptUser()
		{
			var answer = Console.ReadKey().Key;
			ConsoleKey key = ConsoleKey.A;
			while (key != ConsoleKey.Enter)
			{
				key = Console.ReadKey(true).Key;
				if (key == ConsoleKey.Backspace)
				{
					Console.Write("\b \b");
					return PromptUser();
				}
			}

			Console.WriteLine();
			return answer == ConsoleKey.Y;
		}
		
		private static bool PromptUser(string text)
		{
			WriteConsoleQuestion(text);
			return PromptUser();
		}

		private static void WriteConsoleSeparator()
		{
			Console.WriteLine("------");
		}

		private static void WriteConsoleQuestion(string text)
		{
			Console.Write($"\u001b[1m{text}\u001b[0m");
		}

		private static void WriteConsoleError(string message)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(message);
			Console.ResetColor();
		}
	}
}