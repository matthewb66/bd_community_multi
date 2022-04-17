﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Util;
using Android.Views;
using AndroidX.Core.Content;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Controls.DualScreen.Android;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Graphics;
using AColor = Android.Graphics.Color;
using AndroidResource = Android.Resource;
using Size = Microsoft.Maui.Graphics.Size;
using Trace = System.Diagnostics.Trace;

namespace Microsoft.Maui.Controls.Compatibility
{
	public struct InitializationOptions
	{
		public struct EffectScope
		{
			public string Name;
			public ExportEffectAttribute[] Effects;
		}

		public InitializationOptions(Context activity, Bundle bundle, Assembly resourceAssembly)
		{
			this = default(InitializationOptions);
			Activity = activity;
			Bundle = bundle;
			ResourceAssembly = resourceAssembly;
		}
		public Context Activity;
		public Bundle Bundle;
		public Assembly ResourceAssembly;
		public HandlerAttribute[] Handlers;
		public EffectScope[] EffectScopes;
		public InitializationFlags Flags;
	}

	public static class Forms
	{
		const int TabletCrossover = 600;

		// One per process; does not change, suitable for loading resources (e.g., ResourceProvider)
		internal static Context ApplicationContext { get; private set; } = global::Android.App.Application.Context;
		internal static IMauiContext MauiContext { get; private set; }

		public static bool IsInitialized { get; private set; }

		static bool _ColorButtonNormalSet;
		static Color _ColorButtonNormal = null;
		public static Color ColorButtonNormalOverride { get; set; }

		internal static readonly bool IsMarshmallowOrNewer = OperatingSystem.IsAndroidVersionAtLeast((int)BuildVersionCodes.M);

		internal static readonly bool IsNougatOrNewer = OperatingSystem.IsAndroidVersionAtLeast((int)BuildVersionCodes.N);

		public static float GetFontSizeNormal(Context context)
		{
			float size = 50;

			// Android 5.0+
			//this doesn't seem to work
			using (var value = new TypedValue())
			{
				if (context.Theme.ResolveAttribute(AndroidResource.Attribute.TextSize, value, true))
				{
					size = value.Data;
				}
			}

			return size;
		}

		public static Color GetColorButtonNormal(Context context)
		{
			if (!_ColorButtonNormalSet)
			{
				_ColorButtonNormal = GetButtonColor(context);
				_ColorButtonNormalSet = true;
			}

			return _ColorButtonNormal;
		}

		public static void Init(IActivationState activationState, InitializationOptions? options = null) =>
			Init(activationState.Context, options);

		public static void Init(IMauiContext context, InitializationOptions? options = null)
		{
			Assembly resourceAssembly;

			Profile.FrameBegin("Assembly.GetCallingAssembly");
			resourceAssembly = Assembly.GetCallingAssembly();
			Profile.FrameEnd("Assembly.GetCallingAssembly");

			Profile.FrameBegin();
			SetupInit(context, resourceAssembly, options);
			Profile.FrameEnd();
		}

		public static void Init(IMauiContext context, Assembly resourceAssembly)
		{
			Profile.FrameBegin();
			SetupInit(context, resourceAssembly, null);
			Profile.FrameEnd();
		}

		public static void SetTitleBarVisibility(Activity activity, AndroidTitleBarVisibility visibility)
		{
			if (visibility == AndroidTitleBarVisibility.Never)
			{
				if (!activity.Window.Attributes.Flags.HasFlag(WindowManagerFlags.Fullscreen))
					activity.Window.AddFlags(WindowManagerFlags.Fullscreen);
			}
			else
			{
				if (activity.Window.Attributes.Flags.HasFlag(WindowManagerFlags.Fullscreen))
					activity.Window.ClearFlags(WindowManagerFlags.Fullscreen);
			}
		}

		public static event EventHandler<ViewInitializedEventArgs> ViewInitialized;

		internal static void SendViewInitialized(this VisualElement self, global::Android.Views.View nativeView)
		{
			EventHandler<ViewInitializedEventArgs> viewInitialized = ViewInitialized;
			if (viewInitialized != null)
				viewInitialized(self, new ViewInitializedEventArgs { View = self, NativeView = nativeView });
		}

