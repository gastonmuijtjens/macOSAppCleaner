using System.Runtime.InteropServices;
using Mono.Unix.Native;

namespace ApplicationCleaner.Utils
{
	public static class UnixUtils
	{
		public static bool IsRoot => (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) && Syscall.getuid() == 0;
	}
}