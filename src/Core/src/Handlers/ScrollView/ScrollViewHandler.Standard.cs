﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Maui.Handlers
{
	public partial class ScrollViewHandler : ViewHandler<IScrollView, object>
	{
		protected override object CreateNativeView() => throw new NotImplementedException();

		public static void MapContent(IViewHandler handler, IScrollView scrollView) { }
		public static void MapHorizontalScrollBarVisibility(IViewHandler handler, IScrollView scrollView) { }
		public static void MapVerticalScrollBarVisibility(IViewHandler handler, IScrollView scrollView) { }
		public static void MapOrientation(IViewHandler handler, IScrollView scrollView) { }
		public static void MapContentSize(IViewHandler handler, IScrollView scrollView) { }
		public static void MapRequestScrollTo(ScrollViewHandler handler, IScrollView scrollView, object? args) { }
	}
}