		static bool IsInitializedRenderers;

		// Once we get essentials/cg converted to using startup.cs
		// we will delete all the renderer code inside this file
		internal static void RenderersRegistered()
		{
			IsInitializedRenderers = true;
		}

		internal static void RegisterCompatRenderers(InitializationOptions? maybeOptions)
		{
			if (!IsInitializedRenderers)
			{
				IsInitializedRenderers = true;
				if (maybeOptions.HasValue)
				{
					var options = maybeOptions.Value;
					var handlers = options.Handlers;
					var flags = options.Flags;
					var effectScopes = options.EffectScopes;

					//TODO: ExportCell?
					//TODO: ExportFont

					// renderers
					Registrar.RegisterRenderers(handlers);

					// effects
					if (effectScopes != null)
					{
						for (var i = 0; i < effectScopes.Length; i++)
						{
							var effectScope = effectScopes[0];
							Registrar.RegisterEffects(effectScope.Name, effectScope.Effects);
						}
					}

					// css
					Registrar.RegisterStylesheets(flags);
				}
				else
				{
					// Only need to do this once
					Registrar.RegisterAll(new[] {
						typeof(ExportRendererAttribute),
						typeof(ExportCellAttribute),
						typeof(ExportImageSourceHandlerAttribute),
						typeof(ExportFontAttribute)
					});
				}
			}
		}

		static void SetupInit(
			IMauiContext context,
			Assembly resourceAssembly,
			InitializationOptions? maybeOptions = null
		)
		{
			var activity = context.Context;
			Profile.FrameBegin();
			Registrar.RegisterRendererToHandlerShim(RendererToHandlerShim.CreateShim);

			// Allow this multiple times to support the app and then the activity
			ApplicationContext = activity.ApplicationContext;
			MauiContext = context;

			if (!IsInitialized)
			{
				// Only need to do this once
				Profile.FramePartition("ResourceManager.Init");
				ResourceManager.Init(resourceAssembly);
			}

			Profile.FramePartition("Color.SetAccent()");
			// We want this to be updated when we have a new activity (e.g. on a configuration change)
			// This could change if the UI mode changes (e.g., if night mode is enabled)
			Application.AccentColor = GetAccentColor(activity);
			_ColorButtonNormalSet = false;

			// We want this to be updated when we have a new activity (e.g. on a configuration change)
			// because AndroidPlatformServices needs a current activity to launch URIs from
			Profile.FramePartition("Device.PlatformServices");

			var androidServices = new AndroidPlatformServices(activity);

			Device.PlatformServices = androidServices;
			Device.PlatformInvalidator = androidServices;

			Profile.FramePartition("RegisterAll");

			if (maybeOptions?.Flags.HasFlag(InitializationFlags.SkipRenderers) != true)
				RegisterCompatRenderers(maybeOptions);

			Profile.FramePartition("Epilog");

			var currentIdiom = TargetIdiom.Unsupported;

			// First try UIModeManager
			using (var uiModeManager = UiModeManager.FromContext(ApplicationContext))
			{
				try
				{
					var uiMode = uiModeManager?.CurrentModeType ?? UiMode.TypeUndefined;
					currentIdiom = DetectIdiom(uiMode);
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine($"Unable to detect using UiModeManager: {ex.Message}");
				}
			}

			// Then try Configuration
			if (TargetIdiom.Unsupported == currentIdiom)
			{
				var configuration = activity.Resources.Configuration;

				if (configuration != null)
				{
					var minWidth = configuration.SmallestScreenWidthDp;
					var isWide = minWidth >= TabletCrossover;
					currentIdiom = isWide ? TargetIdiom.Tablet : TargetIdiom.Phone;
				}
				else
				{
					// Start clutching at straws
					var metrics = activity.Resources?.DisplayMetrics;

					if (metrics != null)
					{
						var minSize = Math.Min(metrics.WidthPixels, metrics.HeightPixels);
						var isWide = minSize * metrics.Density >= TabletCrossover;
						currentIdiom = isWide ? TargetIdiom.Tablet : TargetIdiom.Phone;
					}
				}
			}

			Device.SetIdiom(currentIdiom);
			Device.SetFlowDirection(activity.Resources.Configuration.LayoutDirection.ToFlowDirection());

			if (ExpressionSearch.Default == null)
				ExpressionSearch.Default = new AndroidExpressionSearch();

			IsInitialized = true;
			Profile.FrameEnd();
		}

