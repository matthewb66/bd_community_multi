﻿using System.Threading;
using System.Threading.Tasks;
using ObjCRuntime;
using UIKit;

namespace Microsoft.Maui.DeviceTests.Stubs
{
	class CustomImageSourceServiceStub : IImageSourceService<ICustomImageSourceStub>
	{
		readonly CustomImageCacheStub _cache;

		public CustomImageSourceServiceStub(CustomImageCacheStub cache)
		{
			_cache = cache;
		}

		public Task<IImageSourceServiceResult<UIImage>> GetImageAsync(IImageSource imageSource, float scale = 1, CancellationToken cancellationToken = default) =>
			GetImageAsync((ICustomImageSourceStub)imageSource, scale, cancellationToken);

		public Task<IImageSourceServiceResult<UIImage>> GetImageAsync(ICustomImageSourceStub imageSource, float scale = 1, CancellationToken cancellationToken = default)
		{
			var color = imageSource.Color;

			var drawable = _cache.Get(color);

			var result = new ImageSourceServiceResult(drawable, () => _cache.Return(color));

			return Task.FromResult<IImageSourceServiceResult<UIImage>>(result);
		}
	}
}