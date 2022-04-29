using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemoteDirectoryList.Models
{
    public class AppSettings
    {
        public RootDirectoryTabModel[] RootDirectoryTabs { get; set; }

        public string DefaultTabId { get; set; }
    }
}
