using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace NetGrab
{
    class KnowyourmemeComSyncLoader : LoaderBase
    {
        private const string staticUrlPart = "http://knowyourmeme.com/photos/";
        private static readonly SortedList<string, int> catalogs = new SortedList<string, int>();

        private Regex searchRegex = new Regex("(?<url>http\\://i\\d{1}\\.kym-cdn\\.com/photos/images/original/\\d{3}/\\d{3}/\\d{3}/(?<name>[\\-\\w]+)\\.(?<ext>\\w+))");
        private Task task;


        public override ILoader New()
        {
            return new KnowyourmemeComSyncLoader();
        }

        public override void DoWork(Task task)
        {
            this.task = task;

            LoadStringAsync(staticUrlPart + task.Suffix, TitlePageDownloaded);
        }

        private void TitlePageDownloaded(string s, string outUrl, Exception error)
        {
            if (error != null)
            {
                //logger.Add(task.Loader.ToString("D2") + " -> " + task.Suffix + " -> FILE1 -> EXCEPTION : " + error.Message);
                taskHost.RaiseNextJob(task.Loader);
                return;
            }

            var i = outUrl.IndexOf('-', outUrl.LastIndexOf('/'));


            var group = (i == -1) ? "-" : outUrl.Substring(i + 1);

            if (!catalogs.ContainsKey(group))
            {
                catalogs.Add(group, 0);
                Directory.CreateDirectory(Path.Combine(downloadPathBase, group));
            }

            catalogs[group]++;

            var matches = searchRegex.Matches(s);

            if (matches.Count == 0)
            {
                //logger.Add(task.Loader.ToString("D2") + " -> " + task.Suffix + " -> NO MATCHES");

                taskHost.RaiseNextJob(task.Loader);
                return;
            }

            var m = matches[0];
            var url = m.Groups["url"].Value;
            var name = m.Groups["name"].Value;
            var ext = m.Groups["ext"].Value;

            name = name.Substring(0, Math.Min(name.Length, 60));
            var fileName = string.Format("{0}\\{1}_{2}.{3}", Path.Combine(downloadPathBase, group), task.Suffix, name, ext);

            SaveFileAsync(url, fileName, FileDownloaded);
        }

        private void FileDownloaded(int fileLength, Exception error)
        {
            //if (error != null)
                //logger.Add(task.Loader.ToString("D2") + " -> " + task.Suffix + " -> FILE2 -> EXCEPTION : " + error.Message);
            //else
                //logger.Add(task.Loader.ToString("D2") + " -> " + task.Suffix + " -> OK : " + fileLength + "b");

            taskHost.RaiseNextJob(task.Loader);
        }
    }
}