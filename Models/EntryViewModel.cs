using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemoteDirectoryList.Models
{
    public class EntryViewModel
    {
        public string Name { get; set; }

        public FileSize Size { get; set; }

        public DateTime LastModifyTime { get; set; }

        public bool IsDirectory { get; set; }
    }
}
