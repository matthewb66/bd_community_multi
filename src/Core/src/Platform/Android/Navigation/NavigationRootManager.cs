﻿using System;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.DrawerLayout.Widget;
using AndroidX.Fragment.App;
using Google.Android.Material.AppBar;
using AView = Android.Views.View;

namespace Microsoft.Maui.Platform
{
	public class NavigationRootManager
	{
		IMauiContext _mauiContext;
		AView? _rootView;
		ViewFragment? _viewFragment;
		IToolbarElement? _toolbarElement;

		// TODO MAUI: temporary event to alert when rootview is ready
		// handlers and various bits use this to start interacting with rootview
		internal event EventHandler? RootViewChanged;

		LayoutInflater LayoutInflater => _mauiContext?.GetLayoutInflater()
			?? throw new InvalidOperationException($"LayoutInflater missing");

		internal FragmentManager FragmentManager => _mauiContext?.GetFragmentManager()
			?? throw new InvalidOperationException($"FragmentManager missing");

		public AView? RootView => _rootView;

		internal DrawerLayout? DrawerLayout { get; private set; }

		public NavigationRootManager(IMauiContext mauiContext)
		{
			_mauiContext = mauiContext;
		}

		internal void SetToolbarElement(IToolbarElement toolbarElement)
		{
			_toolbarElement = toolbarElement;
		}

		internal void SetRootView(IView view, IMauiContext? mauiContext = null)
		{
			mauiContext = mauiContext ?? _mauiContext;
			var containerView = view.ToContainerView(mauiContext);
			var navigationLayout = containerView.FindViewById(Resource.Id.navigation_layout);

			if (containerView is DrawerLayout dl)
			{
				_rootView = dl;
				DrawerLayout = dl;
			}
			else if (containerView is ContainerView cv && cv.MainView is DrawerLayout dlc)
			{
				_rootView = cv;
				DrawerLayout = dlc;
			}
			else
			{
				navigationLayout ??=
				   LayoutInflater
					   .Inflate(Resource.Layout.navigationlayout, null)
					   .JavaCast<CoordinatorLayout>();

				_rootView = navigationLayout;
			}

			if (DrawerLayout == null)
			{
				SetContentView(containerView);
			}

			RootViewChanged?.Invoke(this, EventArgs.Empty);

			// Toolbars are added dynamically to the layout, but this can't be done until the full base
			// layout has been set on the view.
			// This is mainly a problem because the toolbar native view is created during the 'ToContainerView'
			// and at this point the View that's going to house the Toolbar doesn't have access to
			// the AppBarLayout that's part of the RootView
			_toolbarElement?.Toolbar?.Parent?.Handler?.UpdateValue(nameof(IToolbarElement.Toolbar));

		}

		void SetContentView(AView? view)
		{
			if (view == null)
			{
				if (_viewFragment != null)
				{
					FragmentManager
						.BeginTransaction()
						.Remove(_viewFragment)
						.SetReorderingAllowed(true)
						.Commit();
				}
			}
			else
			{
				_viewFragment = new ViewFragment(view);
				FragmentManager
					.BeginTransaction()
					.Replace(Resource.Id.navigationlayout_content, _viewFragment)
					.SetReorderingAllowed(true)
					.Commit();
			}
		}
	}
}
