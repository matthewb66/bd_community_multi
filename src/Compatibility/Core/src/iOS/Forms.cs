﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui.Controls.Internals;
using Foundation;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Animations;

#if __MOBILE__
using ObjCRuntime;
using UIKit;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;
using TNativeView = UIKit.UIView;
#else
using AppKit;
using Microsoft.Maui.Controls.Compatibility.Platform.MacOS;
using TNativeView = AppKit.NSView;
#endif

namespace Microsoft.Maui.Controls.Compatibility
{
	public struct InitializationOptions
	{
		public InitializationFlags Flags;
	}

	public static class Forms
	{
		internal static IMauiContext MauiContext { get; private set; }

		public static bool IsInitialized { get; private set; }

#if __MOBILE__
		static bool? s_isiOS9OrNewer;
		static bool? s_isiOS10OrNewer;
		static bool? s_isiOS11OrNewer;
		static bool? s_isiOS12OrNewer;
		static bool? s_isiOS13OrNewer;
		static bool? s_isiOS14OrNewer;
		static bool? s_isiOS15OrNewer;
		static bool? s_respondsTosetNeedsUpdateOfHomeIndicatorAutoHidden;

		internal static bool IsiOS9OrNewer
		{
			get
			{
				if (!s_isiOS9OrNewer.HasValue)
					s_isiOS9OrNewer = UIDevice.CurrentDevice.CheckSystemVersion(9, 0);
				return s_isiOS9OrNewer.Value;
			}
		}


		internal static bool IsiOS10OrNewer
		{
			get
			{
				if (!s_isiOS10OrNewer.HasValue)
					s_isiOS10OrNewer = UIDevice.CurrentDevice.CheckSystemVersion(10, 0);
				return s_isiOS10OrNewer.Value;
			}
		}

		internal static bool IsiOS11OrNewer
		{
			get
			{
				if (!s_isiOS11OrNewer.HasValue)
					s_isiOS11OrNewer = UIDevice.CurrentDevice.CheckSystemVersion(11, 0);
				return s_isiOS11OrNewer.Value;
			}
		}

		internal static bool IsiOS12OrNewer
		{
			get
			{
				if (!s_isiOS12OrNewer.HasValue)
					s_isiOS12OrNewer = UIDevice.CurrentDevice.CheckSystemVersion(12, 0);
				return s_isiOS12OrNewer.Value;
			}
		}

		internal static bool IsiOS13OrNewer
		{
			get
			{
				if (!s_isiOS13OrNewer.HasValue)
					s_isiOS13OrNewer = UIDevice.CurrentDevice.CheckSystemVersion(13, 0);
				return s_isiOS13OrNewer.Value;
			}
		}

		internal static bool IsiOS14OrNewer
		{
			get
			{
				if (!s_isiOS14OrNewer.HasValue)
					s_isiOS14OrNewer = UIDevice.CurrentDevice.CheckSystemVersion(14, 0);
				return s_isiOS14OrNewer.Value;
			}
		}

		internal static bool IsiOS15OrNewer
		{
			get
			{
				if (!s_isiOS15OrNewer.HasValue)
					s_isiOS15OrNewer = UIDevice.CurrentDevice.CheckSystemVersion(15, 0);
				return s_isiOS15OrNewer.Value;
			}
		}

		// Once we get essentials/cg converted to using startup.cs
		// we will delete all the renderer code inside this file
		internal static void RenderersRegistered()
		{
			IsInitializedRenderers = true;
		}

		internal static bool RespondsToSetNeedsUpdateOfHomeIndicatorAutoHidden
		{
			get
			{
				if (!s_respondsTosetNeedsUpdateOfHomeIndicatorAutoHidden.HasValue)
					s_respondsTosetNeedsUpdateOfHomeIndicatorAutoHidden = new UIViewController().RespondsToSelector(new ObjCRuntime.Selector("setNeedsUpdateOfHomeIndicatorAutoHidden"));
				return s_respondsTosetNeedsUpdateOfHomeIndicatorAutoHidden.Value;
			}
		}
#else
		static bool? s_isSierraOrNewer;

		internal static bool IsSierraOrNewer
		{
			get
			{
				if (!s_isSierraOrNewer.HasValue)
					s_isSierraOrNewer = NSProcessInfo.ProcessInfo.IsOperatingSystemAtLeastVersion(new NSOperatingSystemVersion(10, 12, 0));
				return s_isSierraOrNewer.Value;
			}
		}

