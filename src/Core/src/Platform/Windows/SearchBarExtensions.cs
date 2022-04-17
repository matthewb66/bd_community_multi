﻿using Microsoft.Maui.Graphics;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace Microsoft.Maui.Platform
{
	public static class SearchBarExtensions
	{
		public static void UpdateCharacterSpacing(this AutoSuggestBox nativeControl, ISearchBar searchBar)
		{
			nativeControl.CharacterSpacing = searchBar.CharacterSpacing.ToEm();
		}

		public static void UpdatePlaceholder(this AutoSuggestBox nativeControl, ISearchBar searchBar)
		{
			nativeControl.PlaceholderText = searchBar.Placeholder ?? string.Empty;
		}

		public static void UpdatePlaceholderColor(this AutoSuggestBox nativeControl, ISearchBar searchBar, Brush? defaultPlaceholderColorBrush, Brush? defaultPlaceholderColorFocusBrush, MauiSearchTextBox? queryTextBox)
		{
			if (queryTextBox == null)
				return;

			Color placeholderColor = searchBar.PlaceholderColor;

			BrushHelpers.UpdateColor(placeholderColor, ref defaultPlaceholderColorBrush,
				() => queryTextBox.PlaceholderForegroundBrush, brush => queryTextBox.PlaceholderForegroundBrush = brush);

			BrushHelpers.UpdateColor(placeholderColor, ref defaultPlaceholderColorFocusBrush,
				() => queryTextBox.PlaceholderForegroundFocusBrush, brush => queryTextBox.PlaceholderForegroundFocusBrush = brush);
		}

		public static void UpdateText(this AutoSuggestBox nativeControl, ISearchBar searchBar)
		{
			nativeControl.Text = searchBar.Text;
		}

		public static void UpdateTextColor(this AutoSuggestBox nativeControl, ISearchBar searchBar, Brush? defaultTextColorBrush, Brush? defaultTextColorFocusBrush, MauiSearchTextBox? queryTextBox)
		{
			if (queryTextBox == null)
				return;

			Color textColor = searchBar.TextColor;

			BrushHelpers.UpdateColor(textColor, ref defaultTextColorBrush,
				() => queryTextBox.Foreground, brush => queryTextBox.Foreground = brush);

			BrushHelpers.UpdateColor(textColor, ref defaultTextColorFocusBrush,
				() => queryTextBox.ForegroundFocusBrush, brush => queryTextBox.ForegroundFocusBrush = brush);
		}

		public static void UpdateFont(this AutoSuggestBox nativeControl, ISearchBar searchBar, IFontManager fontManager) =>
			nativeControl.UpdateFont(searchBar.Font, fontManager);

		public static void UpdateHorizontalTextAlignment(this AutoSuggestBox nativeControl, ISearchBar searchBar, MauiSearchTextBox? queryTextBox)
		{
			if (queryTextBox == null)
				return;

			queryTextBox.TextAlignment = searchBar.HorizontalTextAlignment.ToNative();
		}

		public static void UpdateVerticalTextAlignment(this AutoSuggestBox nativeControl, ISearchBar searchBar, MauiSearchTextBox? queryTextBox)
		{
			if (queryTextBox == null)
				return;

			queryTextBox.VerticalAlignment = searchBar.VerticalTextAlignment.ToNativeVerticalAlignment();
		}

		public static void UpdateMaxLength(this AutoSuggestBox nativeControl, ISearchBar searchBar, MauiSearchTextBox? queryTextBox)
		{
			if (queryTextBox == null)
				return;

			queryTextBox.MaxLength = searchBar.MaxLength;

			var currentControlText = nativeControl.Text;

			if (currentControlText.Length > searchBar.MaxLength)
				nativeControl.Text = currentControlText.Substring(0, searchBar.MaxLength);
		}

		public static void UpdateIsTextPredictionEnabled(this AutoSuggestBox nativeControl, ISearchBar searchBar, MauiSearchTextBox? queryTextBox)
		{
			if (queryTextBox == null)
				return;

			queryTextBox.IsTextPredictionEnabled = searchBar.IsTextPredictionEnabled;
		}

		public static void UpdateCancelButtonColor(this AutoSuggestBox nativeControl, ISearchBar searchBar, MauiCancelButton? cancelButton, Brush? defaultDeleteButtonBackgroundColorBrush, Brush? defaultDeleteButtonForegroundColorBrush)
		{
			if (cancelButton == null || !cancelButton.IsReady)
				return;

			Color cancelColor = searchBar.CancelButtonColor;

			BrushHelpers.UpdateColor(cancelColor, ref defaultDeleteButtonForegroundColorBrush,
				() => cancelButton.ForegroundBrush, brush => cancelButton.ForegroundBrush = brush);

			if (cancelColor == null)
			{
				BrushHelpers.UpdateColor(null, ref defaultDeleteButtonBackgroundColorBrush,
					() => cancelButton.BackgroundBrush, brush => cancelButton.BackgroundBrush = brush);
			}
			else
			{
				// Determine whether the background should be black or white (in order to make the foreground color visible) 
				var bcolor = cancelColor.ToWindowsColor().GetContrastingColor().ToColor();
				BrushHelpers.UpdateColor(bcolor, ref defaultDeleteButtonBackgroundColorBrush,
					() => cancelButton.BackgroundBrush, brush => cancelButton.BackgroundBrush = brush);
			}
		}
	}
}
