﻿using System;
using System.Threading.Tasks;
using Microsoft.Maui.DeviceTests.Stubs;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Handlers;
using Xunit;

namespace Microsoft.Maui.DeviceTests
{
	[Category(TestCategory.Entry)]
	public partial class EntryHandlerTests : HandlerTestBase<EntryHandler, EntryStub>
	{
		[Fact(DisplayName = "Text Initializes Correctly")]
		public async Task TextInitializesCorrectly()
		{
			var entry = new EntryStub()
			{
				Text = "Test"
			};

			await ValidatePropertyInitValue(entry, () => entry.Text, GetNativeText, entry.Text);
		}

		[Fact(DisplayName = "TextColor Initializes Correctly")]
		public async Task TextColorInitializesCorrectly()
		{
			var entry = new EntryStub()
			{
				Text = "Test",
				TextColor = Colors.Yellow
			};

			await ValidatePropertyInitValue(entry, () => entry.TextColor, GetNativeTextColor, entry.TextColor);
		}

		[Fact(DisplayName = "Null Text Color Doesn't Crash")]
		public async Task NullTextColorDoesntCrash()
		{
			var entry = new EntryStub()
			{
				Text = "Test",
				TextColor = null
			};

			await CreateHandlerAsync(entry);
		}

		[Theory(DisplayName = "IsPassword Initializes Correctly")]
		[InlineData(true)]
		[InlineData(false)]
		public async Task IsPasswordInitializesCorrectly(bool isPassword)
		{
			var entry = new EntryStub()
			{
				IsPassword = isPassword
			};

			await ValidatePropertyInitValue(entry, () => entry.IsPassword, GetNativeIsPassword, isPassword);
		}

		[Fact(DisplayName = "Placeholder Initializes Correctly")]
		public async Task PlaceholderInitializesCorrectly()
		{
			var entry = new EntryStub()
			{
				Placeholder = "Placeholder"
			};

			await ValidatePropertyInitValue(entry, () => entry.Placeholder, GetNativePlaceholder, "Placeholder");
		}

		[Theory(DisplayName = "Is Text Prediction Enabled")]
		[InlineData(true)]
		[InlineData(false)]
		public async Task IsTextPredictionEnabledCorrectly(bool isEnabled)
		{
			var entry = new EntryStub()
			{
				IsTextPredictionEnabled = isEnabled
			};

			await ValidatePropertyInitValue(entry, () => entry.IsTextPredictionEnabled, GetNativeIsTextPredictionEnabled, isEnabled);
		}

		[Theory(DisplayName = "IsPassword Updates Correctly")]
		[InlineData(true, true)]
		[InlineData(true, false)]
		[InlineData(false, true)]
		[InlineData(false, false)]
		public async Task IsPasswordUpdatesCorrectly(bool setValue, bool unsetValue)
		{
			var entry = new EntryStub();

			await ValidatePropertyUpdatesValue(
				entry,
				nameof(IEntry.IsPassword),
				GetNativeIsPassword,
				setValue,
				unsetValue);
		}

		[Theory(DisplayName = "TextColor Updates Correctly")]
		[InlineData(0xFF0000, 0x0000FF)]
		[InlineData(0x0000FF, 0xFF0000)]
		public async Task TextColorUpdatesCorrectly(uint setValue, uint unsetValue)
		{
			var entry = new EntryStub();

			var setColor = Color.FromUint(setValue);
			var unsetColor = Color.FromUint(unsetValue);

			await ValidatePropertyUpdatesValue(
				entry,
				nameof(IEntry.TextColor),
				GetNativeTextColor,
				setColor,
				unsetColor);
		}

		[Theory(DisplayName = "Text Updates Correctly")]
		[InlineData(null, null)]
		[InlineData(null, "Hello")]
		[InlineData("Hello", null)]
		[InlineData("Hello", "Goodbye")]
		public async Task TextUpdatesCorrectly(string setValue, string unsetValue)
		{
			var entry = new EntryStub();

			await ValidatePropertyUpdatesValue(
				entry,
				nameof(IEntry.Text),
				h =>
				{
					var n = GetNativeText(h);
					if (string.IsNullOrEmpty(n))
						n = null; // native platforms may not upport null text
					return n;
				},
				setValue,
				unsetValue);
		}

