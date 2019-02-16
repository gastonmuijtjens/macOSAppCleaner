using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace ApplicationCleaner
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				Console.WriteLine("This software only works on macOS as of now");
				ExitApplication();
			}

			Console.WriteLine("Welcome to the Application Cleaner!");
			var keyword = args.FirstOrDefault();
			if (keyword == null)
			{
				Search();
			}
			else
			{
				Search(keyword);
			}
		}

		private static void Search(string keyword)
		{
			if (string.IsNullOrWhiteSpace(keyword) || keyword.Length < 3)
			{
				Console.WriteLine("Invalid keyword, try again");
				Search();
				return;
			}

			ListFiles("Application Support", keyword);
			ListFiles("Application Scripts", keyword);
			ListFiles("Caches", keyword);
			ListFiles("Containers", keyword);
			ListFiles("Group Containers", keyword);
			ListFiles("LaunchAgents", keyword);
			ListFiles("Logs", keyword);
			ListFiles("Preferences", keyword);
			ListFiles("Saved Application State", keyword);
			WriteSeperator();

			var answer = GetUserInput("Do you want to perform another search? y/n");
			if (answer == "y")
			{
				WriteSeperator();
				Search();
			}
		}

		private static void Search()
		{
			var keyword = GetUserInput("Type in a keyword to search for...");
			Search(keyword);
		}

		private static void ListFiles(string folder, string keyword)
		{
			WriteSeperator();
			Console.WriteLine($"Matched files in {folder} folder:");
			var files = SearchFiles(folder, keyword);
			foreach (var file in files)
			{
				Console.WriteLine(file);
				var choice = GetUserInput("Delete? y/n");
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

		private static IEnumerable<string> SearchFiles(string folder, string keyword)
		{
			var homeFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			var libraryFolder = $"{homeFolder}/Library";
			if (!Directory.Exists(libraryFolder))
			{
				Console.WriteLine("The library folder does not exist. There is something wrong with the configuration on your Mac");
				ExitApplication();
			}

			var searchFolder = $"{libraryFolder}/{folder}";
			return Directory.EnumerateFileSystemEntries(searchFolder)
				.Where(m => m.ToLower().Contains(keyword.ToLower()));
		}

		private static string GetUserInput(string text)
		{
			Console.WriteLine(text);
			return Console.ReadLine().ToLower();
		}

		private static void WriteSeperator()
		{
			Console.WriteLine("------");
		}

		private static void ExitApplication()
		{
			Environment.Exit(-1);
		}
	}
}
