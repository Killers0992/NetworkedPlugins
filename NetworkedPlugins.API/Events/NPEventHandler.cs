using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedPlugins.API.Events
{
    public class NPEventHandler
    {
        public delegate void CustomEventHandler<TEventArgs>(TEventArgs ev)
            where TEventArgs : System.EventArgs;

        public delegate void CustomEventHandler();
    }
}
