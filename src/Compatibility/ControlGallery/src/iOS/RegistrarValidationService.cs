﻿using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Compatibility.ControlGallery.iOS;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;
using Microsoft.Maui.Controls.Internals;

[assembly: Dependency(typeof(RegistrarValidationService))]
namespace Microsoft.Maui.Controls.Compatibility.ControlGallery.iOS
{
	[Preserve(AllMembers = true)]
	public class RegistrarValidationService : IRegistrarValidationService
	{
		public bool Validate(VisualElement element, out string message)
		{
			message = "Success";

			if (element == null)
				return true;

			var renderer = Platform.iOS.Platform.CreateRenderer(element);

			if (renderer == null
				|| renderer.GetType().Name == "DefaultRenderer"
				|| (element is FlyoutPage && Device.Idiom == TargetIdiom.Tablet && !(renderer is TabletFlyoutPageRenderer))
				|| (element is FlyoutPage && Device.Idiom == TargetIdiom.Phone && !(renderer is PhoneFlyoutPageRenderer))
				)
			{
				message = $"Failed to load proper iOS renderer for {element.GetType().Name}";
				return false;
			}

			return true;
		}
	}
}