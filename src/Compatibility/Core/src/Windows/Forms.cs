using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.Maui.Controls.Compatibility.Platform.UWP;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Graphics;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel.Activation;
using WSolidColorBrush = Microsoft.UI.Xaml.Media.SolidColorBrush;

namespace Microsoft.Maui.Controls.Compatibility
{
	public struct InitializationOptions
	{
		public InitializationOptions(UI.Xaml.LaunchActivatedEventArgs args)
		{
			this = default(InitializationOptions);
			LaunchActivatedEventArgs = args;
		}
		public UI.Xaml.LaunchActivatedEventArgs LaunchActivatedEventArgs;
		public InitializationFlags Flags;
	}

	public static partial class Forms
	{
		//TODO WINUI3 This is set by main page currently because
		// it's only a single window
		public static UI.Xaml.Window MainWindow { get; set; }

		public static bool IsInitialized { get; private set; }

		public static IMauiContext MauiContext { get; private set; }

		public static void Init(IActivationState state, InitializationOptions? options = null)
		{
			SetupInit(state.Context, state.Context.GetOptionalNativeWindow(), maybeOptions: options);
		}

		static void SetupInit(
			IMauiContext mauiContext,
			UI.Xaml.Window mainWindow,
			IEnumerable<Assembly> rendererAssemblies = null,
			InitializationOptions? maybeOptions = null)
		{
			MauiContext = mauiContext;
			Registrar.RegisterRendererToHandlerShim(RendererToHandlerShim.CreateShim);

			var accent = (WSolidColorBrush)Microsoft.UI.Xaml.Application.Current.Resources["SystemColorControlAccentBrush"];
			KnownColor.SetAccent(accent.ToColor());

			Device.SetIdiom(TargetIdiom.Tablet);
			Device.SetFlowDirection(mauiContext.GetFlowDirection());

			//TODO WINUI3
			//switch (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily)
			//{
			//	case "Windows.Desktop":
			//		if (Windows.UI.ViewManagement.UIViewSettings.GetForCurrentView().UserInteractionMode ==
			//			Windows.UI.ViewManagement.UserInteractionMode.Touch)
			//			Device.SetIdiom(TargetIdiom.Tablet);
			//		else
			//			Device.SetIdiom(TargetIdiom.Desktop);
			//		break;
			//	case "Windows.Mobile":
			//		Device.SetIdiom(TargetIdiom.Phone);
			//		break;
			//	case "Windows.Xbox":
			//		Device.SetIdiom(TargetIdiom.TV);
			//		break;
			//	default:
			//		Device.SetIdiom(TargetIdiom.Unsupported);
			//		break;
			//}

			ExpressionSearch.Default = new WindowsExpressionSearch();

			Registrar.ExtraAssemblies = rendererAssemblies?.ToArray();

			var platformServices = new WindowsPlatformServices();

			Device.PlatformServices = platformServices;
			Device.PlatformInvalidator = platformServices;

			if (mainWindow != null)
			{
				MainWindow = mainWindow;

				//if (mainWindow is WindowsBasePage windowsPage)
				//{
				//	windowsPage.LoadApplication(windowsPage.CreateApplication());
				//	windowsPage.Activate();
				//}
			}

			IsInitialized = true;
		}
	}
}