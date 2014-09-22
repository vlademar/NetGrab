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
        private const string DownloadSubdir = "Knowyourmeme";

        private readonly string downloadPathBase = Path.Combine(Properties.Settings.Default.DownloadPath, DownloadSubdir);
        private readonly INameGen nameGen = new NameGen09();

        public string StartSuffix { set { nameGen.Init(value); } }
        public string CurrentSuffix { get { return nameGen.CurrentSuffix; } }

        public bool HasNextTask { get { return true; } }

        public ILoader NewTaskLoader(ITaskHost taskHost, WebProxy proxy, ILogger logger)
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

        public string GetState()
        {
            var id = nameGen.Id - 100;
            return id > 0 ? id.ToString() : string.Empty;
        }

        public void SetState(string state)
        {
            if (!string.IsNullOrWhiteSpace(state))
                StartSuffix = state;
        }
    }
}