		static TargetIdiom DetectIdiom(UiMode uiMode)
		{
			var returnValue = TargetIdiom.Unsupported;
			if (uiMode == UiMode.TypeNormal)
				returnValue = TargetIdiom.Unsupported;
			else if (uiMode == UiMode.TypeTelevision)
				returnValue = TargetIdiom.TV;
			else if (uiMode == UiMode.TypeDesk)
				returnValue = TargetIdiom.Desktop;
			else if (uiMode == UiMode.TypeWatch)
				returnValue = TargetIdiom.Watch;

			Device.SetIdiom(returnValue);
			return returnValue;
		}

		static Color GetAccentColor(Context context)
		{
			Color rc;
			using (var value = new TypedValue())
			{
				if (context.Theme.ResolveAttribute(global::Android.Resource.Attribute.ColorAccent, value, true)) // Android 5.0+
				{
					rc = Color.FromUint((uint)value.Data);
				}
				else if (context.Theme.ResolveAttribute(context.Resources.GetIdentifier("colorAccent", "attr", context.PackageName), value, true))  // < Android 5.0
				{
					rc = Color.FromUint((uint)value.Data);
				}
				else                    // fallback to old code if nothing works (don't know if that ever happens)
				{
					// Hardcoded because could not get color from the theme drawable
					// Holo dark light blue
					rc = Color.FromArgb("#ff33b5e5");
				}
			}
			return rc;
		}

		static Color GetButtonColor(Context context)
		{
			Color rc = ColorButtonNormalOverride;

			if (ColorButtonNormalOverride == null)
			{
				using (var value = new TypedValue())
				{
					if (context.Theme.ResolveAttribute(global::Android.Resource.Attribute.ColorButtonNormal, value, true)) // Android 5.0+
					{
						rc = Color.FromUint((uint)value.Data);
					}
					else if (context.Theme.ResolveAttribute(context.Resources.GetIdentifier("colorButtonNormal", "attr", context.PackageName), value, true))  // < Android 5.0
					{
						rc = Color.FromUint((uint)value.Data);
					}
				}
			}
			return rc;
		}

		class AndroidExpressionSearch : ExpressionVisitor, IExpressionSearch
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
					object container = ((ConstantExpression)node.Expression).Value;
					object value = ((FieldInfo)node.Member).GetValue(container);

