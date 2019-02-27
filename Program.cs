using System;
using System.IO;
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
			var provider = ConfigureServices();
			if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				var logger = provider.GetService<ILogger<Application>>();
				logger.LogError("This software only works on macOS. Exiting application...");
				Environment.Exit(1);
			}

			return provider.GetService<Application>().RunAsync();
		}

		private static IServiceProvider ConfigureServices()
		{
			var configuration = BuildConfiguration();
			var services = new ServiceCollection();
			services.AddOptions();
			services.Configure<CleanerConfiguration>(configuration.GetSection("Configuration"));
			services.AddLogging(opt => opt.AddConsole());
			services.AddTransient<ICleanerService, CleanerService>();
			services.AddTransient<Application>();
			
			return services.BuildServiceProvider();
		}

		private static IConfiguration BuildConfiguration() {
			return new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", false, true)
				.AddEnvironmentVariables()
				.Build();
		}
	}
}
