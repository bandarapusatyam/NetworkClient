using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkClient
{
    public class StatusStrings
    {
        public static string sucessStr = "Request completed successfully";
        public static string errorStr1 = "Server responded Invalid header was received";
        public static string errorStr2 = "Server responded Unsupported protocol version was received";
        public static string errorStr3 = "Server responded Unsupported protocol operation was received";
        public static string errorStr4 = "Server responded Timed out waiting for more data / Incomplete data length received";
        public static string errorStr5 = "Server responded Maximum request length has been exceeded";
        public static string errorStr6 = "Server responded Invalid checksum was received";
        public static string errorStr7 = "Server responded Encode operation failed";
        public static string errorStr8 = "Server responded Decode operation failed";
        public static string errorStr9 = "Server responded Maximum response length after operation exceeds maximum allowed response length";
        public static string errorStr100 = "I/O Exception occur in request/response";
        public static string errorStr101 = "Server responded but the response size exceeds the maximum allowed response length";
        public static string errorStr102 = "Unable to connect to the server";
        public static string errorStrDefault = "Server responded the error code ";
    }
}
