﻿#nullable enable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Handlers;
using Microsoft.UI.Xaml.Controls;
using WImage = Microsoft.UI.Xaml.Controls.Image;
using NativeAutomationProperties = Microsoft.UI.Xaml.Automation.AutomationProperties;

namespace Microsoft.Maui.Controls
{
	public partial class Toolbar
	{
		readonly ImageConverter _imageConverter = new ImageConverter();
		readonly ImageSourceIconElementConverter _imageSourceIconElementConverter = new ImageSourceIconElementConverter();

		NavigationRootManager? NavigationRootManager =>
			MauiContext?.GetNavigationRootManager();

		internal void UpdateMenu()
		{
			if (NavigationRootManager == null)
				return;

			if (NavigationRootManager.RootView is not MauiNavigationView)
				return;

			if (Handler.NativeView is not WindowHeader wh)
				return;

			var commandBar = wh.CommandBar;

			if (commandBar == null)
			{
				return;
			}

			commandBar.PrimaryCommands.Clear();
			commandBar.SecondaryCommands.Clear();

			List<ToolbarItem> toolbarItems = new List<ToolbarItem>(ToolbarItems ?? new ToolbarItem[0]);

			foreach (ToolbarItem item in toolbarItems)
			{
				var button = new AppBarButton();
				button.SetBinding(AppBarButton.LabelProperty, "Text");

				if (commandBar.IsDynamicOverflowEnabled && item.Order == ToolbarItemOrder.Secondary)
				{
					button.SetBinding(AppBarButton.IconProperty, "IconImageSource", _imageSourceIconElementConverter);
				}
				else
				{
					var img = new WImage();
					img.SetBinding(WImage.SourceProperty, "Value");
					img.SetBinding(WImage.DataContextProperty, "IconImageSource", _imageConverter);
					button.Content = img;
				}

				button.Command = new MenuItemCommand(item);
				button.DataContext = item;
				button.SetValue(NativeAutomationProperties.AutomationIdProperty, item.AutomationId);
				button.SetAutomationPropertiesName(item);
				button.SetAutomationPropertiesAccessibilityView(item);
				button.SetAutomationPropertiesHelpText(item);

				button.SetAutomationPropertiesLabeledBy(item, null);

				ToolbarItemOrder order = item.Order == ToolbarItemOrder.Default ? ToolbarItemOrder.Primary : item.Order;
				if (order == ToolbarItemOrder.Primary)
				{
					commandBar.PrimaryCommands.Add(button);
				}
				else
				{
					commandBar.SecondaryCommands.Add(button);
				}
			}
		}

		public static void MapToolbarPlacement(ToolbarHandler arg1, Toolbar arg2)
		{
		}

		public static void MapToolbarDynamicOverflowEnabled(ToolbarHandler arg1, Toolbar arg2)
		{
			arg1.NativeView.UpdateToolbarDynamicOverflowEnabled(arg2);
		}

		public static void MapBarTextColor(ToolbarHandler arg1, Toolbar arg2)
		{
			arg1.NativeView.UpdateBarTextColor(arg2);
		}

		public static void MapBarBackground(ToolbarHandler arg1, Toolbar arg2)
		{
			arg1.NativeView.UpdateBarBackground(arg2);
		}

		public static void MapBarBackgroundColor(ToolbarHandler arg1, Toolbar arg2)
		{
			arg1.NativeView.UpdateBarBackgroundColor(arg2);
		}

		public static void MapBackButtonTitle(ToolbarHandler arg1, Toolbar arg2)
		{
			arg1.NativeView.UpdateBackButton(arg2);
		}

		public static void MapToolbarItems(ToolbarHandler arg1, Toolbar arg2)
		{
			arg2.UpdateMenu();
		}

		public static void MapTitle(ToolbarHandler arg1, Toolbar arg2)
		{
			arg1.NativeView.UpdateTitle(arg2);
		}

		public static void MapIconColor(ToolbarHandler arg1, Toolbar arg2)
		{
			arg1.NativeView.UpdateIconColor(arg2);
		}

		public static void MapTitleView(ToolbarHandler arg1, Toolbar arg2)
		{
			arg1.NativeView.UpdateTitleView(arg2);
		}

		public static void MapTitleIcon(ToolbarHandler arg1, Toolbar arg2)
		{
			arg1.NativeView.UpdateTitleIcon(arg2);
		}

		public static void MapBackButtonVisible(ToolbarHandler arg1, Toolbar arg2)
		{
			arg1.NativeView.UpdateBackButton(arg2);
		}

		public static void MapIsVisible(ToolbarHandler arg1, Toolbar arg2)
		{
			arg1.NativeView.UpdateIsVisible(arg2);
		}
	}
}
