﻿using Microsoft.Maui.Graphics;

namespace Microsoft.Maui.Controls.Compatibility.ControlGallery
{
	public interface INativeColorService
	{
		Color GetConvertedColor(bool shouldCrash);
	}
}