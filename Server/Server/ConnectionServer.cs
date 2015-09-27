using System;
using System.Collections.Generic;
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
using SerializeLib;

namespace Server
{
    class ConnectionServer
    {
        public ConnectionServer(int port)
        {
            _port = port;
        }

        private int _port;

        private TcpClient client;

        public int Port
        {
            get { return _port; }
        }

        public void StartListen(ProgressBar progressBar)
        {
            IPAddress ip = IPAddress.Any;
            TcpListener listener = new TcpListener(ip, _port);
            listener.Start();

            byte[] data = new byte[1048576];
            int recBytes;

            while (true)
            {
                if (listener.Pending())
                {
                    client = listener.AcceptTcpClient();
                    NetworkStream nStream = client.GetStream();

                    SendInfo info = GetInfo(nStream);
                    
                    Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

                    dlg.FileName = Path.GetFileNameWithoutExtension(info.FileName);
                    dlg.DefaultExt = Path.GetExtension(info.FileName);
                    var result = dlg.ShowDialog();
                    string filename = string.Empty;
                    if (result == true)
                    {
                        filename = dlg.FileName;
                    }


                    if (!string.IsNullOrEmpty(filename))
                    {
                        int totalrecbytes = 0;
                        FileStream Fs = new FileStream
         (filename, FileMode.OpenOrCreate, FileAccess.Write);
                        while (((recBytes = nStream.Read
             (data, 0, data.Length)) > 0))
                        {
                            Fs.Write(data, 0, recBytes);
                            totalrecbytes += recBytes;
                            double value = ((double)totalrecbytes / (double)info.FileSize) * 100;
                            progressBar.Dispatcher.Invoke(() => { progressBar.Value = value; });
                        }

                        Fs.Close();
                        MessageBox.Show("Received!");
                    }

                    byte[] hashReceived = info.Hash;
                    byte[] hash = GetChecksum(filename);

                    if (!hash.SequenceEqual(hashReceived))
                    {
                        MessageBox.Show("RecFile error or corrupted!");
                        File.Delete(filename);
                    }
                    else
                    {
                        //MessageBox.Show((new FileInfo(filename)).Length+" "+fileSize);
                    }
                    
                    
                    nStream.Close();
                    client.Close();
                }
            }
        }

        private SendInfo GetInfo(NetworkStream stream)
        {
            SendInfo info = new SendInfo();
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            byte[] resArray = new byte[bytesRead];
            for (int i = 0; i < bytesRead; i++)
                resArray[i] = buffer[i];
            info = (SendInfo) ByteArrayToObject(resArray);
            //MessageBox.Show(info.FileName + "---" + info.FileSize);
            return info;
        }

        private static Object ByteArrayToObject(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                binForm.AssemblyFormat=FormatterAssemblyStyle.Simple;
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return obj;
            }
        }
        /*private byte[] ReceiveHash(NetworkStream stream)
        {
            byte[] recBytes = new byte[1024];
            int bytesRead = stream.Read(recBytes, 0, recBytes.Length);
            byte[] result = new byte[bytesRead];
            Array.Copy(recBytes, result, bytesRead);
            return result;
        }



        private void GetName(NetworkStream stream, out string ext, out string name)
        {
            string result = string.Empty;
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            result = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            name = Path.GetFileNameWithoutExtension(result);
            ext = Path.GetExtension(result);
        }*/

        private byte[] GetChecksum(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    return md5.ComputeHash(stream);
                }
            }
        }

        /*private long GetFileSize(NetworkStream stream)
        {
            long result;
            byte[] recBuffer = new byte[8];
            int recBytes = stream.Read(recBuffer, 0, recBuffer.Length);
            result = BitConverter.ToInt64(recBuffer, 0);
            return result;
        }*/
    }
}
