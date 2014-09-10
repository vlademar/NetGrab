using System.Net;

namespace NetGrab
{
    interface ITaskHost
    {
        WebProxy Proxy { get; set; }
    }
}