﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility.ControlGallery;
using Microsoft.Maui.Controls.Compatibility.ControlGallery.Android;

[assembly: Dependency(typeof(RegistrarValidationService))]
namespace Microsoft.Maui.Controls.Compatibility.ControlGallery.Android
{
	public class RegistrarValidationService : IRegistrarValidationService
	{
		readonly global::Android.Content.Context _context;

		public RegistrarValidationService()
		{
			_context = MainApplication.ActivityContext;
		}

		public bool Validate(VisualElement element, out string message)
		{
			message = "Success";

			if (element == null)
				return true;

			object renderer = Platform.Android.Platform.CreateRendererWithContext(element, _context);

			if (renderer == null
				|| renderer.GetType().Name == "DefaultRenderer"
				)
			{
				var sp = MauiApplication.Current.Services.GetRequiredService<IMauiHandlersFactory>();
				renderer = sp.GetHandler(element.GetType());
			}

			if (renderer == null
				|| renderer.GetType().Name == "DefaultRenderer"
				)
			{
				message = $"Failed to load Android renderer for {element.GetType().Name}";
				return false;
			}

			return true;
		}
	}
}