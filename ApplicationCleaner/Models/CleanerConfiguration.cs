namespace ApplicationCleaner.Models
{
	public class CleanerConfiguration
	{
		public string UserLibraryFolder { get; set; }
		public string[] UserLibraryFolders { get; set; }
		public string RootLibraryFolder { get; set; }
		public string[] RootLibraryFolders { get; set; }
		public int KeywordLength { get; set; }
	}
}