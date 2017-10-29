using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkClient
{
    internal sealed class DataSender
    {
        private NetworkUtility utility = null;
        private NetworkStream stream = null;
        private Thread thread = null;
        internal event EventHandler<StatusEventArgs> DataRequested;
        private ManualResetEvent sendEvent = new ManualResetEvent(false);
        private bool isShutDown = false;
        private byte[] data = null;
        private byte operation = NetworkUtility.encodeOper;

        public DataSender()
        {
            utility = new NetworkUtility();
            thread = new Thread(Run);
            thread.Start();
        }

        internal void StartSendData(NetworkStream aStream, byte[] aData, byte aOperation)
        {
            stream = aStream;
            operation = aOperation;
            data = aData;
            sendEvent.Set();
        }

        internal void CloseThread()
        {
            isShutDown = true;
            sendEvent.Set();
        }

        private void Run()
        {
            while (sendEvent.WaitOne() && !isShutDown)
            {
                StatusEventArgs statusEvent = new StatusEventArgs();
                try
                {
                    byte[] outStream = utility.CalculateRequest(data, operation);
                    stream.Write(outStream, 0, outStream.Length);
                    stream.Flush();
                    statusEvent.Status = NetworkUtility.sucessfulStatus;//successful send
                }
                catch (Exception ex)
                {
                    statusEvent.Status = (byte)NetworkUtility.exceptErr;
                    stream.Close();
                }
                statusEvent.Type = NetworkUtility.requestStatus;
                DataRequested(this, statusEvent);
                sendEvent.Reset();
            }
        }
    }
}
