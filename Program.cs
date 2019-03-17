using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ApplicationCleaner
{
	internal class Program
	{
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
				.SetBasePath(AppContext.BaseDirectory)
				.AddJsonFile("appsettings.json", false, true)
				.AddEnvironmentVariables()
				.Build();
		}
	}
}
