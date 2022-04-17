﻿using Android.Content.Res;
using Microsoft.Maui.Graphics;
using AProgressBar = Android.Widget.ProgressBar;

namespace Microsoft.Maui.Platform
{
	public static class ProgressBarExtensions
	{
		public const int Maximum = 10000;

		public static void UpdateProgress(this AProgressBar nativeProgressBar, IProgress progress)
		{
			nativeProgressBar.Progress = (int)(progress.Progress * Maximum);
		}

		public static void UpdateProgressColor(this AProgressBar nativeProgressBar, IProgress progress)
		{
			Color color = progress.ProgressColor;

			if (color == null)
			{
				(nativeProgressBar.Indeterminate ? nativeProgressBar.IndeterminateDrawable :
					nativeProgressBar.ProgressDrawable)?.ClearColorFilter();
			}
			else
			{
				var tintList = ColorStateList.ValueOf(color.ToNative());

				if (nativeProgressBar.Indeterminate)
					nativeProgressBar.IndeterminateTintList = tintList;
				else
					nativeProgressBar.ProgressTintList = tintList;
			}
		}
	}
}