using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace FileSend_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void PathButton_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog()==true)
            {
                 pathTextBox.Text = openFileDialog.FileName;
            }
        }

        private void SendButton_OnClick(object sender, RoutedEventArgs e)
        {
            int port = Int32.Parse(PortTextBox.Text);
            SendTCP(IPTextBox.Text, port, pathTextBox.Text);
        }

        private void Send(string IP, int port, string path)
        {
            byte[] buffer;
            TcpClient client = new TcpClient(IP, port);
            NetworkStream nStream = client.GetStream();
            FileStream Fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            int NoOfPackets = Convert.ToInt32
         (Math.Ceiling(Convert.ToDouble(Fs.Length) / Convert.ToDouble(1024)));
            int TotalLength = (int)Fs.Length, CurrentPacketLength, counter = 0;
            for (int i = 0; i < NoOfPackets; i++)
            {
                if (TotalLength > 1024)
                {
                    CurrentPacketLength = 1024;
                    TotalLength = TotalLength - CurrentPacketLength;
                }
                else
                {
                    CurrentPacketLength = TotalLength;
                    buffer = new byte[CurrentPacketLength];
                    
                    Fs.Read(buffer, 0, CurrentPacketLength);
                    nStream.Write(buffer, 0, (int)buffer.Length);

                }
                Fs.Close();
            }
            nStream.Close();
        }

        private void SendTCP(string IPA, Int32 PortN, string M)
        {
            int BufferSize = 1024;
            byte[] SendingBuffer = null;
            TcpClient client = null;
            NetworkStream netstream = null;
            try
            {
                client = new TcpClient(IPA, PortN);
                netstream = client.GetStream();

                //byte[] md5 = GetChecksum(M);
                //string checksum = Encoding.ASCII.GetString(md5);
                //MessageBox.Show();
                //netstream.Write(md5, 0, md5.Length);

                FileStream Fs = new FileStream(M, FileMode.Open, FileAccess.Read);
                int NoOfPackets = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Fs.Length)/Convert.ToDouble(BufferSize)));
                int TotalLength = (int) Fs.Length, CurrentPacketLength, counter = 0;
                Stopwatch s = new Stopwatch();
                s.Start();
                for (int i = 0; i < NoOfPackets; i++)
                {
                    if (TotalLength > BufferSize)
                    {
                        CurrentPacketLength = BufferSize;
                        TotalLength = TotalLength - CurrentPacketLength;
                    }
                    else
                        CurrentPacketLength = TotalLength;
                    SendingBuffer = new byte[CurrentPacketLength];
                    Fs.Read(SendingBuffer, 0, CurrentPacketLength);
                    netstream.Write(SendingBuffer, 0, (int) SendingBuffer.Length);
                }
                s.Stop();

                //netstream.Flush();
                /*Thread.Sleep(500);
                Byte[] data = new byte[1024];
                Int32 bytes = netstream.Read(data, 0, data.Length);
                string responseData = Encoding.ASCII.GetString(data, 0, bytes);
                MessageBox.Show(responseData);*/
                //netstream.Close();
                
                MessageBox.Show(s.ElapsedMilliseconds.ToString());
                Fs.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                netstream.Close();
                client.Close();
            }
        }

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
    }
}
