using System.Threading.Tasks;

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
	}
}