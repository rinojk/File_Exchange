using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
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

namespace FilesSend_Server
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

        private TcpClient client;

        private void Receive(int port)
        {
            IPAddress ip = IPAddress.Any;
            MessageBox.Show(ip.ToString());
            TcpListener listener = new TcpListener(ip, port);
            listener.Start();

            byte[] data = new byte[1024];
            int recBytes;

            while (true)
            {
                if (listener.Pending())
                {
                    client = listener.AcceptTcpClient();
                    NetworkStream nStream = client.GetStream();



                    Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                    Nullable<bool> result = dlg.ShowDialog();
                    string filename = String.Empty;
                    if (result == true)
                    {
                        filename = dlg.FileName;
                    }


                    if (!string.IsNullOrEmpty(filename))
                    {
                        int totalrecbytes = 0;
                        FileStream Fs = new FileStream
         (filename, FileMode.OpenOrCreate, FileAccess.Write);
                        while ((recBytes = nStream.Read
             (data, 0, data.Length)) > 0)
                        {
                            Fs.Write(data, 0, recBytes);
                            totalrecbytes += recBytes;
                        }
                        Fs.Close();
                    }

                    nStream.Close();
                    client.Close();
                }
            }
        }

        private void StartButton_OnClick(object sender, RoutedEventArgs e)
        {
            int port = int.Parse(portTextBox.Text);
            Task.Run(() => {Receive(port);});
        }

        private void PathButton_OnClick(object sender, RoutedEventArgs e)
        {
            
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
