﻿using Microsoft.Maui.Graphics.Win2D;

namespace Microsoft.Maui.Handlers
{
	public partial class GraphicsViewHandler : ViewHandler<IGraphicsView, W2DGraphicsView>
	{
		protected override W2DGraphicsView CreateNativeView()
		{
			return new W2DGraphicsView();
		}

		public static void MapDrawable(GraphicsViewHandler handler, IGraphicsView graphicsView)
		{
			handler.NativeView?.UpdateDrawable(graphicsView);
		}

		public static void MapInvalidate(GraphicsViewHandler handler, IGraphicsView graphicsView, object? arg)
		{
			handler.NativeView?.Invalidate();
		}
	}
}