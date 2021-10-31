using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedPlugins.API.Structs
{
    public class NetworkServerConfig
    {
        public string LinkToken { get; set; }
        public string Token { get; set; }
        public string ServerName { get; set; }
        public List<string> InstalledAddons { get; set; }
    }
}
