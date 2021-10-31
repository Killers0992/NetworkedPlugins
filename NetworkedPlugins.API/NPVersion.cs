﻿using NetworkedPlugins.API.Properties;
using System;

namespace NetworkedPlugins.API
{
    public class NPVersion
    {
        private static Version _version;
        public static Version Version 
        {
            get
            {
                if (_version == null)
                    return _version;

                if (Version.TryParse(Resources.version, out Version ver))
                    _version = ver;
                return _version;
            }
        }
    }
}