					if (_targetType.IsInstanceOfType(value))
						_results.Add(value);
				}
				return base.VisitMember(node);
			}
		}

		class AndroidPlatformServices : IPlatformServices, IPlatformInvalidate
		{
			double _buttonDefaultSize;
			double _editTextDefaultSize;
			double _labelDefaultSize;
			double _largeSize;
			double _mediumSize;

			double _microSize;
			double _smallSize;

			readonly Context _context;

			public AndroidPlatformServices(Context context)
			{
				_context = context;
			}

			public Assembly[] GetAssemblies()
			{
				return AppDomain.CurrentDomain.GetAssemblies();
			}

			public double GetNamedSize(NamedSize size, Type targetElementType, bool useOldSizes)
			{
				if (_smallSize == 0)
				{
					_smallSize = ConvertTextAppearanceToSize(AndroidResource.Attribute.TextAppearanceSmall, AndroidResource.Style.TextAppearanceDeviceDefaultSmall, 12);
					_mediumSize = ConvertTextAppearanceToSize(AndroidResource.Attribute.TextAppearanceMedium, AndroidResource.Style.TextAppearanceDeviceDefaultMedium, 14);
					_largeSize = ConvertTextAppearanceToSize(AndroidResource.Attribute.TextAppearanceLarge, AndroidResource.Style.TextAppearanceDeviceDefaultLarge, 18);
					_buttonDefaultSize = ConvertTextAppearanceToSize(AndroidResource.Attribute.TextAppearanceButton, AndroidResource.Style.TextAppearanceDeviceDefaultWidgetButton, 14);
					_editTextDefaultSize = ConvertTextAppearanceToSize(AndroidResource.Style.TextAppearanceWidgetEditText, AndroidResource.Style.TextAppearanceDeviceDefaultWidgetEditText, 18);
					_labelDefaultSize = _smallSize;
					// as decreed by the android docs, ALL HAIL THE ANDROID DOCS, ALL GLORY TO THE DOCS, PRAISE HYPNOTOAD
					_microSize = Math.Max(1, _smallSize - (_mediumSize - _smallSize));
				}

				if (useOldSizes)
				{
					switch (size)
					{
						case NamedSize.Default:
							if (typeof(Button).IsAssignableFrom(targetElementType))
								return _buttonDefaultSize;
							if (typeof(Label).IsAssignableFrom(targetElementType))
								return _labelDefaultSize;
							if (typeof(Editor).IsAssignableFrom(targetElementType) || typeof(Entry).IsAssignableFrom(targetElementType) || typeof(SearchBar).IsAssignableFrom(targetElementType))
								return _editTextDefaultSize;
							return 14;
						case NamedSize.Micro:
							return 10;
						case NamedSize.Small:
							return 12;
						case NamedSize.Medium:
							return 14;
						case NamedSize.Large:
							return 18;
						case NamedSize.Body:
							return 16;
						case NamedSize.Caption:
							return 12;
						case NamedSize.Header:
							return 96;
						case NamedSize.Subtitle:
							return 16;
						case NamedSize.Title:
							return 24;
						default:
							throw new ArgumentOutOfRangeException("size");
					}
				}
				switch (size)
				{
					case NamedSize.Default:
						if (typeof(Button).IsAssignableFrom(targetElementType))
							return _buttonDefaultSize;
						if (typeof(Label).IsAssignableFrom(targetElementType))
							return _labelDefaultSize;
						if (typeof(Editor).IsAssignableFrom(targetElementType) || typeof(Entry).IsAssignableFrom(targetElementType))
							return _editTextDefaultSize;
						return _mediumSize;
					case NamedSize.Micro:
						return _microSize;
					case NamedSize.Small:
						return _smallSize;
					case NamedSize.Medium:
						return _mediumSize;
					case NamedSize.Large:
						return _largeSize;
					case NamedSize.Body:
						return 16;
					case NamedSize.Caption:
						return 12;
					case NamedSize.Header:
						return 96;
					case NamedSize.Subtitle:
						return 16;
					case NamedSize.Title:
						return 24;
					default:
						throw new ArgumentOutOfRangeException("size");
				}
			}

			public Color GetNamedColor(string name)
			{
				int color;
				switch (name)
				{
					case NamedPlatformColor.BackgroundDark:
						color = ContextCompat.GetColor(_context, AndroidResource.Color.BackgroundDark);
						break;
					case NamedPlatformColor.BackgroundLight:
						color = ContextCompat.GetColor(_context, AndroidResource.Color.BackgroundLight);
						break;
					case NamedPlatformColor.Black:
						color = ContextCompat.GetColor(_context, AndroidResource.Color.Black);
						break;
					case NamedPlatformColor.DarkerGray:
						color = ContextCompat.GetColor(_context, AndroidResource.Color.DarkerGray);
						break;
					case NamedPlatformColor.HoloBlueBright:
						color = ContextCompat.GetColor(_context, AndroidResource.Color.HoloBlueBright);
						break;
					case NamedPlatformColor.HoloBlueDark:
						color = ContextCompat.GetColor(_context, AndroidResource.Color.HoloBlueDark);
						break;
					case NamedPlatformColor.HoloBlueLight:
						color = ContextCompat.GetColor(_context, AndroidResource.Color.HoloBlueLight);
						break;
					case NamedPlatformColor.HoloGreenDark:
						color = ContextCompat.GetColor(_context, AndroidResource.Color.HoloGreenDark);
						break;
					case NamedPlatformColor.HoloGreenLight:
						color = ContextCompat.GetColor(_context, AndroidResource.Color.HoloGreenLight);
						break;
					case NamedPlatformColor.HoloOrangeDark:
						color = ContextCompat.GetColor(_context, AndroidResource.Color.HoloOrangeDark);
						break;
					case NamedPlatformColor.HoloOrangeLight:
						color = ContextCompat.GetColor(_context, AndroidResource.Color.HoloOrangeLight);
						break;
					case NamedPlatformColor.HoloPurple:
						color = ContextCompat.GetColor(_context, AndroidResource.Color.HoloPurple);
						break;
					case NamedPlatformColor.HoloRedDark:
						color = ContextCompat.GetColor(_context, AndroidResource.Color.HoloRedDark);
						break;
					case NamedPlatformColor.HoloRedLight:
						color = ContextCompat.GetColor(_context, AndroidResource.Color.HoloRedLight);
						break;
					case NamedPlatformColor.TabIndicatorText:
						color = ContextCompat.GetColor(_context, AndroidResource.Color.TabIndicatorText);
						break;
					case NamedPlatformColor.Transparent:
						color = ContextCompat.GetColor(_context, AndroidResource.Color.Transparent);
						break;
					case NamedPlatformColor.White:
						color = ContextCompat.GetColor(_context, AndroidResource.Color.White);
						break;
					case NamedPlatformColor.WidgetEditTextDark:
						color = ContextCompat.GetColor(_context, AndroidResource.Color.WidgetEditTextDark);
						break;
					default:
						return null;
				}

				if (color != 0)
					return new AColor(color).ToColor();

				return null;
			}

			public string RuntimePlatform => Device.Android;

			public void StartTimer(TimeSpan interval, Func<bool> callback)
			{
				var handler = new Handler(Looper.MainLooper);
				handler.PostDelayed(() =>
				{
					if (callback())
						StartTimer(interval, callback);

					handler.Dispose();
					handler = null;
				}, (long)interval.TotalMilliseconds);
			}

			double ConvertTextAppearanceToSize(int themeDefault, int deviceDefault, double defaultValue)
			{
				double myValue;

				if (TryGetTextAppearance(themeDefault, out myValue) && myValue > 0)
					return myValue;
				if (TryGetTextAppearance(deviceDefault, out myValue) && myValue > 0)
					return myValue;
				return defaultValue;
			}

			static int Hex(int v)
			{
				if (v < 10)
					return '0' + v;
				return 'a' + v - 10;
			}

			bool TryGetTextAppearance(int appearance, out double val)
			{
				val = 0;
				try
				{
					using (var value = new TypedValue())
					{
						if (_context.Theme.ResolveAttribute(appearance, value, true))
						{
							var textSizeAttr = new[] { AndroidResource.Attribute.TextSize };
							const int indexOfAttrTextSize = 0;
							using (TypedArray array = _context.ObtainStyledAttributes(value.Data, textSizeAttr))
							{
								val = _context.FromPixels(array.GetDimensionPixelSize(indexOfAttrTextSize, -1));
								return true;
							}
						}
					}
				}
				catch (Exception ex)
				{
					Application.Current?.FindMauiContext()?.CreateLogger<AndroidPlatformServices>()?
						.LogWarning(ex, "Error retrieving text appearance");
				}
				return false;
			}

			public SizeRequest GetNativeSize(VisualElement view, double widthConstraint, double heightConstraint)
			{
				return Platform.Android.Platform.GetNativeSize(view, widthConstraint, heightConstraint);
			}

			public void Invalidate(VisualElement visualElement)
			{
				var renderer = visualElement.GetRenderer();
				if (renderer == null || renderer.View.IsDisposed())
				{
					return;
				}

				renderer.View.Invalidate();
				renderer.View.RequestLayout();
			}

			public OSAppTheme RequestedTheme
			{
				get
				{
					var nightMode = _context.Resources.Configuration.UiMode & UiMode.NightMask;
					switch (nightMode)
					{
						case UiMode.NightYes:
							return OSAppTheme.Dark;
						case UiMode.NightNo:
							return OSAppTheme.Light;
						default:
							return OSAppTheme.Unspecified;
					};
				}
			}
		}
	}
}
