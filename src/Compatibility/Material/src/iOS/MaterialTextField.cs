using System;
using CoreGraphics;
using MaterialComponents;
using ObjCRuntime;
using UIKit;
using Microsoft.Maui.Controls.Internals;
using MTextField = MaterialComponents.TextField;
using MTextInputControllerBase = MaterialComponents.TextInputControllerBase;

namespace Microsoft.Maui.Controls.Compatibility.Material.iOS
{
	public class MaterialTextField : MTextField, IMaterialTextField
	{
		public ContainerScheme ContainerScheme { get; }
		public SemanticColorScheme ColorScheme { get; set; }
		public TypographyScheme TypographyScheme { get; set; }
		public MTextInputControllerBase ActiveTextInputController { get; set; }
		public ITextInput TextInput => this;
		public CGSize? BackgroundSize { get; set; }

		public MaterialTextField(IMaterialEntryRenderer element, IFontElement fontElement)
		{
			ContainerScheme = new ContainerScheme();
			MaterialTextManager.Init(element, this, fontElement);
		}

		public override CGSize SizeThatFits(CGSize size)
		{
			var result = base.SizeThatFits(size);

			if (nfloat.IsInfinity(result.Width))
				result = SystemLayoutSizeFittingSize(result, (float)UILayoutPriority.FittingSizeLevel, (float)UILayoutPriority.DefaultHigh);

			return result;
		}

		internal void ApplyTypographyScheme(IFontElement fontElement) =>
			MaterialTextManager.ApplyTypographyScheme(this, fontElement);
		internal void ApplyTheme(IMaterialEntryRenderer element) => MaterialTextManager.ApplyTheme(this, element);
		internal void ApplyThemeIfNeeded(IMaterialEntryRenderer element) => MaterialTextManager.ApplyThemeIfNeeded(this, element);
		internal void UpdatePlaceholder(IMaterialEntryRenderer element) => MaterialTextManager.UpdatePlaceholder(this, element);
		internal void UpdateTextColor(IMaterialEntryRenderer element) => MaterialTextManager.UpdateTextColor(this, element);
	}
}