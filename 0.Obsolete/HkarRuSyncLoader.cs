//using System;
//using System.IO;
//using System.Net;
//using System.Text.RegularExpressions;

//namespace NetGrab
//{
//    class HkarRuSyncLoader : SyncLoaderBase
//    {
//        private const string staticUrlPart = "http://hkar.ru/";
//        private Regex searchRegex = new Regex("\\<a\\s+href\\=\"(?<url>http\\://\\w+.hostingkartinok.com/uploads/images/\\d+/\\d+/\\w+\\.\\w+)\"\\s+class\\=\"lightbox\"\\s+title\\=\"(?<name>[\\w\\-_]+\\.\\w+)\\s+");

//        public override ILoader New()
//        {
//            return new HkarRuSyncLoader();
//        }

//        public override void DoWork(Task task)
//        {
//            try
//            {
//                string outUrl;
//                var _string = LoadTextFile(staticUrlPart + task.Suffix, out outUrl);

//                var matches = searchRegex.Matches(_string);

//                if (matches.Count == 0)
//                {
//                    taskHost.RaiseNextJob(task.Loader);
//                    return;
//                }

//                var m = matches[0];
//                var url = staticUrlPart + m.Groups["url"].Value;
//                var name = m.Groups["name"].Value;

//                var fileName = string.Format("./downloads/{0}_{1}.{2}",
//                    task.Suffix.Substring(task.Suffix.LastIndexOf('/') + 1),
//                    Path.GetFileNameWithoutExtension(name).Substring(0, Math.Min(20, Path.GetFileNameWithoutExtension(name).Length)),
//                    Path.GetExtension(name));

//                SaveFile(url, fileName);

//            }
//            catch (WebException) { }

//            taskHost.RaiseNextJob(task.Loader);
//        }
//    }
//}