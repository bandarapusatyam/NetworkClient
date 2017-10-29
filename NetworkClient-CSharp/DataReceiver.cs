using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkClient
{
    internal sealed class DataReceiver
    {
        private NetworkStream stream = null;
        private Thread thread = null;
        public event EventHandler<StatusEventArgs> DataReceived;
        private ManualResetEvent receiveEvent = new ManualResetEvent(false);
        private bool isShutDown = false;
        private int maxRespLength = 1048576;
        private int respLengthSize = 4;
        private int respStatusPos = 2;
        private int respLengthPos = 3;
        private int respDataPos = 7;
        private int respSizeWithoutData = 8;
        private int dataLength = 0;
        private int timeMillis = 0;

        public DataReceiver()
        {
            thread = new Thread(Run);
            thread.Start();
        }

        internal void StartReceiveData(NetworkStream aStream, int aDataLength, int aTimeInMills)
        {
            stream = aStream;
            dataLength = aDataLength;
            timeMillis = aTimeInMills;
            receiveEvent.Set();
        }

        internal void CloseThread()
        {
            isShutDown = true;
            receiveEvent.Set();
        }

        private void Run()
        {
            while (receiveEvent.WaitOne() && !isShutDown)
            {
                byte respStatus = 0;
                StatusEventArgs statusEvent = new StatusEventArgs();
                try
                {
                    int respLength = 0;
                    int readSoFar = 0;
                    byte[] respData = null;
                    do
                    {
                        try
                        {
                            stream.ReadTimeout = timeMillis;
                            byte[] inStream = new byte[dataLength];
                            int read = stream.Read(inStream, 0, dataLength);
                            //reading data in first go
                            if (readSoFar == 0)
                            {
                                respStatus = inStream[respStatusPos];//3rd byte
                                                                     //Get response length of 4 bytes.
                                byte[] respLengthData = new byte[respLengthSize];
                                Array.Copy(inStream, respLengthPos, respLengthData, 0, respLengthSize);// 4th byte to 4 bytes length
                                respLength = BitConverter.ToInt32(respLengthData, 0);
                                //reading response.
                                readSoFar = read <= respLength ? read : respLength;
                                respData = new byte[respLength];
                                Array.Copy(inStream, 0, respData, 0, readSoFar);
                            }
                            else
                            {
                                // reading remaining data.
                                int nextRead = (read + readSoFar) < respLength ? read : (respLength - readSoFar);
                                Array.Copy(inStream, 0, respData, readSoFar, nextRead);
                                readSoFar = readSoFar + nextRead;
                            }
                            //exit if no data to read or responded with error code
                            if (read == 0 || respStatus != 0)
                                break;
                        }
                        catch (Exception ex)
                        {
                            respStatus = (byte)NetworkUtility.exceptErr;
                            break;
                        }
                    } while (readSoFar < respLength);

                    if (respStatus == 0 && respLength <= maxRespLength)
                    {
                        //store encrypted data
                        statusEvent.Data = null;
                        statusEvent.Data = new byte[respLength - respSizeWithoutData];
                        //read response data from 8th byte to last but one.
                        Array.Copy(respData, respDataPos, statusEvent.Data, 0, statusEvent.Data.Length);
                    }
                    else if (respStatus == 0 && respLength > maxRespLength)
                    {
                        // response status assigned as server response status returning zero 
                        // eventhough max response size is greater than 1048576
                        respStatus = (byte)(NetworkUtility.maxRespErr);
                    }
                }catch(Exception ex)
                {
                    respStatus = (byte)NetworkUtility.exceptErr;
                }
                finally
                {
                    stream.Close();
                }
                
                statusEvent.Type = NetworkUtility.receiveStatus;
                statusEvent.Status = respStatus;
                DataReceived(this, statusEvent);
                receiveEvent.Reset();
            }
        }
    }
}
