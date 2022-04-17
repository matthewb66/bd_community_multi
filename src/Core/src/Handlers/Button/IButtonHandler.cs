﻿#if __IOS__ || MACCATALYST
using NativeView = UIKit.UIButton;
#elif MONOANDROID
using NativeView = Google.Android.Material.Button.MaterialButton;
#elif WINDOWS
using NativeView = Microsoft.UI.Xaml.Controls.Button;
#elif NETSTANDARD || (NET6_0 && !IOS && !ANDROID)
using NativeView = System.Object;
#endif

namespace Microsoft.Maui.Handlers
{
	public partial interface IButtonHandler : IViewHandler
	{
		IButton TypedVirtualView { get; }
		NativeView TypedNativeView { get; }
		ImageSourcePartLoader ImageSourceLoader { get; }
	}
}