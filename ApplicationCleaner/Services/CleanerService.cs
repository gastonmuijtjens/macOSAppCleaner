using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApplicationCleaner.Models;
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
			Console.WriteLine("Welcome to the Application Cleaner!");
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
			var keyword = GetUserInput("Type in a keyword to search for...");
			return Search(keyword, token);
		}

		private async Task Search(string keyword, CancellationToken token)
		{
			var keywordLength = _config.KeywordLength;
			if (string.IsNullOrWhiteSpace(keyword) || keyword.Length < keywordLength)
			{
				Console.WriteLine($"Keyword must at least have a length of {keywordLength}");
				await Search(token);
				return;
			}

			foreach (var folder in _config.LibraryFolders)
			{
				await ListLibraryFiles(folder, keyword, token);
			}

			WriteSeparator();

			var answer = GetUserInput("Do you want to perform another search? (Y/n)");
			if (answer == "y")
			{
				WriteSeparator();
				await Search(token);
				return;
			}

			await StopAsync(token);
		}

		private async Task ListLibraryFiles(string folder, string keyword, CancellationToken token)
		{
			WriteSeparator();
			Console.WriteLine($"Matched files in {folder} folder:");
			var files = await SearchLibraryFiles(folder, keyword, token);
			ListFiles(files);
		}

		private static void ListFiles(IEnumerable<string> files)
		{
			foreach (var file in files)
			{
				Console.WriteLine(file);
				var choice = GetUserInput("Delete? (Y/n)");
				if (choice == "y")
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

		private static IEnumerable<string> SearchFiles(string folder, string keyword, CancellationToken token)
		{
			return Directory.EnumerateFileSystemEntries(folder)
				.Where(m => m.ToLower().Contains(keyword.ToLower()));
		}

		private async Task<IEnumerable<string>> SearchLibraryFiles(string subFolder, string keyword, CancellationToken token)
		{
			var homeFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			var libraryFolder = $"{homeFolder}/Library";
			if (!Directory.Exists(libraryFolder))
			{
				Console.WriteLine("The library folder does not exist. There is something wrong with the configuration on your Mac");
				await StopAsync(token);
				return new List<string>();
			}

			var searchFolder = $"{libraryFolder}/{subFolder}";
			return SearchFiles(searchFolder, keyword, token);
		}

		private static string GetUserInput(string text)
		{
			Console.WriteLine(text);
			return Console.ReadLine()?.ToLower();
		}

		private static void WriteSeparator()
		{
			Console.WriteLine("------");
		}
	}
}