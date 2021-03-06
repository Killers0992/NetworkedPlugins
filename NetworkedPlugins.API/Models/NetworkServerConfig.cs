using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedPlugins.API.Models
{
    public class NetworkServerConfig
    {
        public string LinkToken { get; set; }
        public string Token { get; set; }
        public string ServerName { get; set; } = "Default Name";
        public List<string> InstalledAddons { get; set; } = new List<string>();
    }
}
