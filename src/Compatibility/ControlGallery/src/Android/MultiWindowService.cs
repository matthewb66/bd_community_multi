﻿using System;
using Android.Content;
using Microsoft.Maui.Controls.Compatibility.ControlGallery.Issues;

namespace Microsoft.Maui.Controls.Compatibility.ControlGallery.Android
{
	public class MultiWindowService : IMultiWindowService
	{
		public void OpenWindow(Type type)
		{
			if (type == typeof(Issue10182))
			{
				var context = DependencyService.Resolve<Context>();
				Intent intent = new Intent(context, typeof(Issue10182Activity));
				context.StartActivity(intent);
			}
		}
	}
}