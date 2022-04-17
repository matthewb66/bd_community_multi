using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents;

namespace Microsoft.Maui.Controls.Hosting
{
	public static partial class AppHostBuilderExtensions
	{
		internal static MauiAppBuilder ConfigureCompatibilityLifecycleEvents(this MauiAppBuilder builder) =>
			   builder.ConfigureLifecycleEvents(events => events.AddAndroid(OnConfigureLifeCycle));

		static void OnConfigureLifeCycle(IAndroidLifecycleBuilder android)
		{
			android
				.OnApplicationCreating((app) =>
				{
					// This is the initial Init to set up any system services registered by
					// Forms.Init(). This happens in the Application's OnCreate - before
					// any UI has appeared.
					// This creates a dummy MauiContext that wraps the Application.

					var services = MauiApplication.Current.Services;
					var mauiContext = new MauiContext(services, app);
					var state = new ActivationState(mauiContext);
					Forms.Init(state, new InitializationOptions { Flags = InitializationFlags.SkipRenderers });
				})
				.OnMauiContextCreated((mauiContext) =>
				{
					// This is the final Init that sets up the real context from the activity.

					var state = new ActivationState(mauiContext);
					Forms.Init(state);
				});
		}
	}
}
