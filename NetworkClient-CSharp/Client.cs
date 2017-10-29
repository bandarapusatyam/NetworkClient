using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NetworkClient
{
    public sealed partial class Client
    {
        private TcpClient client = null;
        private DataReceiver receiver = null;
        private DataSender sender = null;
        private byte[] data = null;

        public event EventHandler<StatusEventArgs> StatusUpdated;

        public Client()
        {
            sender = new DataSender();
            receiver = new DataReceiver();

            sender.DataRequested += OnStatusUpdated;
            receiver.DataReceived += OnStatusUpdated;
        }

        public void GenerateData(int dataSize)
        {
            data = NetworkUtility.GenerateData(dataSize);
        }

        public int GetDataLength() {
            return data.Length;
        }

        private bool Connect(string host, int port)
        {
            if (client != null)
                client.Close();
               
            try
            {
                client = new TcpClient();
                client.Connect(host, port);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        public void ConnectAndRequest(string host, int port, byte operation)
        {
            if (Connect(host, port))
            {
                sender.StartSendData(client.GetStream(), data, operation);
            }
            else
            {
                StatusEventArgs e = new StatusEventArgs();
                e.Type = 1;
                e.Status = NetworkUtility.connectErr;
                OnStatusUpdated(this, e);
            }
        }

        public void ReceiveData(int timeOut)
        {
            receiver.StartReceiveData(client.GetStream(), data.Length, timeOut);
        }

        private void OnStatusUpdated(object sender, StatusEventArgs e)
        {
            var handler = StatusUpdated;
            if (handler != null)
            {
                //updating the data with received data for next request.
                if (e.Type == NetworkUtility.receiveStatus && e.Status == NetworkUtility.sucessfulStatus)
                    data = e.Data;

                // update Window from UI thread
                Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
                {
                    StatusUpdated(this, e);  // re-raise event
                }));
            }
        }

        public void CloseClient()
        {
            receiver.CloseThread();
            sender.CloseThread();
        }
    }
}
