using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Net;
using System.Net.Sockets;

namespace ChatApp.NET.IO
{
    class UdpSender
    {
        Socket socket;
        IPEndPoint endPoint;

        public UdpSender(string address, int port)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            endPoint = new IPEndPoint(IPAddress.Parse(address), port);
        }

        public void Send(byte[] data)
        {
            socket.SendTo(data, endPoint);
        }

    }
}
