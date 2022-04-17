﻿using System.Threading.Tasks;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Widget;

namespace Microsoft.Maui.Platform
{
	public static class SliderExtensions
	{
		public const double NativeMaxValue = int.MaxValue;

		public static void UpdateMinimum(this SeekBar seekBar, ISlider slider) => UpdateValue(seekBar, slider);

		public static void UpdateMaximum(this SeekBar seekBar, ISlider slider) => UpdateValue(seekBar, slider);

		public static void UpdateValue(this SeekBar seekBar, ISlider slider)
		{
			var min = slider.Minimum;
			var max = slider.Maximum;
			var value = slider.Value;

			seekBar.Progress = (int)((value - min) / (max - min) * NativeMaxValue);
		}

		public static void UpdateMinimumTrackColor(this SeekBar seekBar, ISlider slider) =>
			UpdateMinimumTrackColor(seekBar, slider, null, null);

		public static void UpdateMinimumTrackColor(this SeekBar seekBar, ISlider slider, ColorStateList? defaultProgressTintList, PorterDuff.Mode? defaultProgressTintMode)
		{
			if (slider.MinimumTrackColor == null)
			{
				if (defaultProgressTintList != null)
					seekBar.ProgressTintList = defaultProgressTintList;

				if (defaultProgressTintMode != null)
					seekBar.ProgressTintMode = defaultProgressTintMode;
			}
			else
			{
				seekBar.ProgressTintList = ColorStateList.ValueOf(slider.MinimumTrackColor.ToNative());
				seekBar.ProgressTintMode = PorterDuff.Mode.SrcIn;
			}
		}

		public static void UpdateMaximumTrackColor(this SeekBar seekBar, ISlider slider) =>
			UpdateMaximumTrackColor(seekBar, slider, null, null);

		public static void UpdateMaximumTrackColor(this SeekBar seekBar, ISlider slider, ColorStateList? defaultProgressBackgroundTintList, PorterDuff.Mode? defaultProgressBackgroundTintMode)
		{
			if (slider.MaximumTrackColor == null)
			{
				if (defaultProgressBackgroundTintList != null)
					seekBar.ProgressBackgroundTintList = defaultProgressBackgroundTintList;

				if (defaultProgressBackgroundTintMode != null)
					seekBar.ProgressBackgroundTintMode = defaultProgressBackgroundTintMode;
			}
			else
			{
				seekBar.ProgressBackgroundTintList = ColorStateList.ValueOf(slider.MaximumTrackColor.ToNative());
				seekBar.ProgressBackgroundTintMode = PorterDuff.Mode.SrcIn;
			}
		}

		public static void UpdateThumbColor(this SeekBar seekBar, ISlider slider) =>
			UpdateThumbColor(seekBar, slider);

		public static void UpdateThumbColor(this SeekBar seekBar, ISlider slider, ColorFilter? defaultThumbColorFilter) =>
			seekBar.Thumb?.SetColorFilter(slider.ThumbColor, FilterMode.SrcIn, defaultThumbColorFilter);

		public static async Task UpdateThumbImageSourceAsync(this SeekBar seekBar, ISlider slider, IImageSourceServiceProvider provider, Drawable? defaultThumb)
		{
			var context = seekBar.Context;

			if (context == null)
				return;

			var thumbImageSource = slider.ThumbImageSource;

			if (thumbImageSource != null)
			{
				var service = provider.GetRequiredImageSourceService(thumbImageSource);
				var result = await service.GetDrawableAsync(thumbImageSource, context);
				Drawable? thumbDrawable = result?.Value;

				if (seekBar.IsAlive())
					seekBar.SetThumb(thumbDrawable ?? defaultThumb);
			}
		}
	}
}