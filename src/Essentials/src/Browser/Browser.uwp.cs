using System;
using System.Threading.Tasks;

namespace Microsoft.Maui.Essentials
{
	public static partial class Browser
	{
		static Task<bool> PlatformOpenAsync(Uri uri, BrowserLaunchOptions options) =>
			global::Windows.System.Launcher.LaunchUriAsync(uri).AsTask();
	}
}
