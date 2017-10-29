using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkClient
{
    public class StatusEventArgs : EventArgs
    {
        public byte Status { get; set; }
        //type 1 -> Request status and 2 -> Response status
        public byte Type { get; set; }
        public byte[] Data { get; set; }

    }
}
