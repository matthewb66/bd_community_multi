﻿using System;
#if __IOS__ || MACCATALYST
using NativeImage = UIKit.UIImage;
using NativeImageView = UIKit.UIImageView;
using NativeView = UIKit.UIButton;
#elif MONOANDROID
using NativeImage = Android.Graphics.Drawables.Drawable;
using NativeImageView = Android.Widget.ImageView;
using NativeView = AndroidX.AppCompat.Widget.AppCompatImageButton;
#elif WINDOWS
using NativeImage = Microsoft.UI.Xaml.Media.ImageSource;
using NativeImageView = Microsoft.UI.Xaml.Controls.Image;
using NativeView = Microsoft.UI.Xaml.FrameworkElement;
#elif NETSTANDARD || (NET6_0 && !IOS && !ANDROID)
using NativeImage = System.Object;
using NativeImageView = System.Object;
using NativeView = System.Object;
#endif

namespace Microsoft.Maui.Handlers
{
	public partial class ImageButtonHandler : IImageButtonHandler
	{
		public static IPropertyMapper<IImage, IImageHandler> ImageMapper = new PropertyMapper<IImage, IImageHandler>(ImageHandler.Mapper);
		public static IPropertyMapper<IImageButton, IImageButtonHandler> Mapper = new PropertyMapper<IImageButton, IImageButtonHandler>(ImageMapper)
		{
#if WINDOWS
			[nameof(IImageButton.Background)] = MapBackground,
#endif
		};

		ImageSourcePartLoader? _imageSourcePartLoader;
		public ImageSourcePartLoader SourceLoader =>
			_imageSourcePartLoader ??= new ImageSourcePartLoader(this, () => VirtualView, OnSetImageSource);

		public ImageButtonHandler() : base(Mapper)
		{
		}

		public ImageButtonHandler(IPropertyMapper mapper) : base(mapper ?? Mapper)
		{
		}

		IImageButton IImageButtonHandler.TypedVirtualView => VirtualView;

		IImage IImageHandler.TypedVirtualView => VirtualView;

		NativeImageView IImageHandler.TypedNativeView =>
#if __IOS__
			NativeView.ImageView;
#elif WINDOWS
			NativeView.GetContent<NativeImageView>() ?? throw new InvalidOperationException("ImageButton did not contain an Image element.");
#else
			NativeView;
#endif
		ImageSourcePartLoader IImageHandler.SourceLoader => SourceLoader;
	}
}