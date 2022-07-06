using System;
using System.Threading.Tasks;
using System.Threading;
using CefSharp;
using CefSharp.OffScreen;
using System.IO;

namespace URFUParser
{
	internal class WEBHandler
	{

        public ChromiumWebBrowser CWB { get; private set; }
        public RequestContext RequestContext { get; private set; }
        private ManualResetEvent ManualResetEvent = new ManualResetEvent(false);

        public WEBHandler()
		{
			//url = "";
   //         pg = "";
            var settings = new CefSettings()
            {
                CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache"),
            };

            CefSharpSettings.ShutdownOnExit = true;
            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);

            RequestContext = new RequestContext();
            CWB = new ChromiumWebBrowser("", null, RequestContext);
            PageInitialize();
        }

		public async Task<string> GetHtmlPage(string url)
		{
            this.OpenUrl(url);
            return await this.CWB.GetSourceAsync();
        }

        private void OpenUrl(string url)
        {
            try
            {
                CWB.LoadingStateChanged += PageLoadingStateChanged;
                if (CWB.IsBrowserInitialized)
                {
                    CWB.Load(url);

                    bool isSignalled = ManualResetEvent.WaitOne(TimeSpan.FromSeconds(60));
                    ManualResetEvent.Reset();

                    if (!isSignalled)
                    {
                        CWB.Stop();
                    }
                }
            }
            catch (ObjectDisposedException)
            {}
            CWB.LoadingStateChanged -= PageLoadingStateChanged;
        }
        private void PageLoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (!e.IsLoading)
            {
                ManualResetEvent.Set();
            }
        }
        private void PageInitialize()
        {
            SpinWait.SpinUntil(() => CWB.IsBrowserInitialized);
        }


    }
}
