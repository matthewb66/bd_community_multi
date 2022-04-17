using Android.Content;
using Android.Views;
using AndroidX.Core.Content;
using Microsoft.Maui.Graphics;
using AColor = Android.Graphics.Color;
using AColorRes = Android.Resource.Color;
using AView = Android.Views.View;

namespace Microsoft.Maui.Controls.Platform
{
	internal class ShellPageContainer : ViewGroup
	{
		static int? DarkBackground;
		static int? LightBackground;


		public IViewHandler Child { get; set; }

		public bool IsInFragment { get; set; }

		public ShellPageContainer(Context context, INativeViewHandler child, bool inFragment = false) : base(context)
		{
			Child = child;
			IsInFragment = inFragment;
			if (child.VirtualView.Background == null)
			{
				int color;
				if (ShellView.IsDarkTheme)
					color = DarkBackground ??= ContextCompat.GetColor(context, AColorRes.BackgroundDark);
				else
					color = LightBackground ??= ContextCompat.GetColor(context, AColorRes.BackgroundLight);

				child.NativeView.SetBackgroundColor(new AColor(color));
			}
			child.NativeView.RemoveFromParent();
			AddView(child.NativeView);
		}

		protected override void OnLayout(bool changed, int l, int t, int r, int b)
		{
			var width = r - l;
			var height = b - t;

			if (Child.NativeView is AView aView)
				aView.Layout(0, 0, width, height);
		}

		protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
		{
			if (Child.NativeView is AView aView)
			{
				aView.Measure(widthMeasureSpec, heightMeasureSpec);
				SetMeasuredDimension(aView.MeasuredWidth, aView.MeasuredHeight);
			}
		}
	}
}