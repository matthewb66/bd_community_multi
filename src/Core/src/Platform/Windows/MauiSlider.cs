﻿#nullable disable
using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using WImageSource = Microsoft.UI.Xaml.Media.ImageSource;

namespace Microsoft.Maui.Platform
{
	public class MauiSlider : Slider
	{
		public MauiSlider()
		{
			DefaultStyleKey = typeof(MauiSlider);
			ThumbColorOver = (Brush)Resources["SystemControlHighlightChromeAltLowBrush"];
		}

		public Brush ThumbColorOver
		{
			get { return (Brush)GetValue(ThumbColorOverProperty); }
			set { SetValue(ThumbColorOverProperty, value); }
		}

		public static readonly DependencyProperty ThumbColorOverProperty =
		DependencyProperty.Register(nameof(ThumbColorOver), typeof(Brush), typeof(MauiSlider), new PropertyMetadata(null));

		internal Thumb Thumb { get; set; }
		internal Thumb ImageThumb { get; set; }

		public static readonly DependencyProperty ThumbImageSourceProperty =
			DependencyProperty.Register(nameof(ThumbImageSource), typeof(WImageSource),
				typeof(MauiSlider), new PropertyMetadata(null, PropertyChangedCallback));

		static void PropertyChangedCallback(DependencyObject dependencyObject,
			DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			var slider = (MauiSlider)dependencyObject;
			SwapThumbs(slider);
		}

		static void SwapThumbs(MauiSlider slider)
		{
			if (slider.Thumb == null || slider.ImageThumb == null)
			{
				return;
			}

			if (slider.ThumbImageSource != null)
			{
				slider.Thumb.Visibility = UI.Xaml.Visibility.Collapsed;
				slider.ImageThumb.Visibility = UI.Xaml.Visibility.Visible;
			}
			else
			{
				slider.Thumb.Visibility = UI.Xaml.Visibility.Visible;
				slider.ImageThumb.Visibility = UI.Xaml.Visibility.Collapsed;
			}
		}

		public WImageSource ThumbImageSource
		{
			get { return (WImageSource)GetValue(ThumbImageSourceProperty); }
			set { SetValue(ThumbImageSourceProperty, value); }
		}

		internal event EventHandler Ready;

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			Thumb = GetTemplateChild("HorizontalThumb") as Thumb;
			ImageThumb = GetTemplateChild("HorizontalImageThumb") as Thumb;

			SwapThumbs(this);

			OnReady();
		}

		protected virtual void OnReady()
		{
			Ready?.Invoke(this, EventArgs.Empty);
		}
	}
}