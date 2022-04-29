using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemoteDirectoryList.Models
{
    public class DirectoryViewModel
    {
        public string DirectoryPath { get; set; }

        public string ParentDirectoryPath { get; set; }

        public bool IsRootDirectory { get; set; }

        public string TabId { get; set; }

        public List<EntryViewModel> Files { get; set; }
    }
}