		[Theory(DisplayName = "IsTextPredictionEnabled Updates Correctly")]
		[InlineData(true, true)]
		[InlineData(true, false)]
		[InlineData(false, true)]
		[InlineData(false, false)]
		public async Task IsTextPredictionEnabledUpdatesCorrectly(bool setValue, bool unsetValue)
		{
			var entry = new EntryStub();

			await ValidatePropertyUpdatesValue(
				entry,
				nameof(IEntry.IsTextPredictionEnabled),
				GetNativeIsTextPredictionEnabled,
				setValue,
				unsetValue);
		}

		[Theory(DisplayName = "IsReadOnly Updates Correctly")]
		[InlineData(true, true)]
		[InlineData(true, false)]
		[InlineData(false, true)]
		[InlineData(false, false)]
		public async Task IsReadOnlyUpdatesCorrectly(bool setValue, bool unsetValue)
		{
			var entry = new EntryStub();

			await ValidatePropertyUpdatesValue(
				entry,
				nameof(IEntry.IsReadOnly),
				GetNativeIsReadOnly,
				setValue,
				unsetValue);
		}

		[Theory(DisplayName = "Validates clear button visibility.")]
		[InlineData(ClearButtonVisibility.WhileEditing, true)]
		[InlineData(ClearButtonVisibility.Never, false)]
		public async Task ValidateClearButtonVisibility(ClearButtonVisibility clearButtonVisibility, bool expected)
		{
			var entryStub = new EntryStub()
			{
				ClearButtonVisibility = clearButtonVisibility,
				Text = "Test text input.",
				FlowDirection = FlowDirection.LeftToRight
			};

			await ValidatePropertyInitValue(entryStub, () => expected, GetNativeClearButtonVisibility, expected);
		}

		[Theory(DisplayName = "Validates Numeric Keyboard")]
		[InlineData(nameof(Keyboard.Chat), false)]
		[InlineData(nameof(Keyboard.Default), false)]
		[InlineData(nameof(Keyboard.Email), false)]
		[InlineData(nameof(Keyboard.Numeric), true)]
		[InlineData(nameof(Keyboard.Plain), false)]
		[InlineData(nameof(Keyboard.Telephone), false)]
		[InlineData(nameof(Keyboard.Text), false)]
		[InlineData(nameof(Keyboard.Url), false)]
		public async Task ValidateNumericKeyboard(string keyboardName, bool expected)
		{
			var keyboard = (Keyboard)typeof(Keyboard).GetProperty(keyboardName).GetValue(null);

			var entryStub = new EntryStub() { Keyboard = keyboard };

			await ValidatePropertyInitValue(entryStub, () => expected, GetNativeIsNumericKeyboard, expected);
		}

		[Theory(DisplayName = "Validates Email Keyboard")]
		[InlineData(nameof(Keyboard.Chat), false)]
		[InlineData(nameof(Keyboard.Default), false)]
		[InlineData(nameof(Keyboard.Email), true)]
		[InlineData(nameof(Keyboard.Numeric), false)]
		[InlineData(nameof(Keyboard.Plain), false)]
		[InlineData(nameof(Keyboard.Telephone), false)]
		[InlineData(nameof(Keyboard.Text), false)]
		[InlineData(nameof(Keyboard.Url), false)]
		public async Task ValidateEmailKeyboard(string keyboardName, bool expected)
		{
			var keyboard = (Keyboard)typeof(Keyboard).GetProperty(keyboardName).GetValue(null);

			var entryStub = new EntryStub() { Keyboard = keyboard };

			await ValidatePropertyInitValue(entryStub, () => expected, GetNativeIsEmailKeyboard, expected);
		}

