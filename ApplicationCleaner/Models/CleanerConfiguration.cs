namespace ApplicationCleaner.Models
{
	public class CleanerConfiguration
	{
		public string LibraryFolder { get; set; }
		public string[] LibraryFolders { get; set; }
		public string RootLibraryFolder { get; set; }
		public string[] RootLibraryFolders { get; set; }
		public int KeywordLength { get; set; }
	}
}