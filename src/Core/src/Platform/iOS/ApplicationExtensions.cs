﻿using System.Collections.Generic;
using Foundation;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.LifecycleEvents;
using ObjCRuntime;
using UIKit;

namespace Microsoft.Maui.Platform
{
	public static class ApplicationExtensions
	{
		public static void RequestNewWindow(this UIApplicationDelegate nativeApplication, IApplication application, OpenWindowRequest? args)
		{
			if (application.Handler?.MauiContext is not IMauiContext applicationContext || args is null)
				return;

			var state = args?.State;
			var userActivity = state.ToUserActivity(MauiUIApplicationDelegate.MauiSceneConfigurationKey);

			UIApplication.SharedApplication.RequestSceneSessionActivation(
				null,
				userActivity,
				null,
				err => application.Handler?.MauiContext?.CreateLogger<IApplication>()?.LogError(new NSErrorException(err), err.Description));
		}

		public static void CreateNativeWindow(this UIApplicationDelegate nativeApplication, IApplication application, UIApplication uiApplication, NSDictionary launchOptions)
		{
			// Find any userinfo/dictionaries we might pass into the activation state
			var dicts = new List<NSDictionary>();
			if (uiApplication.UserActivity?.UserInfo is not null)
				dicts.Add(uiApplication.UserActivity.UserInfo);
			if (launchOptions is not null)
				dicts.Add(launchOptions);

			var window = CreateNativeWindow(application, null, dicts.ToArray());
			if (window is not null)
			{
				nativeApplication.Window = window;
				nativeApplication.Window.MakeKeyAndVisible();
			}
		}

		public static void CreateNativeWindow(this UIWindowSceneDelegate sceneDelegate, IApplication application, UIScene scene, UISceneSession session, UISceneConnectionOptions connectionOptions)
		{
			// Find any userinfo/dictionaries we might pass into the activation state
			var dicts = new List<NSDictionary>();
			if (scene.UserActivity?.UserInfo is not null)
				dicts.Add(scene.UserActivity.UserInfo);
			if (session.UserInfo is not null)
				dicts.Add(session.UserInfo);
			if (session.StateRestorationActivity?.UserInfo is not null)
				dicts.Add(session.StateRestorationActivity.UserInfo);
			if (connectionOptions.UserActivities is not null)
			{
				foreach (var u in connectionOptions.UserActivities)
				{
					if (u is NSUserActivity userActivity && userActivity.UserInfo is not null)
						dicts.Add(userActivity.UserInfo);
				}
			}

			var window = CreateNativeWindow(application, scene as UIWindowScene, dicts.ToArray());
			if (window is not null)
			{
				sceneDelegate.Window = window;
				sceneDelegate.Window.MakeKeyAndVisible();
			}
		}

		static UIWindow? CreateNativeWindow(IApplication application, UIWindowScene? windowScene, NSDictionary[]? states)
		{
			if (application.Handler?.MauiContext is not IMauiContext applicationContext)
				return null;

			var uiWindow = windowScene is not null
				? new UIWindow(windowScene)
				: new UIWindow();

			var mauiContext = applicationContext.MakeWindowScope(uiWindow, out var windowScope);

			applicationContext.Services?.InvokeLifecycleEvents<iOSLifecycle.OnMauiContextCreated>(del => del(mauiContext));

			var activationState = new ActivationState(mauiContext, states);

			var mauiWindow = application.CreateWindow(activationState);

			uiWindow.SetWindowHandler(mauiWindow, mauiContext);

			return uiWindow;
		}

		public static NSUserActivity ToUserActivity(this IPersistedState? state, string userActivityType)
		{
			var userInfo = new NSMutableDictionary();

			if (state is not null)
			{
				foreach (var pair in state)
				{
					userInfo.SetValueForKey(new NSString(pair.Value), new NSString(pair.Key));
				}
			}

			var userActivity = new NSUserActivity(userActivityType);
			userActivity.AddUserInfoEntries(userInfo);

			return userActivity;
		}
	}
}