		[Theory(DisplayName = "Validates Telephone Keyboard")]
		[InlineData(nameof(Keyboard.Chat), false)]
		[InlineData(nameof(Keyboard.Default), false)]
		[InlineData(nameof(Keyboard.Email), false)]
		[InlineData(nameof(Keyboard.Numeric), false)]
		[InlineData(nameof(Keyboard.Plain), false)]
		[InlineData(nameof(Keyboard.Telephone), true)]
		[InlineData(nameof(Keyboard.Text), false)]
		[InlineData(nameof(Keyboard.Url), false)]
		public async Task ValidateTelephoneKeyboard(string keyboardName, bool expected)
		{
			var keyboard = (Keyboard)typeof(Keyboard).GetProperty(keyboardName).GetValue(null);

			var entryStub = new EntryStub() { Keyboard = keyboard };

			await ValidatePropertyInitValue(entryStub, () => expected, GetNativeIsTelephoneKeyboard, expected);
		}

		[Theory(DisplayName = "Validates Url Keyboard")]
		[InlineData(nameof(Keyboard.Chat), false)]
		[InlineData(nameof(Keyboard.Default), false)]
		[InlineData(nameof(Keyboard.Email), false)]
		[InlineData(nameof(Keyboard.Numeric), false)]
		[InlineData(nameof(Keyboard.Plain), false)]
		[InlineData(nameof(Keyboard.Telephone), false)]
		[InlineData(nameof(Keyboard.Text), false)]
		[InlineData(nameof(Keyboard.Url), true)]
		public async Task ValidateUrlKeyboard(string keyboardName, bool expected)
		{
			var keyboard = (Keyboard)typeof(Keyboard).GetProperty(keyboardName).GetValue(null);

			var entryStub = new EntryStub() { Keyboard = keyboard };

			await ValidatePropertyInitValue(entryStub, () => expected, GetNativeIsUrlKeyboard, expected);
		}

		[Theory(DisplayName = "Validates Text Keyboard")]
		[InlineData(nameof(Keyboard.Chat), false)]
		[InlineData(nameof(Keyboard.Default), false)]
		[InlineData(nameof(Keyboard.Email), false)]
		[InlineData(nameof(Keyboard.Numeric), false)]
		[InlineData(nameof(Keyboard.Plain), false)]
		[InlineData(nameof(Keyboard.Telephone), false)]
		[InlineData(nameof(Keyboard.Text), true)]
		[InlineData(nameof(Keyboard.Url), false)]
		public async Task ValidateTextKeyboard(string keyboardName, bool expected)
		{
			var keyboard = (Keyboard)typeof(Keyboard).GetProperty(keyboardName).GetValue(null);

			var entryStub = new EntryStub() { Keyboard = keyboard };

			await ValidatePropertyInitValue(entryStub, () => expected, GetNativeIsTextKeyboard, expected);
		}

		[Theory(DisplayName = "Validates Chat Keyboard")]
		[InlineData(nameof(Keyboard.Chat), true)]
		[InlineData(nameof(Keyboard.Default), false)]
		[InlineData(nameof(Keyboard.Email), false)]
		[InlineData(nameof(Keyboard.Numeric), false)]
		[InlineData(nameof(Keyboard.Plain), false)]
		[InlineData(nameof(Keyboard.Telephone), false)]
		[InlineData(nameof(Keyboard.Text), false)]
		[InlineData(nameof(Keyboard.Url), false)]
		public async Task ValidateChatKeyboard(string keyboardName, bool expected)
		{
			var keyboard = (Keyboard)typeof(Keyboard).GetProperty(keyboardName).GetValue(null);

			var entryStub = new EntryStub() { Keyboard = keyboard };

			await ValidatePropertyInitValue(entryStub, () => expected, GetNativeIsChatKeyboard, expected);
		}

		[Theory(DisplayName = "MaxLength Initializes Correctly")]
		[InlineData(2)]
		[InlineData(5)]
		[InlineData(8)]
		[InlineData(10)]
		public async Task MaxLengthInitializesCorrectly(int maxLength)
		{
			const string text = "Lorem ipsum dolor sit amet";
			var expectedText = text.Substring(0, maxLength);

			var entry = new EntryStub()
			{
				MaxLength = maxLength,
				Text = text
			};

			var nativeText = await GetValueAsync(entry, GetNativeText);

			Assert.Equal(expectedText, nativeText);
			Assert.Equal(expectedText, entry.Text);
		}

