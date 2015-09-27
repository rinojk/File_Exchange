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
using Server;

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

        private void StartButton_OnClick(object sender, RoutedEventArgs e)
        {
            int port = int.Parse(portTextBox.Text);
            ConnectionServer conn = new ConnectionServer(port);
            Task.Run(() => {conn.StartListen(progressBar);});
        }

        private void PathButton_OnClick(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
