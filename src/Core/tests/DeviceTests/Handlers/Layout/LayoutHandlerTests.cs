using System;
using System.Threading.Tasks;
using Microsoft.Maui.DeviceTests.Stubs;
using Microsoft.Maui.Handlers;
using Xunit;

namespace Microsoft.Maui.DeviceTests.Handlers.Layout
{
	[Category(TestCategory.Layout)]
	public partial class LayoutHandlerTests : HandlerTestBase<LayoutHandler, LayoutStub>
	{
		[Fact(DisplayName = "Empty layout")]
		public async Task EmptyLayout()
		{
			var layout = new LayoutStub();
			await ValidatePropertyInitValue(layout, () => layout.Count, GetNativeChildCount, 0);
		}

		[Fact(DisplayName = "Handler view count matches layout view count")]
		public async Task HandlerViewCountMatchesLayoutViewCount()
		{
			var layout = new LayoutStub();

			layout.Add(new SliderStub());
			layout.Add(new SliderStub());

			await ValidatePropertyInitValue(layout, () => layout.Count, GetNativeChildCount, 2);
		}

		[Fact(DisplayName = "Handler removes child from native layout")]
		public async Task HandlerRemovesChildFromNativeLayout()
		{
			var layout = new LayoutStub();
			var slider = new SliderStub();
			layout.Add(slider);

			var handler = await CreateHandlerAsync(layout);

			var children = await InvokeOnMainThreadAsync(() =>
			{
				return GetNativeChildren(handler);
			});

			Assert.Equal(1, children.Count);
			Assert.Same(slider.Handler.NativeView, children[0]);

			var count = await InvokeOnMainThreadAsync(() =>
			{
				handler.Remove(slider);
				return GetNativeChildCount(handler);
			});

			Assert.Equal(0, count);
		}

		[Fact(DisplayName = "DisconnectHandler removes child from native layout")]
		public async Task DisconnectHandlerRemovesChildFromNativeLayout()
		{
			var layout = new LayoutStub();
			var slider = new SliderStub();
			layout.Add(slider);

			var handler = await CreateHandlerAsync(layout);

			var count = await InvokeOnMainThreadAsync(() =>
			{
				var nativeView = layout.Handler.NativeView;
				layout.Handler.DisconnectHandler();
				return GetNativeChildCount(nativeView);
			});

			Assert.Equal(0, count);
		}

		[Fact]
		public async Task ClearRemovesChildrenFromNativeLayout()
		{
			var layout = new LayoutStub();
			var slider = new SliderStub();
			var button = new ButtonStub();

			layout.Add(slider);
			layout.Add(button);

			var handler = await CreateHandlerAsync(layout);

			var children = await InvokeOnMainThreadAsync(() =>
			{
				return GetNativeChildren(handler);
			});

			Assert.Equal(2, children.Count);
			Assert.Same(slider.Handler.NativeView, children[0]);
			Assert.Same(button.Handler.NativeView, children[1]);

			var count = await InvokeOnMainThreadAsync(() =>
			{
				handler.Clear();
				return GetNativeChildCount(handler);
			});

			Assert.Equal(0, count);
		}

		[Fact]
		public async Task InsertAddsChildToNativeLayout()
		{
			var layout = new LayoutStub();
			var slider = new SliderStub();
			var button = new ButtonStub();

			layout.Add(slider);

			var handler = await CreateHandlerAsync(layout);

			var children = await InvokeOnMainThreadAsync(() =>
			{
				return GetNativeChildren(handler);
			});

			Assert.Equal(1, children.Count);
			Assert.Same(slider.Handler.NativeView, children[0]);

			children = await InvokeOnMainThreadAsync(() =>
			{
				layout.Insert(0, button);
				handler.Insert(0, button);
				return GetNativeChildren(handler);
			});

			Assert.Equal(2, children.Count);
			Assert.Same(button.Handler.NativeView, children[0]);
			Assert.Same(slider.Handler.NativeView, children[1]);
		}

		[Fact]
		public async Task UpdateNativeLayout()
		{
			var layout = new LayoutStub();
			var slider = new SliderStub();
			var button = new ButtonStub();

			layout.Add(slider);

			var handler = await CreateHandlerAsync(layout);

			var children = await InvokeOnMainThreadAsync(() =>
			{
				return GetNativeChildren(handler);
			});

			Assert.Equal(1, children.Count);
			Assert.Same(slider.Handler.NativeView, children[0]);

			children = await InvokeOnMainThreadAsync(() =>
			{
				layout[0] = button;
				handler.Update(0, button);
				return GetNativeChildren(handler);
			});

			Assert.Equal(1, children.Count);
			Assert.Same(button.Handler.NativeView, children[0]);
		}

		[Fact]
		public async Task ContainerViewAddedToLayout()
		{
			var layout = new LayoutStub();
			var addedSlider = new ButtonWithContainerStub();
			var insertedSlider = new ButtonWithContainerStub();

			layout.Add(addedSlider);

			var handler = await CreateHandlerAsync(layout);

			var children = await InvokeOnMainThreadAsync(() =>
			{
				return GetNativeChildren(handler);
			});

			Assert.Equal(1, children.Count);
			Assert.Same(addedSlider.Handler.ContainerView, children[0]);

			children = await InvokeOnMainThreadAsync(() =>
			{
				layout.Insert(0, insertedSlider);
				handler.Insert(0, insertedSlider);
				return GetNativeChildren(handler);
			});

			Assert.Equal(2, children.Count);
			Assert.Same(insertedSlider.Handler.ContainerView, children[0]);
			Assert.Same(addedSlider.Handler.ContainerView, children[1]);
		}

