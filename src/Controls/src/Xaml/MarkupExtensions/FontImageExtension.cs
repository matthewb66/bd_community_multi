using System;
using Microsoft.Maui.Graphics;

namespace Microsoft.Maui.Controls.Xaml
{
	[AcceptEmptyServiceProvider]
	[ContentProperty(nameof(Glyph))]
	public class FontImageExtension : IMarkupExtension<ImageSource>
	{
		public string FontFamily { get; set; }
		public string Glyph { get; set; }
		public Color Color { get; set; }

		[System.ComponentModel.TypeConverter(typeof(FontSizeConverter))]
		public double Size { get; set; } = (double)FontImageSource.SizeProperty.DefaultValue;

		public ImageSource ProvideValue(IServiceProvider serviceProvider)
		{
			return new FontImageSource
			{
				FontFamily = FontFamily,
				Glyph = Glyph,
				Color = Color,
				Size = Size
			};
		}

		object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
		{
			return (this as IMarkupExtension<ImageSource>).ProvideValue(serviceProvider);
		}
	}
}
