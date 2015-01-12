using System.Net;
using System.Net.Sockets;

namespace NetGrab.TOR
{
    class SocksProxy
    {
        public string ProxyAddress { get; set; }
        public ushort ProxyPort { get; set; }
        public string Login { get; set; }
        public string Pass { get; set; }

        public TcpClient MasterClient { get; set; }
    }
}
