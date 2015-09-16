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
using Client;
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

        private Connection connection;
        private string path;

        private void PathButton_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog()==true)
            {
                 pathTextBox.Text = openFileDialog.FileName;
                path = openFileDialog.FileName;
            }
        }

        private void SendButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(IPTextBox.Text) && !string.IsNullOrEmpty(PortTextBox.Text))
            {
                connection = new Connection(IPTextBox.Text, int.Parse(PortTextBox.Text));
                connection.Send(path);
            }
            else
            {
                MessageBox.Show("Fill IP adress and Port fields!");
            }
        }

        
    }
}
