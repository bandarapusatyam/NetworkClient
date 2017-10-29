using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

namespace NetworkClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        private Client client = null;
        private int count =0;

        private byte operation = NetworkUtility.encodeOper;
        public MainWindow()
        {
            InitializeComponent();
            client = new Client();
            btnGenerateTestData_Click(this, null);
            client.StatusUpdated += OnStatusUpdated;
        }

        private void btnGenerateTestData_Click(object sender, RoutedEventArgs e)
        {
            client.GenerateData(Int32.Parse(txtTestDataSize.Text));
            count = 0;
            lblEncodeCycleCount.Content = count + " (cycle count)";
        }

        private async void btnEncryptData_Click(object sender, RoutedEventArgs e)
        {
            operation = NetworkUtility.encodeOper;
            grid.IsEnabled = false;
            client.ConnectAndRequest(txtServerHostname.Text, Int32.Parse(txtServerPort.Text), NetworkUtility.encodeOper);
        }

        private void btnDecryptData_Click(object sender, RoutedEventArgs e)
        {
            operation = NetworkUtility.decodeOper;
            grid.IsEnabled = false;
            client.ConnectAndRequest(txtServerHostname.Text, Int32.Parse(txtServerPort.Text), NetworkUtility.decodeOper);
        }

        private void OnStatusUpdated(object sender, StatusEventArgs e)
        {
            if (e.Type == NetworkUtility.requestStatus) //Request status
            {
                if (e.Status == NetworkUtility.sucessfulStatus)
                {
                    lstStatus.Items.Insert(0, NetworkUtility.GetStatusInfo(e.Type, e.Status, 
                                NetworkUtility.GetRequestInfo(operation, client.GetDataLength())));
                    //request success and receive data now
                    client.ReceiveData(Int32.Parse(txtTimeout.Text));
                }
                else {
                    lstStatus.Items.Insert(0, NetworkUtility.GetStatusInfo(e.Type, e.Status,null));
                    grid.IsEnabled = true;
                }
            } else if (e.Type == NetworkUtility.receiveStatus) {//Receive status
                if (e.Status == 0){ 
                    if  (operation == NetworkUtility.encodeOper)
                    {
                        count++;
                    }
                    else if (operation == NetworkUtility.decodeOper)
                    {
                        count--;
                    }
                }
                //update UI about the response.
                lblEncodeCycleCount.Content = count + " (cycle count)";
                lstStatus.Items.Insert(0, NetworkUtility.GetStatusInfo(e.Type, e.Status, null));
                grid.IsEnabled = true;
            }
        }

        private void Window_Close(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // for closing threads.
            client.CloseClient();
        }
    }
}
