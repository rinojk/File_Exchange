using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml.Serialization;
using SerializeLib;

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

        private int _bufferSize = 1048576;

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

        public void Send(string path, ProgressBar progBar)
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
                int TotalLength = (int)Fs.Length, CurrentPacketLength;
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
                    double value = ((double)i/(double)NoOfPackets)*100;
                    progBar.Dispatcher.Invoke(() => { progBar.Value = value; });
                    //Thread.Sleep(50);
                }

                Fs.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
            SendInfo fileInfo = new SendInfo();
            
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(path))
                {
                    fileInfo.Hash = md5.ComputeHash(stream);
                }
            }
            
            fileInfo.FileName = Path.GetFileNameWithoutExtension(path)+Path.GetExtension(path);
            fileInfo.FileSize = (new FileInfo(path)).Length;

            byte[] bytes = ObjectToByteArray(fileInfo);
            netStream.Write(bytes,0,bytes.Length);

            //MessageBox.Show("sent");
        }


        private static byte[] ObjectToByteArray(Object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.AssemblyFormat = FormatterAssemblyStyle.Simple;
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
        private static Object ByteArrayToObject(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return obj;
            }
        }
    }
}
