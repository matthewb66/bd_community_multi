namespace Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific
{
	using Microsoft.Maui.Graphics;
	using FormsElement = Maui.Controls.Cell;

	public static class Cell
	{
		public static readonly BindableProperty DefaultBackgroundColorProperty = BindableProperty.Create(nameof(DefaultBackgroundColor), typeof(Color), typeof(Cell), null);

		public static Color GetDefaultBackgroundColor(BindableObject element)
			=> (Color)element.GetValue(DefaultBackgroundColorProperty);

		public static void SetDefaultBackgroundColor(BindableObject element, Color value)
			=> element.SetValue(DefaultBackgroundColorProperty, value);

		public static Color DefaultBackgroundColor(this IPlatformElementConfiguration<iOS, FormsElement> config)
			=> GetDefaultBackgroundColor(config.Element);

		public static IPlatformElementConfiguration<iOS, FormsElement> SetDefaultBackgroundColor(this IPlatformElementConfiguration<iOS, FormsElement> config, Color value)
		{
			SetDefaultBackgroundColor(config.Element, value);
			return config;
		}
	}
}
