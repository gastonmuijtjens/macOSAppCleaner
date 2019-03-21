using System.Threading.Tasks;
using ApplicationCleaner.Services;

namespace ApplicationCleaner
{
	public class Application
	{
		private readonly ICleanerService _service;

		public Application(ICleanerService service)
		{
			_service = service;
		}

		public Task RunAsync()
		{
			return _service.ExecuteAsync();
		}

		public Task StopAsync()
		{
			return _service.StopAsync();
		}
	}
}