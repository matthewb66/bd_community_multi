﻿#nullable enable
using System;
using NativeView = Microsoft.UI.Xaml.FrameworkElement;

namespace Microsoft.Maui.Handlers
{
	public partial class ViewHandler
	{
		static partial void MappingFrame(IViewHandler handler, IView view)
		{
			// Both Clip and Shadow depend on the Control size.
			handler.GetWrappedNativeView()?.UpdateClip(view);
			handler.GetWrappedNativeView()?.UpdateShadow(view);
		}

		public static void MapTranslationX(IViewHandler handler, IView view) 
		{ 
			handler.GetWrappedNativeView()?.UpdateTransformation(view);
		}

		public static void MapTranslationY(IViewHandler handler, IView view) 
		{ 
			handler.GetWrappedNativeView()?.UpdateTransformation(view);
		}

		public static void MapScale(IViewHandler handler, IView view) 
		{ 
			handler.GetWrappedNativeView()?.UpdateTransformation(view);
		}

		public static void MapScaleX(IViewHandler handler, IView view)
		{
			handler.GetWrappedNativeView()?.UpdateTransformation(view);
		}

		public static void MapScaleY(IViewHandler handler, IView view)
		{
			handler.GetWrappedNativeView()?.UpdateTransformation(view);
		}

		public static void MapRotation(IViewHandler handler, IView view) 
		{ 
			handler.GetWrappedNativeView()?.UpdateTransformation(view); 
		}

		public static void MapRotationX(IViewHandler handler, IView view) 
		{ 
			handler.GetWrappedNativeView()?.UpdateTransformation(view);
		}

		public static void MapRotationY(IViewHandler handler, IView view) 
		{ 
			handler.GetWrappedNativeView()?.UpdateTransformation(view); 
		}

		public static void MapAnchorX(IViewHandler handler, IView view) 
		{ 
			handler.GetWrappedNativeView()?.UpdateTransformation(view); 
		}

		public static void MapAnchorY(IViewHandler handler, IView view) 
		{ 
			handler.GetWrappedNativeView()?.UpdateTransformation(view);
		}

		public static void MapToolbar(IViewHandler handler, IView view)
		{
			if (view is IToolbarElement tb)
				MapToolbar(handler, tb);
		}

		internal static void MapToolbar(IElementHandler handler, IToolbarElement toolbarElement)
		{
			_ = handler.MauiContext ?? throw new InvalidOperationException($"{nameof(handler.MauiContext)} null");

			if (toolbarElement.Toolbar != null)
			{
				var toolBar = toolbarElement.Toolbar.ToNative(handler.MauiContext, true);
				handler.MauiContext.GetNavigationRootManager().SetToolbar(toolBar);
			}
		}
	}
}