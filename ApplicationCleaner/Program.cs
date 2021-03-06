﻿using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ApplicationCleaner.Models;
using ApplicationCleaner.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ApplicationCleaner
{
	internal class Program
	{
		private const string AppSettingsName = "appsettings.json";

		private static Task Main(string[] args)
		{
			var provider = ConfigureServices(args);
			if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				var logger = provider.GetService<ILogger<Application>>();
				logger.LogError("This software only works on macOS. Exiting application...");
				Environment.Exit(1);
			}

			return provider.GetService<Application>().RunAsync();
		}

		private static IServiceProvider ConfigureServices(string[] args)
		{
			var configuration = BuildConfiguration();
			return new ServiceCollection()
				.AddOptions()
				.Configure<CleanerConfiguration>(configuration.GetSection("Configuration"))
				.AddLogging(opt => opt.AddConsole())
				.AddTransient<ICleanerService, CleanerService>()
				.AddTransient<Application>()
				.AddTransient<IUserInput>(instance => new UserInput
				{
					Arguments = args.ToList()
				})
				.BuildServiceProvider();
		}

		private static IConfiguration BuildConfiguration()
		{
			return new ConfigurationBuilder()
				.SetBasePath(AppSettingsPath)
				.AddJsonFile(AppSettingsName, false, true)
				.AddEnvironmentVariables()
				.Build();
		}

		private static string AppSettingsPath
		{
			get
			{
				var currentDirectory = Directory.GetCurrentDirectory();
				return Directory
					.GetFiles(currentDirectory)
					.Any(m => m.Contains(AppSettingsName))
					? currentDirectory
					: AppContext.BaseDirectory;
			}
		}
	}
}