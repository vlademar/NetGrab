using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace NetGrab
{
    class KnowyourmemeComLoaderTaskGroup : ILoaderTaskGroup
    {
        private static readonly string DownloadSubdir = "Knowyourmeme";

        private readonly string downloadPathBase = Path.Combine(Properties.Settings.Default.DownloadPath, DownloadSubdir);
        private readonly NameGenAZ09 nameGen = new NameGenAZ09();

        public string StartSuffix { set { nameGen.Init(value); } }
        public string CurrentSuffix { get { return nameGen.CurrentSuffix; } }

        public bool HasNextTask { get { return true; } }

        public ILoader NewTaskLoader(ITaskHost taskHost, IWebProxy proxy, ILogger logger)
        {
            return new KnowyourmemeComLoader
            {
                LoaderTaskGroup = this,
                DownloadPathBase = downloadPathBase,
                TaskHost = taskHost,
                Proxy = proxy,
                Logger = logger
            };
        }

        public void ReinitLoader(ILoader loader)
        {
            var _loader = loader as KnowyourmemeComLoader;
            if (_loader == null)
                throw new Exception("loader !is KnowyourmemeComLoader");

            _loader.TaskUrlSuffix = nameGen.NextName();
        }
    }
}
