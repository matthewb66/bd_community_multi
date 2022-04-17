﻿using Microsoft.Maui.Graphics;

namespace Microsoft.Maui.DeviceTests.Stubs
{
	partial class SolidPaintStub : SolidPaint
	{
		public SolidPaintStub(Color color)
		{
			Color = color;
		}

#if IOS || __IOS__
		public CoreAnimation.CALayer ToCALayer(CoreGraphics.CGRect frame = default) =>
			new CoreAnimation.CALayer
			{
				ContentsGravity = CoreAnimation.CALayer.GravityResizeAspectFill,
				BackgroundColor = Color.ToCGColor(),
				Frame = frame,
			};
#elif __ANDROID__
		public Android.Graphics.Drawables.Drawable ToDrawable()
		{
			var drawable = new Microsoft.Maui.Graphics.MauiDrawable(Platform.DefaultContext);
			drawable.SetBackgroundColor(Color.ToNative());
			return drawable;
		}
#endif
	}
}