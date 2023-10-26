using ChatApp.NET.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.NET
{
    delegate void IncomeVoiceAvailableHandler(byte[] data);

    class VoiceNetwork
    {
        public event IncomeVoiceAvailableHandler IncomeVoiceAvailable;

        UdpReceiver dataReceiver;
        UdpSender dataSender;

        public VoiceNetwork(string address)
        {
            dataSender = new UdpSender("127.0.0.1", 37336);
            dataReceiver = new UdpReceiver("127.0.0.1", 37336);

            dataReceiver.DataArrived += delegate (byte[] data, string senderIP)
            {
                OnIncomeVoiceAvailable(data);
            };
        }



        public void Start()
        {
            dataReceiver.Start();
        }

        public void Send(byte[] data)
        {
            dataSender.Send(data);
        }

        public void Stop()
        {
            dataReceiver.Close();
        }

        void OnIncomeVoiceAvailable(byte[] data)
        {
            if (IncomeVoiceAvailable != null)
            {
                IncomeVoiceAvailable(data);
            }
        }
    }
}
