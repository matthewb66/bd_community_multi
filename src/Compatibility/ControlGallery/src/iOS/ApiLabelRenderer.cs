﻿using System;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Compatibility.ControlGallery;
using Microsoft.Maui.Controls.Compatibility.ControlGallery.iOS;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;
using Microsoft.Maui.Controls.Platform;
using ObjCRuntime;
using UIKit;

[assembly: ExportRenderer(typeof(ApiLabel), typeof(ApiLabelRenderer))]
namespace Microsoft.Maui.Controls.Compatibility.ControlGallery.iOS
{
	public class ApiLabelRenderer : LabelRenderer
	{

		protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
		{
			Element.Text = UIDevice.CurrentDevice.SystemVersion.ToString();
			base.OnElementChanged(e);

		}
	}
}
