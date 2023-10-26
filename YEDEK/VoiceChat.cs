using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace ChatApp.NET
{
    delegate void OutgoingVoiceAvailableHandler(byte[] data);

    class VoiceChat
    {
        public event OutgoingVoiceAvailableHandler OutgoingVoiceAvailable;

        WaveIn waveIn;
        WaveOut waveOut;
        BufferedWaveProvider waveProvider;

        public VoiceChat()
        {
            var waveFormat = new WaveFormat(8000, 16, 1);
            waveProvider = new BufferedWaveProvider(waveFormat);

            waveOut = new WaveOut();
            waveOut.Init(waveProvider);

            waveIn = new WaveIn();
            waveIn.WaveFormat = waveFormat;

            waveIn.DataAvailable += delegate (object sender, WaveInEventArgs e)
            {
                OnOutgoingVoiceAvailable(e.Buffer);
            };
        }

        public void Start()
        {
            waveOut.Play();
            waveIn.StartRecording();
        }

        public void Stop()
        {
            waveOut.Stop();
            waveIn.StopRecording();
        }

        public void Play(byte[] data)
        {
            waveProvider.AddSamples(data, 0, data.Length);
        }

        void OnOutgoingVoiceAvailable(byte[] data)
        {
            if (OutgoingVoiceAvailable != null)
            {
                OutgoingVoiceAvailable(data);
            }
        }
    }
}