		static bool? s_isHighSierraOrNewer;

		internal static bool IsHighSierraOrNewer
		{
			get
			{
				if (!s_isHighSierraOrNewer.HasValue)
					s_isHighSierraOrNewer = NSProcessInfo.ProcessInfo.IsOperatingSystemAtLeastVersion(new NSOperatingSystemVersion(10, 13, 0));
				return s_isHighSierraOrNewer.Value;
			}
		}

		static bool? s_isMojaveOrNewer;

		internal static bool IsMojaveOrNewer
		{
			get
			{
				if (!s_isMojaveOrNewer.HasValue)
					s_isMojaveOrNewer = NSProcessInfo.ProcessInfo.IsOperatingSystemAtLeastVersion(new NSOperatingSystemVersion(10, 14, 0));
				return s_isMojaveOrNewer.Value;
			}
		}

#endif

		public static bool IsInitializedRenderers { get; private set; }

		public static void Init(IActivationState activationState, InitializationOptions? options = null) =>
			SetupInit(activationState.Context, options);

		static void SetupInit(IMauiContext context, InitializationOptions? maybeOptions = null)
		{
			MauiContext = context;

			Microsoft.Maui.Controls.Internals.Registrar.RegisterRendererToHandlerShim(RendererToHandlerShim.CreateShim);

			Application.AccentColor = Color.FromRgba(50, 79, 133, 255);

#if __MOBILE__
			Device.SetIdiom(UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? TargetIdiom.Tablet : TargetIdiom.Phone);
			Device.SetFlowDirection(UIApplication.SharedApplication.UserInterfaceLayoutDirection.ToFlowDirection());
#else
			if (!IsInitialized)
			{
				// Only need to do this once
				// Subscribe to notifications in OS Theme changes
				NSDistributedNotificationCenter.GetDefaultCenter().AddObserver((NSString)"AppleInterfaceThemeChangedNotification", (n) =>
				{
					var interfaceStyle = NSUserDefaults.StandardUserDefaults.StringForKey("AppleInterfaceStyle");

					var aquaAppearance = NSAppearance.GetAppearance(interfaceStyle == "Dark" ? NSAppearance.NameDarkAqua : NSAppearance.NameAqua);
					NSApplication.SharedApplication.Appearance = aquaAppearance;

					Application.Current?.TriggerThemeChanged(new AppThemeChangedEventArgs(interfaceStyle == "Dark" ? OSAppTheme.Dark : OSAppTheme.Light));
				});
			}

			Device.SetIdiom(TargetIdiom.Desktop);
			Device.SetFlowDirection(NSApplication.SharedApplication.UserInterfaceLayoutDirection.ToFlowDirection());

			if (IsMojaveOrNewer)
			{
				var interfaceStyle = NSUserDefaults.StandardUserDefaults.StringForKey("AppleInterfaceStyle");
				var aquaAppearance = NSAppearance.GetAppearance(interfaceStyle == "Dark" ? NSAppearance.NameDarkAqua : NSAppearance.NameAqua);
				NSApplication.SharedApplication.Appearance = aquaAppearance;
			}
#endif
			var platformServices = new IOSPlatformServices();

			Device.PlatformServices = platformServices;

#if __MOBILE__
			Device.PlatformInvalidator = platformServices;
#endif
			if (maybeOptions?.Flags.HasFlag(InitializationFlags.SkipRenderers) != true)
				RegisterCompatRenderers();

			ExpressionSearch.Default = new iOSExpressionSearch();

			IsInitialized = true;
		}

		internal static void RegisterCompatRenderers()
		{
			if (!IsInitializedRenderers)
			{
				IsInitializedRenderers = true;

				// Only need to do this once
				Controls.Internals.Registrar.RegisterAll(new[]
				{
					typeof(ExportRendererAttribute),
					typeof(ExportCellAttribute),
					typeof(ExportImageSourceHandlerAttribute),
					typeof(ExportFontAttribute)
				});
			}
		}

		public static event EventHandler<ViewInitializedEventArgs> ViewInitialized;

		internal static void SendViewInitialized(this VisualElement self, TNativeView nativeView)
		{
			ViewInitialized?.Invoke(self, new ViewInitializedEventArgs { View = self, NativeView = nativeView });
		}

