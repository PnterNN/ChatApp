using ChatServer.NET.IO;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChatApp.NET
{
    class SqlServer
    {
        public TcpClient _sqlclient = new TcpClient();

        public event Action isValidUserEvent;

        public PacketReader PacketReader;

        public SqlServer()
        {
            _sqlclient = new TcpClient();
        }
        public void loginToSqlServer(string username, string password)
        {
            ConnectToSqlServer(username);
            var connectPacket = new PacketBuilder();
            connectPacket.WriteOpCode(3);
            connectPacket.WriteMessage(username);
            connectPacket.WriteMessage(password);
            _sqlclient.Client.Send(connectPacket.GetPacketBytes());
        }
        public void ConnectToSqlServer(string username)
        {
            if (!_sqlclient.Connected)
            {
                try
                {
                    _sqlclient.Connect("127.0.0.1", 37336);
                }
                catch
                {
                    MessageBox.Show("Sunucu bulunumadı, uygulama kapanıyor...");
                    Application.Current.Dispatcher.Invoke(() => Application.Current.Shutdown());
                    return;
                }
                
                PacketReader = new PacketReader(_sqlclient.GetStream());


                var connectPacket = new PacketBuilder();
                connectPacket.WriteOpCode(0);
                connectPacket.WriteMessage(username);
                _sqlclient.Client.Send(connectPacket.GetPacketBytes());
                ReadPackets();
            }
        }
        private void ReadPackets()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        var opcode = PacketReader.ReadByte();
                        switch (opcode)
                        {
                            case 3:
                                isValidUserEvent?.Invoke();
                                break;
                            default:
                                Console.WriteLine("Unknown opcode: " + opcode);
                                break;
                        }
                    }
                    catch(Exception ex)
                    {
                        return;
                    }
                
                }
            });
        }

    }
}
