﻿using System.Threading.Tasks;
using Android.Graphics.Drawables;
using Android.Widget;
using AndroidX.AppCompat.Widget;

namespace Microsoft.Maui.Handlers
{
	public partial class ImageHandler : ViewHandler<IImage, ImageView>
	{
		protected override ImageView CreateNativeView() => new AppCompatImageView(Context);

		protected override void DisconnectHandler(ImageView nativeView)
		{
			base.DisconnectHandler(nativeView);
			SourceLoader.Reset();
		}

		public override bool NeedsContainer =>
			VirtualView?.Background != null ||
			base.NeedsContainer;

		public static void MapBackground(IImageHandler handler, IImage image)
		{
			handler.UpdateValue(nameof(IViewHandler.ContainerView));

			handler.GetWrappedNativeView()?.UpdateBackground(image);
		}

		public static void MapAspect(IImageHandler handler, IImage image) =>
			handler.TypedNativeView?.UpdateAspect(image);

		public static void MapIsAnimationPlaying(IImageHandler handler, IImage image) =>
			handler.TypedNativeView?.UpdateIsAnimationPlaying(image);

		public static void MapSource(IImageHandler handler, IImage image) =>
			MapSourceAsync(handler, image).FireAndForget(handler);

		public static Task MapSourceAsync(IImageHandler handler, IImage image)
		{
			handler.TypedNativeView.Clear();
			return handler.SourceLoader.UpdateImageSourceAsync();
		}

		void OnSetImageSource(Drawable? obj)
		{
			NativeView.SetImageDrawable(obj);
		}
	}
}