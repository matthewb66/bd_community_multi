﻿using Android.Content.Res;

namespace Microsoft.Maui.Platform
{
	public static class PickerExtensions
	{
		public static void UpdateTitle(this MauiPicker nativePicker, IPicker picker) =>
			UpdatePicker(nativePicker, picker);

		public static void UpdateTitleColor(this MauiPicker nativePicker, IPicker picker, ColorStateList? defaultColor)
		{
			var titleColor = picker.TitleColor;

			if (titleColor == null)
			{
				nativePicker.SetHintTextColor(defaultColor);
			}
			else
			{
				var androidColor = titleColor.ToNative();
				if (!nativePicker.TextColors.IsOneColor(ColorStates.EditText, androidColor))
					nativePicker.SetHintTextColor(ColorStateListExtensions.CreateEditText(androidColor));
			}
		}

		public static void UpdateTextColor(this MauiPicker nativePicker, IPicker picker, ColorStateList? defaultColor)
		{
			var textColor = picker.TextColor;

			if (textColor == null)
			{
				nativePicker.SetTextColor(defaultColor);
			}
			else
			{
				var androidColor = textColor.ToNative();
				if (!nativePicker.TextColors.IsOneColor(ColorStates.EditText, androidColor))
					nativePicker.SetTextColor(ColorStateListExtensions.CreateEditText(androidColor));
			}
		}

		public static void UpdateSelectedIndex(this MauiPicker nativePicker, IPicker picker) =>
			UpdatePicker(nativePicker, picker);

		internal static void UpdatePicker(this MauiPicker nativePicker, IPicker picker)
		{
			nativePicker.Hint = picker.Title;

			if (picker.SelectedIndex == -1 || picker.SelectedIndex >= picker.GetCount())
				nativePicker.Text = null;
			else
				nativePicker.Text = picker.GetItem(picker.SelectedIndex);
		}
	}
}