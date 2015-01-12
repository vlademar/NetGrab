using System.IO;
using System.Net.Sockets;

namespace NetGrab.TOR
{
    class TcpListenerIOState
    {
        public byte[] Buffer { get; private set; }
        public TcpListener Listener { get; set; }
        public TcpClient Client { get; set; }
        public Stream ClientIoStream { get; set; }

        public TcpListenerIOState()
        {
            Buffer = new byte[4096];
        }
    }
}