		LabelStub CreateZTestLabel(int zIndex)
		{
			return new LabelStub() { Text = zIndex.ToString("000"), ZIndex = zIndex };
		}

		async Task AddZTestLabels(ILayout layout, params ILabel[] labels)
		{
			foreach (var label in labels)
			{
				layout.Add(label);
				await InitZTestLabel(label);
			}
		}

		async Task InitZTestLabel(ILabel label)
		{
			await InvokeOnMainThreadAsync(() =>
			{
				var handler = new LabelHandler();
				InitializeViewHandler(label, handler, MauiContext);
			});
		}

		async Task AssertZIndexOrder(LayoutHandler handler)
		{
			var children = await InvokeOnMainThreadAsync(() =>
			{
				return GetNativeChildren(handler);
			});

			await AssertZIndexOrder(children);
		}

		[Fact]
		public async Task NativeChildrenStartInZIndexOrder()
		{
			var layout = new LayoutStub();

			var view0 = CreateZTestLabel(zIndex: 1);
			var view1 = CreateZTestLabel(zIndex: 0);

			await AddZTestLabels(layout, view0, view1);

			var handler = await CreateHandlerAsync(layout);
			await AssertZIndexOrder(handler);
		}

		[Fact]
		public async Task AddChildPreservesZIndexOrder()
		{
			var layout = new LayoutStub();

			var view0 = CreateZTestLabel(zIndex: 10);
			var view1 = CreateZTestLabel(zIndex: 20);

			await AddZTestLabels(layout, view0, view1);

			var handler = await CreateHandlerAsync(layout);

			// Add the third item with zIndex that should put it at the beginning of the native child collection
			var view2 = CreateZTestLabel(zIndex: 0);
			await AddZTestLabels(layout, view2);

			// The stubs won't trigger the actual handler commands, so we'll do it manually
			await InvokeOnMainThreadAsync(() => handler.Invoke(nameof(ILayoutHandler.Add), new LayoutHandlerUpdate(2, view2)));

			// Verify the views are in the correct order
			await AssertZIndexOrder(handler);
		}

		[Fact]
		public async Task InsertChildPreservesZIndexOrder()
		{
			var layout = new LayoutStub();

			var view0 = CreateZTestLabel(zIndex: 10);
			var view1 = CreateZTestLabel(zIndex: 20);

			await AddZTestLabels(layout, view0, view1);

			var handler = await CreateHandlerAsync(layout);

			// Add the third item with zIndex that should put it at the beginning of the native child collection
			var view2 = CreateZTestLabel(zIndex: 0);
			await InitZTestLabel(view2);
			layout.Insert(1, view2);

			// The stubs won't trigger the actual handler commands, so we'll do it manually
			await InvokeOnMainThreadAsync(() => handler.Invoke(nameof(ILayoutHandler.Insert), new LayoutHandlerUpdate(1, view2)));

			// Verify the views are in the correct order
			await AssertZIndexOrder(handler);
		}

		[Fact]
		public async Task UpdateChildPreservesZIndexOrder()
		{
			var layout = new LayoutStub();

			var view0 = CreateZTestLabel(zIndex: 10);
			var view1 = CreateZTestLabel(zIndex: 20);

			await AddZTestLabels(layout, view0, view1);

			var handler = await CreateHandlerAsync(layout);

			// Add the third item with zIndex that should put it at the beginning of the native child collection
			var view2 = CreateZTestLabel(zIndex: 0);
			await InitZTestLabel(view2);
			layout[1] = view2;

			// The stubs won't trigger the actual handler commands, so we'll do it manually
			await InvokeOnMainThreadAsync(() => handler.Invoke(nameof(ILayoutHandler.Update), new LayoutHandlerUpdate(1, view2)));

			// Verify the views are in the correct order
			await AssertZIndexOrder(handler);
		}

		[Fact]
		public async Task UpdateChildZindexPreservesZIndexOrder()
		{
			var layout = new LayoutStub();

			var view0 = CreateZTestLabel(zIndex: 10);
			var view1 = CreateZTestLabel(zIndex: 20);

			await AddZTestLabels(layout, view0, view1);

			var handler = await CreateHandlerAsync(layout);

			// Update the z-index for view0 in a way that should move it to the end of the collection
			view0.Text = "030";
			view0.ZIndex = 30;

			// The stubs won't trigger the actual handler commands, so we'll do it manually
			await InvokeOnMainThreadAsync(() => view0.Handler.UpdateValue(nameof(ILabel.Text)));
			await InvokeOnMainThreadAsync(() => handler.Invoke(nameof(ILayoutHandler.UpdateZIndex), view0));

			// Verify the views are in the correct order
			await AssertZIndexOrder(handler);
		}
	}
}