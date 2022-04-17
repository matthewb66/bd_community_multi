﻿using ObjCRuntime;
using UIKit;

namespace Microsoft.Maui.Platform
{
	public static class TextAlignmentExtensions
	{
		public static UITextAlignment ToNative(this TextAlignment alignment, IView view)
			=> alignment.ToNative(view.FlowDirection == FlowDirection.LeftToRight);

		public static UITextAlignment ToNative(this TextAlignment alignment, bool isLtr)
		{
			switch (alignment)
			{
				case TextAlignment.Center:
					return UITextAlignment.Center;
				case TextAlignment.End:
					if (isLtr)
						return UITextAlignment.Right;
					else
						return UITextAlignment.Left;
				default:
					if (isLtr)
						return UITextAlignment.Left;
					else
						return UITextAlignment.Right;
			}
		}

		public static UIControlContentVerticalAlignment ToNative(this TextAlignment alignment)
		{
			switch (alignment)
			{
				case TextAlignment.Center:
					return UIControlContentVerticalAlignment.Center;
				case TextAlignment.End:
					return UIControlContentVerticalAlignment.Bottom;
				case TextAlignment.Start:
					return UIControlContentVerticalAlignment.Top;
				default:
					return UIControlContentVerticalAlignment.Top;
			}
		}
	}
}