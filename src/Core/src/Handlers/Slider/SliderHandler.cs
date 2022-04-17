#nullable enable
namespace Microsoft.Maui.Handlers
{
	public partial class SliderHandler
	{
		public static IPropertyMapper<ISlider, SliderHandler> SliderMapper = new PropertyMapper<ISlider, SliderHandler>(ViewHandler.ViewMapper)
		{
			[nameof(ISlider.Maximum)] = MapMaximum,
			[nameof(ISlider.MaximumTrackColor)] = MapMaximumTrackColor,
			[nameof(ISlider.Minimum)] = MapMinimum,
			[nameof(ISlider.MinimumTrackColor)] = MapMinimumTrackColor,
			[nameof(ISlider.ThumbColor)] = MapThumbColor,
			[nameof(ISlider.ThumbImageSource)] = MapThumbImageSource,
			[nameof(ISlider.Value)] = MapValue,
		};

		public SliderHandler() : base(SliderMapper)
		{

		}

		public SliderHandler(IPropertyMapper? mapper = null) : base(mapper ?? SliderMapper)
		{

		}
	}
}