using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client
{
    class Connection : IDisposable
    {
        public Connection(string IPAdress, Int32 port)
        {
            _ipAdress = IPAdress;
            _port = port;
        }

        private string _ipAdress;

        private int _port;

        private int _bufferSize = 1024;

        public int BufferSize
        {
            get { return _bufferSize; }
            set
            {
                if (value > 0)
                    _bufferSize = value;
            }
        }

        public string IPAdress
        {
            get { return _ipAdress; }
        }

        public int Port
        {
            get { return _port; }
        }

        public void Send(string path)
        {
            byte[] SendingBuffer = null;
            TcpClient client = null;
            NetworkStream netStream = null;
            try
            {
                client = new TcpClient(_ipAdress, _port);
                netStream = client.GetStream();

                SendFileInfo(path, netStream);
                
                FileStream Fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                int NoOfPackets = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Fs.Length) / Convert.ToDouble(_bufferSize)));
                int TotalLength = (int)Fs.Length, CurrentPacketLength, counter = 0;
                for (int i = 0; i < NoOfPackets; i++)
                {
                    if (TotalLength > _bufferSize)
                    {
                        CurrentPacketLength = _bufferSize;
                        TotalLength = TotalLength - CurrentPacketLength;
                    }
                    else
                    {
                        CurrentPacketLength = TotalLength;
                    }
                    SendingBuffer = new byte[CurrentPacketLength];
                    Fs.Read(SendingBuffer, 0, CurrentPacketLength);
                    netStream.Write(SendingBuffer, 0, SendingBuffer.Length);
                }

                Fs.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                netStream.Close();
                client.Close();
            }
        }

        //TODO: connection close

        public void Dispose()
        {
            Dispose();
        }

        public void SendFileInfo(string path, NetworkStream netStream)
        {
            byte[] hash;
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(path))
                {
                    hash = md5.ComputeHash(stream);
                }
            }
            netStream.Write(hash, 0, hash.Length);
            string fileName = Path.GetFileName(path);
            byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileName);
            netStream.Write(fileNameBytes, 0, fileNameBytes.Length);
        }

    }
}
