using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemoteDirectoryList.Models
{
    public class FileSize
    {
        public double Byte { get; set; }

        public double KB { get { return Byte / 1024; } set { Byte = value * 1024; } }

        public double MB { get { return KB / 1024; } set { Byte = value * 1024 * 1024; } }

        public double GB { get { return MB / 1024; } set { Byte = value * 1024 * 1024 * 1024; } }
    }
}
