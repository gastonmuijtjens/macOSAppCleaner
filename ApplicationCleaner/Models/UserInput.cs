using System.Collections.Generic;

namespace ApplicationCleaner.Models
{
	public interface IUserInput
	{
		IEnumerable<string> Arguments { get; set; }
	}

	public class UserInput : IUserInput
	{
		public IEnumerable<string> Arguments { get; set; }
	}
}