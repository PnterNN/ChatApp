using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ChatApp.NET.IO
{
    delegate void DataArrivedHandler(byte[] data, string senderIP);

    class UdpReceiver
    {
        UdpClient sock;
        IPEndPoint endPoint;

        public event DataArrivedHandler DataArrived;

        bool connected = false;
        Dispatcher _dispatcher;

        public UdpReceiver(int port) : this(string.Empty, port)
        {

        }

        public UdpReceiver(string address, int port)
        {
            _dispatcher = App.Current.Dispatcher;

            IPAddress ipAddress;
            if (address.Length == 0)
                ipAddress = IPAddress.Any;
            else
                ipAddress = IPAddress.Parse(address);

            endPoint = new IPEndPoint(ipAddress, port);
            sock = new UdpClient(port);
        }

        public void Start()
        {
            connected = true;
            ListenerThreadState state = new ListenerThreadState() { EndPoint = endPoint };
            ThreadPool.QueueUserWorkItem(this.ListenerThread, state);
        }

        public void Close()
        {
            connected = false;
            sock.Close();
        }

        void ListenerThread(object state)
        {
            var listenerThreadState = (ListenerThreadState)state;
            var endPoint = listenerThreadState.EndPoint;
            try
            {
                while (connected)
                {
                    byte[] data = this.sock.Receive(ref endPoint);
                    OnDataArrived(data, endPoint.Address.ToString());
                }
            }
            catch (SocketException)
            {
                // usually not a problem - just means we have disconnected
            }
        }

        void OnDataArrived(byte[] data, string senderIP)
        {
            if (DataArrived != null)
            {
                _dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                {
                    DataArrived(data, senderIP);
                }));
            }
        }
    }

    class ListenerThreadState
    {
        public IPEndPoint EndPoint { get; set; }
    }
}
