using Microsoft.Maui.Graphics;

namespace Microsoft.Maui.Controls
{
	public partial class FontImageSource : ImageSource
	{
		public override bool IsEmpty => string.IsNullOrEmpty(Glyph);

		public static readonly BindableProperty ColorProperty = BindableProperty.Create(nameof(Color), typeof(Color), typeof(FontImageSource), default(Color),
			propertyChanged: (b, o, n) => ((FontImageSource)b).OnSourceChanged());

		public Color Color
		{
			get => (Color)GetValue(ColorProperty);
			set => SetValue(ColorProperty, value);
		}

		public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(FontImageSource), default(string),
			propertyChanged: (b, o, n) => ((FontImageSource)b).OnSourceChanged());

		public string FontFamily
		{
			get => (string)GetValue(FontFamilyProperty);
			set => SetValue(FontFamilyProperty, value);
		}

		public static readonly BindableProperty GlyphProperty = BindableProperty.Create(nameof(Glyph), typeof(string), typeof(FontImageSource), default(string),
			propertyChanged: (b, o, n) => ((FontImageSource)b).OnSourceChanged());

		public string Glyph
		{
			get => (string)GetValue(GlyphProperty);
			set => SetValue(GlyphProperty, value);
		}

		public static readonly BindableProperty SizeProperty = BindableProperty.Create(nameof(Size), typeof(double), typeof(FontImageSource), 30d,
			propertyChanged: (b, o, n) => ((FontImageSource)b).OnSourceChanged());

		[System.ComponentModel.TypeConverter(typeof(FontSizeConverter))]
		public double Size
		{
			get => (double)GetValue(SizeProperty);
			set => SetValue(SizeProperty, value);
		}

		public static readonly BindableProperty FontAutoScalingEnabledProperty =
			BindableProperty.Create("FontAutoScalingEnabled", typeof(bool), typeof(FontImageSource), false,
				propertyChanged: (b, o, n) => ((FontImageSource)b).OnSourceChanged());

		public bool FontAutoScalingEnabled
		{
			get => (bool)GetValue(FontAutoScalingEnabledProperty);
			set => SetValue(FontAutoScalingEnabledProperty, value);
		}
	}
}
