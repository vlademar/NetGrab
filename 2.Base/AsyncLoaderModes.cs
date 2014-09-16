using System;
using System.IO;
using System.IO.Compression;

namespace NetGrab
{
    public partial class AsyncLoaderHelper
    {
        public delegate void SaveFileCompleteCallback(int fileLength, Exception error);
        public delegate void LoadStringCompleteCallback(string s, string actualUrl, Exception error);


        private interface IAsyncLoaderHelperMode
        {
            void OnFail(Exception e);
            void OnSuccess(RequestState requestState);
        }

        private class IAsyncLoaderHelperLoadString : IAsyncLoaderHelperMode
        {
            private readonly LoadStringCompleteCallback callback;

            public IAsyncLoaderHelperLoadString(LoadStringCompleteCallback _callback)
            {
                callback = _callback;
            }

            public void OnSuccess(RequestState state)
            {
                Stream decodedStream = null;
                StreamReader responseSr = null;

                try
                {
                    if (state.response.ContentEncoding.ToLower().Contains("gzip"))
                        decodedStream = new GZipStream(state.resultStream, CompressionMode.Decompress);
                    else if (state.response.ContentEncoding.ToLower().Contains("deflate"))
                        decodedStream = new DeflateStream(state.resultStream, CompressionMode.Decompress);
                    else
                        decodedStream = state.resultStream;

                    responseSr = new StreamReader(decodedStream);
                    var resultString = responseSr.ReadToEnd();
                    callback.Invoke(resultString, state.response.ResponseUri.ToString(), null);
                }
                catch (Exception e)
                {
                    OnFail(e);
                }
                finally
                {
                    if (decodedStream != null)
                        decodedStream.Close();

                    if (responseSr != null)
                        responseSr.Close();
                }
            }

            public void OnFail(Exception e)
            {
                callback.Invoke(null, string.Empty, e);
            }
        }

        private class IAsyncLoaderHelperSaveFile : IAsyncLoaderHelperMode
        {
            private readonly string path;
            private readonly SaveFileCompleteCallback callback;

            public IAsyncLoaderHelperSaveFile(string _path, SaveFileCompleteCallback _callback)
            {
                path = _path;
                callback = _callback;
            }

            public void OnSuccess(RequestState state)
            {
                FileStream fileStream = null;

                try
                {
                    fileStream = File.OpenWrite(path);
                    state.resultStream.CopyTo(fileStream);
                    callback.Invoke(state.resultStreamLength, null);
                }
                catch (Exception e)
                {
                    OnFail(e);
                }
                finally
                {
                    if (fileStream != null)
                    {
                        fileStream.Flush(true);
                        fileStream.Close();
                    }
                }
            }

            public void OnFail(Exception e)
            {
                callback.Invoke(-1, e);
            }
        }

    }
}
