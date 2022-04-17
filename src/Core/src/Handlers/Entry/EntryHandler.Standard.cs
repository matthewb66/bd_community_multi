﻿using System;

namespace Microsoft.Maui.Handlers
{
	public partial class EntryHandler : ViewHandler<IEntry, object>
	{
		protected override object CreateNativeView() => throw new NotImplementedException();

		public static void MapText(IViewHandler handler, IEntry entry) { }
		public static void MapTextColor(IViewHandler handler, IEntry entry) { }
		public static void MapIsPassword(IViewHandler handler, IEntry entry) { }
		public static void MapHorizontalTextAlignment(IViewHandler handler, IEntry entry) { }
		public static void MapVerticalTextAlignment(IViewHandler handler, IEntry entry) { }
		public static void MapIsTextPredictionEnabled(IViewHandler handler, IEntry entry) { }
		public static void MapMaxLength(IViewHandler handler, IEntry entry) { }
		public static void MapPlaceholder(IViewHandler handler, IEntry entry) { }
		public static void MapPlaceholderColor(IViewHandler handler, IEntry entry) { }
		public static void MapIsReadOnly(IViewHandler handler, IEntry entry) { }
		public static void MapKeyboard(IViewHandler handler, IEntry entry) { }
		public static void MapFont(IViewHandler handler, IEntry entry) { }
		public static void MapReturnType(IViewHandler handler, IEntry entry) { }
		public static void MapClearButtonVisibility(IViewHandler handler, IEntry entry) { }
		public static void MapCharacterSpacing(IViewHandler handler, IEntry entry) { }
		public static void MapCursorPosition(IViewHandler handler, IEntry entry) { }
		public static void MapSelectionLength(IViewHandler handler, IEntry entry) { }
	}
}