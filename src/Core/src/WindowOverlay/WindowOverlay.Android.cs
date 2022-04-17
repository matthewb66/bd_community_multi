﻿using System.Linq;
using Android.App;
using Android.Views;
using AndroidX.CoordinatorLayout.Widget;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Native;
using Microsoft.Maui.Handlers;

namespace Microsoft.Maui
{
	public partial class WindowOverlay
	{
		Activity? _nativeActivity;
		NativeGraphicsView? _graphicsView;
		ViewGroup? _nativeLayer;

		public virtual bool Initialize()
		{
			if (IsNativeViewInitialized)
				return true;

			if (Window == null)
				return false;

			var nativeWindow = Window?.Content?.GetNative(true);
			if (nativeWindow == null)
				return false;

			var handler = Window?.Handler as WindowHandler;
			if (handler?.MauiContext == null)
				return false;
			var rootManager = handler.MauiContext.GetNavigationRootManager();
			if (rootManager == null)
				return false;


			if (handler.NativeView is not Activity activity)
				return false;

			_nativeActivity = activity;
			_nativeLayer = rootManager.RootView as ViewGroup;

			if (_nativeLayer?.Context == null)
				return false;

			if (_nativeActivity?.WindowManager?.DefaultDisplay == null)
				return false;

			var measuredHeight = _nativeLayer.MeasuredHeight;

			if (_nativeActivity.Window != null)
				_nativeActivity.Window.DecorView.LayoutChange += DecorViewLayoutChange;

			if (_nativeActivity?.Resources?.DisplayMetrics != null)
				Density = _nativeActivity.Resources.DisplayMetrics.Density;

			_graphicsView = new NativeGraphicsView(_nativeLayer.Context, this);
			if (_graphicsView == null)
				return false;

			_graphicsView.Touch += TouchLayerTouch;
			_nativeLayer.AddView(_graphicsView, 0, new CoordinatorLayout.LayoutParams(CoordinatorLayout.LayoutParams.MatchParent, CoordinatorLayout.LayoutParams.MatchParent));
			_graphicsView.BringToFront();

			IsNativeViewInitialized = true;
			return IsNativeViewInitialized;
		}

		/// <inheritdoc/>
		public void Invalidate()
		{
			_graphicsView?.Invalidate();
		}

		/// <summary>
		/// Deinitializes the native event hooks and handlers used to drive the overlay.
		/// </summary>
		void DeinitializeNativeDependencies()
		{
			if (_nativeActivity?.Window != null)
				_nativeActivity.Window.DecorView.LayoutChange -= DecorViewLayoutChange;

			_nativeLayer?.RemoveView(_graphicsView);

			_graphicsView = null;
			IsNativeViewInitialized = false;
		}

		void TouchLayerTouch(object? sender, View.TouchEventArgs e)
		{
			if (e?.Event == null)
				return;

			if (e.Event.Action != MotionEventActions.Down && e.Event.ButtonState != MotionEventButtonState.Primary)
				return;

			var point = new Point(e.Event.RawX, e.Event.RawY);

			e.Handled = false;
			if (DisableUITouchEventPassthrough)
				e.Handled = true;
			else if (EnableDrawableTouchHandling)
				e.Handled = _windowElements.Any(n => n.Contains(point));

			OnTappedInternal(point);
		}

		void DecorViewLayoutChange(object? sender, View.LayoutChangeEventArgs e)
		{
			HandleUIChange();
			Invalidate();
		}
	}
}