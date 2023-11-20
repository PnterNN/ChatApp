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
    class Server
    {
        public TcpClient _client = new TcpClient();

        public event Action connectedEvent;
        public event Action messageReceivedEvent;
        public event Action userDisconnectEvent;
        public event Action groupCreatedEvent;
        public event Action isValidUserEvent;

        public PacketReader PacketReader;

        public Server()
        {
            _client = new TcpClient();
        }
        public void loginToServer(string username, string password)
        {
            ConnectToServer(username);
            var connectPacket = new PacketBuilder();
            connectPacket.WriteOpCode(3);
            connectPacket.WriteMessage(username);
            connectPacket.WriteMessage(password);
            _client.Client.Send(connectPacket.GetPacketBytes());
            connectPacket._ms.Dispose();
        }
        public void ConnectToServer(string username)
        {
            if (!_client.Connected)
            {
                try
                {
                    _client.Connect("127.0.0.1", 9001);
                }
                catch
                {
                    MessageBox.Show("Sunucu bulunumadı, uygulama kapanıyor...");
                    Application.Current.Dispatcher.Invoke(() => Application.Current.Shutdown());
                    return;
                }
                
                PacketReader = new PacketReader(_client.GetStream());


                var connectPacket = new PacketBuilder();
                connectPacket.WriteOpCode(0);
                connectPacket.WriteMessage(username);
                _client.Client.Send(connectPacket.GetPacketBytes());
                connectPacket._ms.Dispose();
                ReadPackets(PacketReader._ns);
                
            }
        }
        private void ReadPackets(NetworkStream packet)
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
                            case 1:
                                connectedEvent?.Invoke();
                                break;
                            case 2:
                                groupCreatedEvent?.Invoke();
                                break;
                            case 3:
                                isValidUserEvent?.Invoke();
                                break;
                            case 5:
                                messageReceivedEvent?.Invoke();
                                break;
                            case 10:
                                userDisconnectEvent?.Invoke();
                                break;
                            default:
                                Console.WriteLine("Unknown opcode: " + opcode);
                                break;
                        }
                    }
                    catch(Exception ex)
                    {
                        packet.Dispose();
                        MessageBox.Show("Sunucu Çöktü Uygulama kapanıyor... ");
                        Application.Current.Dispatcher.Invoke(() => Application.Current.Shutdown());
                        return;
                    }
                
                }
            });
        }

        public void createGroup(string groupName,string users)
        {
            var packet = new PacketBuilder();
            packet.WriteOpCode(2);
            packet.WriteMessage(groupName);
            packet.WriteMessage(users);
            _client.Client.Send(packet.GetPacketBytes());
            packet._ms.Dispose();
        }
        public void SendMessageToGroup(string message, string contactUID)
        {
            var packet = new PacketBuilder();
            packet.WriteOpCode(20);
            packet.WriteMessage(message);
            packet.WriteMessage(contactUID);
            _client.Client.Send(packet.GetPacketBytes());
            packet._ms.Dispose();
        }
        public void SendMessageToServer(string message, string contactUID)
        {
            var packet = new PacketBuilder();
            packet.WriteOpCode(5);
            packet.WriteMessage(message);
            packet.WriteMessage(contactUID);
            _client.Client.Send(packet.GetPacketBytes());
            packet._ms.Dispose();
        }

    }
}