		[Fact(DisplayName = "Negative MaxLength Does Not Clip")]
		public async Task NegativeMaxLengthDoesNotClip()
		{
			const string text = "Lorem ipsum dolor sit amet";

			var entry = new EntryStub()
			{
				MaxLength = -1,
			};

			var nativeText = await GetValueAsync(entry, handler =>
			{
				entry.Text = text;

				return GetNativeText(handler);
			});

			Assert.Equal(text, nativeText);
			Assert.Equal(text, entry.Text);
		}

		[Theory(DisplayName = "MaxLength Clips Native Text Correctly")]
		[InlineData(2)]
		[InlineData(5)]
		[InlineData(8)]
		[InlineData(10)]
		public async Task MaxLengthClipsNativeTextCorrectly(int maxLength)
		{
			const string text = "Lorem ipsum dolor sit amet";
			var expectedText = text.Substring(0, maxLength);

			var entry = new EntryStub()
			{
				MaxLength = maxLength,
			};

			var nativeText = await GetValueAsync(entry, handler =>
			{
				entry.Text = text;

				return GetNativeText(handler);
			});

			Assert.Equal(expectedText, nativeText);
			Assert.Equal(expectedText, entry.Text);
		}

		[Theory(DisplayName = "Updating Font Does Not Affect CharacterSpacing")]
		[InlineData(10, 20)]
		[InlineData(20, 10)]
		public async Task FontDoesNotAffectCharacterSpacing(double initialSize, double newSize)
		{
			var entry = new EntryStub
			{
				Text = "This is TEXT!",
				CharacterSpacing = 5,
				Font = Font.SystemFontOfSize(initialSize)
			};

			await ValidateUnrelatedPropertyUnaffected(
				entry,
				GetNativeCharacterSpacing,
				nameof(IEntry.Font),
				() => entry.Font = Font.SystemFontOfSize(newSize));
		}

		[Theory(DisplayName = "Updating Text Does Not Affect CharacterSpacing")]
		[InlineData("Short", "Longer Text")]
		[InlineData("Long thext here", "Short")]
		public async Task TextDoesNotAffectCharacterSpacing(string initialText, string newText)
		{
			var entry = new EntryStub
			{
				Text = initialText,
				CharacterSpacing = 5,
			};

			await ValidateUnrelatedPropertyUnaffected(
				entry,
				GetNativeCharacterSpacing,
				nameof(IEntry.Text),
				() => entry.Text = newText);
		}

		[Theory(DisplayName = "Updating Font Does Not Affect HorizontalTextAlignment")]
		[InlineData(10, 20)]
		[InlineData(20, 10)]
		public async Task FontDoesNotAffectHorizontalTextAlignment(double initialSize, double newSize)
		{
			var entry = new EntryStub
			{
				Text = "This is TEXT!",
				HorizontalTextAlignment = TextAlignment.Center,
				Font = Font.SystemFontOfSize(initialSize),
			};

			await ValidateUnrelatedPropertyUnaffected(
				entry,
				GetNativeHorizontalTextAlignment,
				nameof(IEntry.Font),
				() => entry.Font = Font.SystemFontOfSize(newSize));
		}

		[Theory(DisplayName = "Updating Text Does Not Affect HorizontalTextAlignment")]
		[InlineData("Short", "Longer Text")]
		[InlineData("Long thext here", "Short")]
		public async Task TextDoesNotAffectHorizontalTextAlignment(string initialText, string newText)
		{
			var entry = new EntryStub
			{
				Text = initialText,
				HorizontalTextAlignment = TextAlignment.Center,
			};

			await ValidateUnrelatedPropertyUnaffected(
				entry,
				GetNativeHorizontalTextAlignment,
				nameof(IEntry.Text),
				() => entry.Text = newText);
		}

		[Theory(DisplayName = "Updating MaxLength Does Not Affect HorizontalTextAlignment")]
		[InlineData(5, 20)]
		[InlineData(20, 5)]
		public async Task MaxLengthDoesNotAffectHorizontalTextAlignment(int initialSize, int newSize)
		{
			var entry = new EntryStub
			{
				Text = "This is TEXT!",
				HorizontalTextAlignment = TextAlignment.Center,
				MaxLength = initialSize,
			};

			await ValidateUnrelatedPropertyUnaffected(
				entry,
				GetNativeHorizontalTextAlignment,
				nameof(IEntry.MaxLength),
				() => entry.MaxLength = newSize);
		}

