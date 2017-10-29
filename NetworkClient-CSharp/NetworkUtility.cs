using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkClient
{
    class NetworkUtility
    {
        private String reqHeader = "0x1092";
        private int bitsInByte = 8;
        private int bitsInChar = 16;
        private int reqHeaderSize = 2;
        private int reqVersionSize = 1;
        private int reqLengthSize = 4;
        private int reqOperationSize = 1;
        private int reqChecksumSize = 1;
        private int twoBytes = 0x10;
        private int threeBytes = 0x18;

        public static int exceptErr = 100;
        // This field is needed as server response status returning zero 
        // eventhough max response size is greater than 1048576
        public static int maxRespErr = 101;
        public static byte connectErr = 102;

        public static byte encodeOper = 1;
        public static byte decodeOper = 2;
        public static byte requestStatus = 1;
        public static byte receiveStatus = 2;
        public static byte sucessfulStatus = 0;

        public NetworkUtility()
        {
        }

        public static string GetRequestInfo(byte operation, int length)
        {
            StringBuilder requestInfo = new StringBuilder();
            requestInfo.Append("Requesting to ");
            if (operation == encodeOper)
            {
                requestInfo.Append("Encode ");
            }
            else
            {
                requestInfo.Append("Decode ");
            }
            requestInfo.Append(length + " byte(s) of data");

            return requestInfo.ToString();
        }
        public static string GetStatusInfo(byte type, byte status, string msg) {
            StringBuilder statusInfo = new StringBuilder();
            statusInfo.Append(DateTime.Now.ToString("HH:mm:ss.ffff"));

            string statusInfoStr = "    Information    ";
            string statusErrorStr = "    Error               ";
            switch (status) {
                case 0:
                    statusInfo.Append(statusInfoStr);
                    if (type == NetworkUtility.requestStatus) {
                        statusInfo.Append(msg);
                    }
                    else
                    {
                        statusInfo.Append(StatusStrings.sucessStr);
                    }
                    break;
                case 1:
                    statusInfo.Append(statusErrorStr);
                    statusInfo.Append(StatusStrings.errorStr1);
                    break;
                case 2:
                    statusInfo.Append(statusErrorStr);
                    statusInfo.Append(StatusStrings.errorStr2);
                    break;
                case 3:
                    statusInfo.Append(statusErrorStr);
                    statusInfo.Append(StatusStrings.errorStr3);
                    break;
                case 4:
                    statusInfo.Append(statusErrorStr);
                    statusInfo.Append(StatusStrings.errorStr4);
                    break;
                case 5:
                    statusInfo.Append(statusErrorStr);
                    statusInfo.Append(StatusStrings.errorStr5);
                    break;
                case 6:
                    statusInfo.Append(statusErrorStr);
                    statusInfo.Append(StatusStrings.errorStr6);
                    break;
                case 7:
                    statusInfo.Append(statusErrorStr);
                    statusInfo.Append(StatusStrings.errorStr7);
                    break;
                case 8:
                    statusInfo.Append(statusErrorStr);
                    statusInfo.Append(StatusStrings.errorStr8);
                    break;
                case 9:
                    statusInfo.Append(statusErrorStr);
                    statusInfo.Append(StatusStrings.errorStr9);
                    break;
                case 100:
                    statusInfo.Append(statusErrorStr);
                    statusInfo.Append(StatusStrings.errorStr100);
                    break;
                case 101:
                    statusInfo.Append(statusErrorStr);
                    statusInfo.Append(StatusStrings.errorStr101);
                    break;
                case 102:
                    statusInfo.Append(statusErrorStr);
                    statusInfo.Append(StatusStrings.errorStr102);
                    break;
                default:
                    statusInfo.Append(statusErrorStr);
                    statusInfo.Append(StatusStrings.errorStrDefault + status);
                    break;
            }
            return statusInfo.ToString();
        } 

       public byte[] CalculateRequest(byte[] payloadData, byte operation)
        {
           int offset = 0;
            UInt32 payloadLength = (UInt32)payloadData.Length;
            //calculating request length(header+version+length+operation+payload+checksum)
            int sendDataSize = reqHeaderSize + reqVersionSize + reqLengthSize + reqOperationSize + (int)payloadLength + reqChecksumSize;
            byte[] sendData = new byte[sendDataSize];

            //Header 2 bytes
            char header = System.Convert.ToChar(System.Convert.ToUInt32(reqHeader, bitsInChar));
            sendData[offset] = (byte)header;// first 8 bits
            sendData[++offset] = (byte)(header >> bitsInByte); //bits from 9 to 16
            //Version 1 byte
            sendData[++offset] = 1;
            //length 4 bytes
            UInt32 requestLength = (UInt32)sendDataSize;
            sendData[++offset] = (byte)requestLength; // first 8 bits
            sendData[++offset] = (byte)(requestLength >> bitsInByte); // get bits from 9 to 16
            sendData[++offset] = (byte)(requestLength >> twoBytes);// get bits from 17 to 24
            sendData[++offset] = (byte)(requestLength >> threeBytes); //get bits from 25 to 32
            //Operation 1 byte
            sendData[++offset] = operation;
            //payload payloadLength bytes
            Array.Copy(payloadData, 0, sendData, ++offset, payloadLength);
            //checksum 1 byte
            sendData[sendDataSize - 1] = CheckSum(sendData);
            return sendData;
        }

        /*Calculating the checksum for the data as mentioned in the requirement.
         To fit the data in one byte, remainder with 256 (this is not in the instruction)*/
        public byte CheckSum(byte[] data)
        {
            int checksum = 0;
            //participates all bytes except last checksum byte.
            for (int i = 0; i < data.Length-1; i++)
            {
                //bit position to calculate(1 to 8).
                byte bitPos = (byte)((i % bitsInByte));
                //get bit value at bitPos from i'th byte of the data
                if ((data[i] & (1 << bitPos)) > 0)
                    checksum++;
            }
            //Remainedr to checksum by 256 to fit in 1 byte.
            return (byte)(checksum % (byte.MaxValue+1));
        }
        
        /*Generate some random data between 0 to 255 in bytes*/
        public static byte[] GenerateData(int size)
        {
            byte[] data = new byte[size];
            for (int i = 0; i < size; i++)
            {
                //generates data in between 0 to 255
                data[i] = (byte)(i % (byte.MaxValue + 1));
            }
            return data;
        }
    }
}
