using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace NetGrab
{
    class KnowyourmemeComLoader : LoaderBase
    {
        private const string staticUrlPart = "http://knowyourmeme.com/photos/";
        private static readonly SortedList<string, int> catalogs = new SortedList<string, int>();
        private static readonly Regex searchRegex = new Regex("(?<url>http\\://i\\d{1}\\.kym-cdn\\.com/photos/images/original/\\d{3}/\\d{3}/\\d{3}/(?<name>[\\-\\w\\.]+)\\.(?<ext>\\w+))");

        public ILogger Logger { get; set; }
        public string DownloadPathBase { get; set; }
        public string TaskUrlSuffix { get; set; }

        protected override void DoWork()
        {
            AsyncLoaderHelper.LoadStringAsync(staticUrlPart + TaskUrlSuffix, Proxy, TitlePageDownloaded);
        }

        private void TitlePageDownloaded(string s, string outUrl, Exception error)
        {
            if (error != null)
            {
                Logger.Add(LoaderId.ToString("D2") + " -> " + TaskUrlSuffix + " -> FILE1 -> EXCEPTION : " + error.Message);
                OnFinished();
                return;
            }

            var i = outUrl.IndexOf('-', outUrl.LastIndexOf('/'));


            var group = (i == -1) ? "-" : outUrl.Substring(i + 1);

            if (!catalogs.ContainsKey(group))
            {
                catalogs.Add(group, 0);
                Directory.CreateDirectory(Path.Combine(DownloadPathBase, group));
            }

            catalogs[group]++;

            var matches = searchRegex.Matches(s);

            if (matches.Count == 0)
            {
                Logger.Add(LoaderId.ToString("D2") + " -> " + TaskUrlSuffix + " -> NO MATCHES");
                OnFinished();
                return;
            }

            var m = matches[0];
            var url = m.Groups["url"].Value;
            var name = m.Groups["name"].Value;
            var ext = m.Groups["ext"].Value;

            name = name.Substring(0, Math.Min(name.Length, 60));
            var fileName = string.Format(@"{0}\{1}_{2}.{3}", Path.Combine(DownloadPathBase, group), TaskUrlSuffix, name, ext);

            AsyncLoaderHelper.SaveFileAsync(url, fileName, Proxy, FileDownloaded);
        }

        private void FileDownloaded(int fileLength, Exception error)
        {
            if (error != null)
                Logger.Add(LoaderId.ToString("D2") + " -> " + TaskUrlSuffix + " -> FILE2 -> EXCEPTION : " + error.Message);
            else
                Logger.Add(LoaderId.ToString("D2") + " -> " + TaskUrlSuffix + " -> OK : " + fileLength + "b");

            OnFinished();
        }
    }
}