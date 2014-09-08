using System.Net;

namespace NetGrab
{
    interface ITaskHost
    {
        IWebProxy Proxy { get; set; }
    }
}