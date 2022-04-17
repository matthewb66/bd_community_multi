#nullable enable
using Microsoft.Maui.Graphics;
using Microsoft.UI.Xaml.Controls;

namespace Microsoft.Maui.Platform
{
	public static class TextBoxExtensions
	{
		public static void UpdateIsPassword(this TextBox nativeControl, IEntry entry)
		{
			if (nativeControl is MauiPasswordTextBox passwordTextBox)
				passwordTextBox.IsPassword = entry.IsPassword;
		}

		public static void UpdateText(this TextBox nativeControl, ITextInput textInput)
		{
			var newText = textInput.Text;

			if (nativeControl is MauiPasswordTextBox passwordTextBox && passwordTextBox.Password == newText)
				return;
			if (nativeControl.Text == newText)
				return;

			nativeControl.Text = newText ?? string.Empty;

			if (!string.IsNullOrEmpty(nativeControl.Text))
				nativeControl.SelectionStart = nativeControl.Text.Length;
		}

		public static void UpdateBackground(this TextBox textBox, IView view)
		{
			var brush = view.Background?.ToNative();
			if (brush is null)
			{
				textBox.Resources.Remove("TextControlBackground");
				textBox.Resources.Remove("TextControlBackgroundPointerOver");
				textBox.Resources.Remove("TextControlBackgroundFocused");
				textBox.Resources.Remove("TextControlBackgroundDisabled");
			}
			else
			{
				textBox.Resources["TextControlBackground"] = brush;
				textBox.Resources["TextControlBackgroundPointerOver"] = brush;
				textBox.Resources["TextControlBackgroundFocused"] = brush;
				textBox.Resources["TextControlBackgroundDisabled"] = brush;
			}
		}

		public static void UpdateTextColor(this TextBox textBox, ITextStyle textStyle)
		{
			var brush = textStyle.TextColor?.ToNative();
			if (brush is null)
			{
				textBox.Resources.Remove("TextControlForeground");
				textBox.Resources.Remove("TextControlForegroundPointerOver");
				textBox.Resources.Remove("TextControlForegroundFocused");
				textBox.Resources.Remove("TextControlForegroundDisabled");
			}
			else
			{
				textBox.Resources["TextControlForeground"] = brush;
				textBox.Resources["TextControlForegroundPointerOver"] = brush;
				textBox.Resources["TextControlForegroundFocused"] = brush;
				textBox.Resources["TextControlForegroundDisabled"] = brush;
			}
		}

		public static void UpdateCharacterSpacing(this TextBox textBox, ITextStyle textStyle)
		{
			textBox.CharacterSpacing = textStyle.CharacterSpacing.ToEm();
		}

		public static void UpdateReturnType(this TextBox textBox, ITextInput textInput)
		{
			textBox.UpdateInputScope(textInput);
		}

		public static void UpdateClearButtonVisibility(this TextBox textBox, IEntry entry) =>
			MauiTextBox.SetIsDeleteButtonEnabled(textBox, entry.ClearButtonVisibility == ClearButtonVisibility.WhileEditing);

		public static void UpdatePlaceholder(this TextBox textBox, IPlaceholder placeholder)
		{
			textBox.PlaceholderText = placeholder.Placeholder ?? string.Empty;
		}

		public static void UpdatePlaceholderColor(this TextBox textBox, IPlaceholder placeholder)
		{
			var brush = placeholder.PlaceholderColor?.ToNative();

			if (brush is null)
			{
				// Windows.Foundation.UniversalApiContract < 5
				textBox.Resources.Remove("TextControlPlaceholderForeground");
				textBox.Resources.Remove("TextControlPlaceholderForegroundPointerOver");
				textBox.Resources.Remove("TextControlPlaceholderForegroundFocused");
				textBox.Resources.Remove("TextControlPlaceholderForegroundDisabled");

				// Windows.Foundation.UniversalApiContract >= 5
				textBox.ClearValue(TextBox.PlaceholderForegroundProperty);
			}
			else
			{
				// Windows.Foundation.UniversalApiContract < 5
				textBox.Resources["TextControlPlaceholderForeground"] = brush;
				textBox.Resources["TextControlPlaceholderForegroundPointerOver"] = brush;
				textBox.Resources["TextControlPlaceholderForegroundFocused"] = brush;
				textBox.Resources["TextControlPlaceholderForegroundDisabled"] = brush;

				// Windows.Foundation.UniversalApiContract >= 5
				textBox.PlaceholderForeground = brush;
			}
		}

		public static void UpdateFont(this TextBox nativeControl, IText text, IFontManager fontManager) =>
			nativeControl.UpdateFont(text.Font, fontManager);

		public static void UpdateIsReadOnly(this TextBox textBox, ITextInput textInput)
		{
			textBox.IsReadOnly = textInput.IsReadOnly;
		}

		public static void UpdateMaxLength(this TextBox textBox, ITextInput textInput)
		{
			var maxLength = textInput.MaxLength;

			if (maxLength == -1)
				maxLength = int.MaxValue;

			textBox.MaxLength = maxLength;

			var currentControlText = textBox.Text;

			if (currentControlText.Length > maxLength)
				textBox.Text = currentControlText.Substring(0, maxLength);
		}

		public static void UpdateIsTextPredictionEnabled(this TextBox textBox, ITextInput textInput)
		{
			textBox.UpdateInputScope(textInput);
		}

		public static void UpdateKeyboard(this TextBox textBox, ITextInput textInput)
		{
			textBox.UpdateInputScope(textInput);
		}

		internal static void UpdateInputScope(this TextBox textBox, ITextInput textInput)
		{
			if (textInput.Keyboard is CustomKeyboard custom)
			{
				textBox.IsTextPredictionEnabled = (custom.Flags & KeyboardFlags.Suggestions) != 0;
				textBox.IsSpellCheckEnabled = (custom.Flags & KeyboardFlags.Spellcheck) != 0;
			}
			else
			{
				textBox.IsTextPredictionEnabled = textInput.IsTextPredictionEnabled;
				textBox.IsSpellCheckEnabled = textInput.IsTextPredictionEnabled;
			}

			var inputScope = new UI.Xaml.Input.InputScope();

			if (textInput is IEntry entry && entry.ReturnType == ReturnType.Search)
				inputScope.Names.Add(new UI.Xaml.Input.InputScopeName(UI.Xaml.Input.InputScopeNameValue.Search));

			inputScope.Names.Add(textInput.Keyboard.ToInputScopeName());

			textBox.InputScope = inputScope;
		}

		public static void UpdateHorizontalTextAlignment(this TextBox textBox, ITextAlignment textAlignment)
		{
			// We don't have a FlowDirection yet, so there's nothing to pass in here. 
			// TODO: Update this when FlowDirection is available 
			// (or update the extension to take an ILabel instead of an alignment and work it out from there) 
			textBox.TextAlignment = textAlignment.HorizontalTextAlignment.ToNative(true);
		}

		public static void UpdateVerticalTextAlignment(this TextBox textBox, ITextAlignment textAlignment) =>
			MauiTextBox.SetVerticalTextAlignment(textBox, textAlignment.VerticalTextAlignment.ToNativeVerticalAlignment());

		public static void UpdateCursorPosition(this TextBox textBox, IEntry entry)
		{
			if (textBox.SelectionStart != entry.CursorPosition)
				textBox.SelectionStart = entry.CursorPosition;
		}

		public static void UpdateSelectionLength(this TextBox textBox, IEntry entry)
		{
			if (textBox.SelectionLength != entry.SelectionLength)
				textBox.SelectionLength = entry.SelectionLength;
		}
	}
}