		[Theory(DisplayName = "Updating CharacterSpacing Does Not Affect HorizontalTextAlignment")]
		[InlineData(1, 5)]
		[InlineData(5, 1)]
		public async Task CharacterSpacingDoesNotAffectHorizontalTextAlignment(int initialSize, int newSize)
		{
			var entry = new EntryStub
			{
				Text = "This is TEXT!",
				HorizontalTextAlignment = TextAlignment.Center,
				CharacterSpacing = initialSize,
			};

			await ValidateUnrelatedPropertyUnaffected(
				entry,
				GetNativeHorizontalTextAlignment,
				nameof(IEntry.CharacterSpacing),
				() => entry.CharacterSpacing = newSize);
		}

		[Theory(DisplayName = "CursorPosition Initializes Correctly")]
		[InlineData(0)]
		public async Task CursorPositionInitializesCorrectly(int initialPosition)
		{
			var entry = new EntryStub
			{
				Text = "This is TEXT!",
				CursorPosition = initialPosition
			};

			await ValidatePropertyInitValue(entry, () => entry.CursorPosition, GetNativeCursorPosition, initialPosition);
		}

		[Theory(DisplayName = "CursorPosition Updates Correctly")]
		[InlineData(2, 5)]
		public async Task CursorPositionUpdatesCorrectly(int setValue, int unsetValue)
		{
			string text = "This is TEXT!";

			var entry = new EntryStub
			{
				Text = text,
			};

			await ValidatePropertyUpdatesValue(
				entry,
				nameof(IEntry.CursorPosition),
				GetNativeCursorPosition,
				setValue,
				unsetValue
			);
		}

		[Theory(DisplayName = "CursorPosition is Capped to Text's Length")]
		[InlineData(30)]
		public async Task CursorPositionIsCapped(int initialPosition)
		{
			string text = "This is TEXT!";

			var entry = new EntryStub
			{
				Text = text,
				CursorPosition = initialPosition
			};

			int actualPosition = await GetValueAsync(entry, GetNativeCursorPosition);

			Assert.Equal(text.Length, actualPosition);
		}

		[Theory(DisplayName = "SelectionLength Initializes Correctly")]
		[InlineData(0)]
		public async Task SelectionLengthInitializesCorrectly(int initialLength)
		{
			var entry = new EntryStub
			{
				Text = "This is TEXT!",
				SelectionLength = initialLength
			};

			await ValidatePropertyInitValue(entry, () => entry.SelectionLength, GetNativeSelectionLength, initialLength);
		}

		[Theory(DisplayName = "SelectionLength Updates Correctly")]
		[InlineData(2, 5)]
		public async Task SelectionLengthUpdatesCorrectly(int setValue, int unsetValue)
		{
			string text = "This is TEXT!";

			var entry = new EntryStub
			{
				Text = text,
			};

			await ValidatePropertyUpdatesValue(
				entry,
				nameof(IEntry.SelectionLength),
				GetNativeSelectionLength,
				setValue,
				unsetValue
			);
		}

		[Theory(DisplayName = "SelectionLength is Capped to Text Length")]
		[InlineData(30)]
		public async Task SelectionLengthIsCapped(int selectionLength)
		{
			string text = "This is TEXT!";

			var entry = new EntryStub
			{
				Text = text,
				SelectionLength = selectionLength
			};

			var actualLength = await GetValueAsync(entry, GetNativeSelectionLength);

			Assert.Equal(text.Length, actualLength);
		}


		[Category(TestCategory.Entry)]
		public class EntryTextInputTests : TextInputHandlerTests<EntryHandler, EntryStub>
		{
			protected override void SetNativeText(EntryHandler entryHandler, string text)
			{
				EntryHandlerTests.SetNativeText(entryHandler, text);
			}
			protected override int GetCursorStartPosition(EntryHandler entryHandler)
			{
				return EntryHandlerTests.GetCursorStartPosition(entryHandler);
			}

			protected override void UpdateCursorStartPosition(EntryHandler entryHandler, int position)
			{
				EntryHandlerTests.UpdateCursorStartPosition(entryHandler, position);
			}
		}

	}
}