		class iOSExpressionSearch : ExpressionVisitor, IExpressionSearch
		{
			List<object> _results;
			Type _targetType;

			public List<T> FindObjects<T>(Expression expression) where T : class
			{
				_results = new List<object>();
				_targetType = typeof(T);
				Visit(expression);
				return _results.Select(o => o as T).ToList();
			}

			protected override Expression VisitMember(MemberExpression node)
			{
				if (node.Expression is ConstantExpression && node.Member is FieldInfo)
				{
					var container = ((ConstantExpression)node.Expression).Value;
					var value = ((FieldInfo)node.Member).GetValue(container);

					if (_targetType.IsInstanceOfType(value))
						_results.Add(value);
				}
				return base.VisitMember(node);
			}
		}

		class IOSPlatformServices : IPlatformServices
#if __MOBILE__
			, IPlatformInvalidate
#endif
		{
			readonly double _fontScalingFactor = 1;
			public IOSPlatformServices()
			{
#if __MOBILE__
				//The standard accessibility size for a font is 17, we can get a
				//close approximation to the new Size by multiplying by this scale factor
				_fontScalingFactor = (double)UIFont.PreferredBody.PointSize / 17f;
#endif
			}

			public Assembly[] GetAssemblies()
			{
				return AppDomain.CurrentDomain.GetAssemblies();
			}

			public double GetNamedSize(NamedSize size, Type targetElementType, bool useOldSizes)
			{
				// We make these up anyway, so new sizes didn't really change
				// iOS docs say default button font size is 15, default label font size is 17 so we use those as the defaults.
				var scalingFactor = _fontScalingFactor;

				if (Application.Current?.On<PlatformConfiguration.iOS>().GetEnableAccessibilityScalingForNamedFontSizes() == false)
				{
					scalingFactor = 1;
				}

				switch (size)
				{
					//We multiply the fonts by the scale factor, and cast to an int, to make them whole numbers.
					case NamedSize.Default:
						return (int)((typeof(Button).IsAssignableFrom(targetElementType) ? 15 : 17) * scalingFactor);
					case NamedSize.Micro:
						return (int)(12 * scalingFactor);
					case NamedSize.Small:
						return (int)(14 * scalingFactor);
					case NamedSize.Medium:
						return (int)(17 * scalingFactor);
					case NamedSize.Large:
						return (int)(22 * scalingFactor);
#if __IOS__
					case NamedSize.Body:
						return (double)UIFont.PreferredBody.PointSize;
					case NamedSize.Caption:
						return (double)UIFont.PreferredCaption1.PointSize;
					case NamedSize.Header:
						return (double)UIFont.PreferredHeadline.PointSize;
					case NamedSize.Subtitle:
						return (double)UIFont.PreferredTitle2.PointSize;
					case NamedSize.Title:
						return (double)UIFont.PreferredTitle1.PointSize;
#else
					case NamedSize.Body:
						return 23;
					case NamedSize.Caption:
						return 18;
					case NamedSize.Header:
						return 23;
					case NamedSize.Subtitle:
						return 28;
					case NamedSize.Title:
						return 34;

#endif
					default:
						throw new ArgumentOutOfRangeException("size");
				}
			}

			public Color GetNamedColor(string name)
			{
#if __IOS__
				UIColor resultColor = null;

				// If not iOS 13, but 11+ we can only get the named colors
				if (!IsiOS13OrNewer && IsiOS11OrNewer)
					return (resultColor = UIColor.FromName(name)) == null ? null : resultColor.ToColor();

				// If iOS 13+ check all dynamic colors too
				switch (name)
				{
					case NamedPlatformColor.Label:
						resultColor = UIColor.LabelColor;
						break;
					case NamedPlatformColor.Link:
						resultColor = UIColor.LinkColor;
						break;
					case NamedPlatformColor.OpaqueSeparator:
						resultColor = UIColor.OpaqueSeparatorColor;
						break;
					case NamedPlatformColor.PlaceholderText:
						resultColor = UIColor.PlaceholderTextColor;
						break;
					case NamedPlatformColor.QuaternaryLabel:
						resultColor = UIColor.QuaternaryLabelColor;
						break;
					case NamedPlatformColor.SecondaryLabel:
						resultColor = UIColor.SecondaryLabelColor;
						break;
					case NamedPlatformColor.Separator:
						resultColor = UIColor.SeparatorColor;
						break;
					case NamedPlatformColor.SystemBlue:
						resultColor = UIColor.SystemBlueColor;
						break;
					case NamedPlatformColor.SystemGray:
						resultColor = UIColor.SystemGrayColor;
						break;
					case NamedPlatformColor.SystemGray2:
						resultColor = UIColor.SystemGray2Color;
						break;
					case NamedPlatformColor.SystemGray3:
						resultColor = UIColor.SystemGray3Color;
						break;
					case NamedPlatformColor.SystemGray4:
						resultColor = UIColor.SystemGray4Color;
						break;
					case NamedPlatformColor.SystemGray5:
						resultColor = UIColor.SystemGray5Color;
						break;
					case NamedPlatformColor.SystemGray6:
						resultColor = UIColor.SystemGray6Color;
						break;
					case NamedPlatformColor.SystemGreen:
						resultColor = UIColor.SystemGreenColor;
						break;
					case NamedPlatformColor.SystemIndigo:
						resultColor = UIColor.SystemIndigoColor;
						break;
					case NamedPlatformColor.SystemOrange:
						resultColor = UIColor.SystemOrangeColor;
						break;
					case NamedPlatformColor.SystemPink:
						resultColor = UIColor.SystemPinkColor;
						break;
					case NamedPlatformColor.SystemPurple:
						resultColor = UIColor.SystemPurpleColor;
						break;
					case NamedPlatformColor.SystemRed:
						resultColor = UIColor.SystemRedColor;
						break;
					case NamedPlatformColor.SystemTeal:
						resultColor = UIColor.SystemTealColor;
						break;
					case NamedPlatformColor.SystemYellow:
						resultColor = UIColor.SystemYellowColor;
						break;
					case NamedPlatformColor.TertiaryLabel:
						resultColor = UIColor.TertiaryLabelColor;
						break;
					default:
						resultColor = UIColor.FromName(name);
						break;
				}

				if (resultColor == null)
					return null;

				return resultColor.ToColor();
#elif __MACOS__

				NSColor resultColor = null;

				switch (name)
				{
					case NamedPlatformColor.AlternateSelectedControlTextColor:
						resultColor = NSColor.AlternateSelectedControlText;
							break;
					case NamedPlatformColor.ControlAccent:
						if (IsMojaveOrNewer)
							resultColor = NSColor.ControlAccentColor;
						break;
					case NamedPlatformColor.ControlBackgroundColor:
						resultColor = NSColor.ControlBackground;
						break;
					case NamedPlatformColor.ControlColor:
						resultColor = NSColor.Control;
						break;
					case NamedPlatformColor.ControlTextColor:
						resultColor = NSColor.ControlText;
						break;
					case NamedPlatformColor.DisabledControlTextColor:
						resultColor = NSColor.DisabledControlText;
						break;
					case NamedPlatformColor.FindHighlightColor:
						if (IsHighSierraOrNewer)
							resultColor = NSColor.FindHighlightColor;
						break;
					case NamedPlatformColor.GridColor:
						resultColor = NSColor.Grid;
						break;
					case NamedPlatformColor.HeaderTextColor:
						resultColor = NSColor.HeaderText;
						break;
					case NamedPlatformColor.HighlightColor:
						resultColor = NSColor.Highlight;
						break;
					case NamedPlatformColor.KeyboardFocusIndicatorColor:
						resultColor = NSColor.KeyboardFocusIndicator;
						break;
					case NamedPlatformColor.LabelColor:
						resultColor = NSColor.LabelColor;
						break;
					case NamedPlatformColor.LinkColor:
						resultColor = NSColor.LinkColor;
						break;
					case NamedPlatformColor.PlaceholderTextColor:
						resultColor = NSColor.PlaceholderTextColor;
						break;
					case NamedPlatformColor.QuaternaryLabelColor:
						resultColor = NSColor.QuaternaryLabelColor;
						break;
					case NamedPlatformColor.SecondaryLabelColor:
						resultColor = NSColor.SecondaryLabelColor;
						break;
					case NamedPlatformColor.SelectedContentBackgroundColor:
						resultColor = NSColor.SelectedContentBackgroundColor;
						break;
					case NamedPlatformColor.SelectedControlColor:
						resultColor = NSColor.SelectedControl;
						break;
					case NamedPlatformColor.SelectedControlTextColor:
						resultColor = NSColor.SelectedControlText;
						break;
					case NamedPlatformColor.SelectedMenuItemTextColor:
						resultColor = NSColor.SelectedMenuItemText;
						break;
					case NamedPlatformColor.SelectedTextBackgroundColor:
						resultColor = NSColor.SelectedTextBackground;
						break;
					case NamedPlatformColor.SelectedTextColor:
						resultColor = NSColor.SelectedText;
						break;
					case NamedPlatformColor.SeparatorColor:
						resultColor = NSColor.SeparatorColor;
						break;
					case NamedPlatformColor.ShadowColor:
						resultColor = NSColor.Shadow;
						break;
					case NamedPlatformColor.TertiaryLabelColor:
						resultColor = NSColor.TertiaryLabelColor;
						break;
					case NamedPlatformColor.TextBackgroundColor:
						resultColor = NSColor.TextBackground;
						break;
					case NamedPlatformColor.TextColor:
						resultColor = NSColor.Text;
						break;
					case NamedPlatformColor.UnderPageBackgroundColor:
						resultColor = NSColor.UnderPageBackgroundColor;
						break;
					case NamedPlatformColor.UnemphasizedSelectedContentBackgroundColor:
						if (IsMojaveOrNewer)
							resultColor = NSColor.UnemphasizedSelectedContentBackgroundColor;
						break;
					case NamedPlatformColor.UnemphasizedSelectedTextBackgroundColor:
						if (IsMojaveOrNewer)
							resultColor = NSColor.UnemphasizedSelectedTextBackgroundColor;
						break;
					case NamedPlatformColor.UnemphasizedSelectedTextColor:
						if (IsMojaveOrNewer)
							resultColor = NSColor.UnemphasizedSelectedTextColor;
						break;
					case NamedPlatformColor.WindowBackgroundColor:
						resultColor = NSColor.WindowBackground;
						break;
					case NamedPlatformColor.WindowFrameTextColor:
						resultColor = NSColor.WindowFrameText;
						break;
					case NamedPlatformColor.Label:
						resultColor = NSColor.LabelColor;
						break;
					case NamedPlatformColor.Link:
						resultColor = NSColor.LinkColor;
						break;
					case NamedPlatformColor.PlaceholderText:
						resultColor = NSColor.PlaceholderTextColor;
						break;
					case NamedPlatformColor.QuaternaryLabel:
						resultColor = NSColor.QuaternaryLabelColor;
						break;
					case NamedPlatformColor.SecondaryLabel:
						resultColor = NSColor.SecondaryLabelColor;
						break;
					case NamedPlatformColor.Separator:
						if (IsMojaveOrNewer)
							resultColor = NSColor.SeparatorColor;
						break;
					case NamedPlatformColor.SystemBlue:
						resultColor = NSColor.SystemBlueColor;
						break;
					case NamedPlatformColor.SystemGray:
						resultColor = NSColor.SystemGrayColor;
						break;
					case NamedPlatformColor.SystemGreen:
						resultColor = NSColor.SystemGreenColor;
						break;
					case NamedPlatformColor.SystemIndigo:
						resultColor = NSColor.SystemIndigoColor;
						break;
					case NamedPlatformColor.SystemOrange:
						resultColor = NSColor.SystemOrangeColor;
						break;
					case NamedPlatformColor.SystemPink:
						resultColor = NSColor.SystemPinkColor;
						break;
					case NamedPlatformColor.SystemPurple:
						resultColor = NSColor.SystemPurpleColor;
						break;
					case NamedPlatformColor.SystemRed:
						resultColor = NSColor.SystemRedColor;
						break;
					case NamedPlatformColor.SystemTeal:
						resultColor = NSColor.SystemTealColor;
						break;
					case NamedPlatformColor.SystemYellow:
						resultColor = NSColor.SystemYellowColor;
						break;
					case NamedPlatformColor.TertiaryLabel:
						resultColor = NSColor.TertiaryLabelColor;
						break;
					default:
						resultColor = NSColor.FromName(name);
						break;
				}

				if (resultColor == null)
					return null;

				return resultColor.ToColor(NSColorSpace.GenericRGBColorSpace);
#else
				return null;
#endif
			}

			public async Task<Stream> GetStreamAsync(Uri uri, CancellationToken cancellationToken)
			{
				using (var client = GetHttpClient())
				{
					// Do not remove this await otherwise the client will dispose before
					// the stream even starts
					var result = await StreamWrapper.GetStreamAsync(uri, cancellationToken, client).ConfigureAwait(false);

					return result;
				}
			}

#if __MOBILE__
			public string RuntimePlatform => Device.iOS;
#else
			public string RuntimePlatform => Device.macOS;
#endif

			public void StartTimer(TimeSpan interval, Func<bool> callback)
			{
				NSTimer timer = NSTimer.CreateRepeatingTimer(interval, t =>
				{
					if (!callback())
						t.Invalidate();
				});
				NSRunLoop.Main.AddTimer(timer, NSRunLoopMode.Common);
			}

			HttpClient GetHttpClient()
			{
				var proxy = CoreFoundation.CFNetwork.GetSystemProxySettings();
				var handler = new HttpClientHandler();
				if (!string.IsNullOrEmpty(proxy.HTTPProxy))
				{
					handler.Proxy = CoreFoundation.CFNetwork.GetDefaultProxy();
					handler.UseProxy = true;
				}
				return new HttpClient(handler);
			}

			static int Hex(int v)
			{
				if (v < 10)
					return '0' + v;
				return 'a' + v - 10;
			}

#if !__MOBILE__
			public void QuitApplication()
			{
				NSApplication.SharedApplication.Terminate(new NSObject());
			}
#endif

			public SizeRequest GetNativeSize(VisualElement view, double widthConstraint, double heightConstraint)
			{
#if __MOBILE__
				return Platform.iOS.Platform.GetNativeSize(view, widthConstraint, heightConstraint);
#else
				return Platform.MacOS.Platform.GetNativeSize(view, widthConstraint, heightConstraint);
#endif
			}

			public OSAppTheme RequestedTheme
			{
				get
				{
#if __IOS__ || __TVOS__
					if (!IsiOS13OrNewer)
						return OSAppTheme.Unspecified;
					var uiStyle = GetCurrentUIViewController()?.TraitCollection?.UserInterfaceStyle ??
						UITraitCollection.CurrentTraitCollection.UserInterfaceStyle;

					switch (uiStyle)
					{
						case UIUserInterfaceStyle.Light:
							return OSAppTheme.Light;
						case UIUserInterfaceStyle.Dark:
							return OSAppTheme.Dark;
						default:
							return OSAppTheme.Unspecified;
					};
#else
					return AppearanceIsDark() ? OSAppTheme.Dark : OSAppTheme.Light;
#endif
				}
			}

#if __MACOS__
			bool AppearanceIsDark()
			{
				if (IsMojaveOrNewer)
				{
					var appearance = NSApplication.SharedApplication.EffectiveAppearance;
					var matchedAppearance = appearance.FindBestMatch(new string[] { NSAppearance.NameAqua, NSAppearance.NameDarkAqua });

					return matchedAppearance == NSAppearance.NameDarkAqua;
				}
				else
				{
					return false;
				}
			}
#endif

#if __IOS__ || __TVOS__

			static UIViewController GetCurrentUIViewController() =>
				GetCurrentViewController(false);

			static UIViewController GetCurrentViewController(bool throwIfNull = true)
			{
				UIViewController viewController = null;

				var window = UIApplication.SharedApplication.GetKeyWindow();

				if (window != null && window.WindowLevel == UIWindowLevel.Normal)
					viewController = window.RootViewController;

				if (viewController == null)
				{
					window = UIApplication.SharedApplication
						.Windows
						.OrderByDescending(w => w.WindowLevel)
						.FirstOrDefault(w => w.RootViewController != null && w.WindowLevel == UIWindowLevel.Normal);

					if (window == null && throwIfNull)
						throw new InvalidOperationException("Could not find current view controller.");
					else
						viewController = window?.RootViewController;
				}

				while (viewController?.PresentedViewController != null)
					viewController = viewController.PresentedViewController;

				if (throwIfNull && viewController == null)
					throw new InvalidOperationException("Could not find current view controller.");

				return viewController;
			}

			public void Invalidate(VisualElement visualElement)
			{
				var renderer = Platform.iOS.Platform.GetRenderer(visualElement);

				if (renderer == null)
				{
					return;
				}

				renderer.NativeView.SetNeedsLayout();
			}
#endif
		}
	}
}