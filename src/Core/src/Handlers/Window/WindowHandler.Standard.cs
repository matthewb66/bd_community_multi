using System;

namespace Microsoft.Maui.Handlers
{
	public partial class WindowHandler : ElementHandler<IWindow, object>
	{
		protected override object CreateNativeElement() => throw new NotImplementedException();

		public static void MapTitle(WindowHandler handler, IWindow window) { }

		public static void MapContent(WindowHandler handler, IWindow window) { }
	}
}