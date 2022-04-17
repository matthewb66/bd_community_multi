﻿using AWebView = Android.Webkit.WebView;

namespace Microsoft.Maui.Platform
{
	public static class WebViewExtensions
	{
		public static void UpdateSource(this AWebView nativeWebView, IWebView webView)
		{
			nativeWebView.UpdateSource(webView, null);
		}

		public static void UpdateSource(this AWebView nativeWebView, IWebView webView, IWebViewDelegate? webViewDelegate)
		{
			if (webViewDelegate != null)
				webView.Source?.Load(webViewDelegate);
		}

		public static void UpdateSettings(this AWebView nativeWebView, IWebView webView, bool javaScriptEnabled, bool domStorageEnabled)
		{
			if (nativeWebView.Settings == null)
				return;

			nativeWebView.Settings.JavaScriptEnabled = javaScriptEnabled;
			nativeWebView.Settings.DomStorageEnabled = domStorageEnabled;
		}
	}
}