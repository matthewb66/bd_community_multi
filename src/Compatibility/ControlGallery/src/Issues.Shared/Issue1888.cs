using System;
using Microsoft.Maui.Controls.CustomAttributes;
using Microsoft.Maui.Controls.Internals;

namespace Microsoft.Maui.Controls.Compatibility.ControlGallery.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 1888, "Fix image resources not being freed after page is navigated away from ", PlatformAffected.iOS)]
	public class Issue1888 : ContentPage
	{
		public Issue1888()
		{
			var btn = new Button
			{
				Text = "Click!"
			};
			btn.Clicked += (sender, e) => Navigation.PushAsync(new LeakPage());
			Content = btn;
		}
	}

	public class LeakPage : ContentPage
	{
		public LeakPage()
		{
			var img = new Image
			{
				Source = new FileImageSource
				{
					File = "Default-568h@2x.png"
				}
			};
			Content = img;
		}
	}
}

