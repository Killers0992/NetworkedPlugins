namespace NetworkedPlugins.Dedicated
{
    using NetworkedPlugins.API;
    using System;
    using System.Reflection;

    /// <inheritdoc/>
    public class ConsoleLogger : NPLogger
    {
        /// <inheritdoc/>
        public override void Debug(string message, bool isDebug = false)
        {
            if (!isDebug)
                return;

            Assembly callingAssembly = Assembly.GetCallingAssembly();
            if (callingAssembly == typeof(Host).Assembly || callingAssembly == typeof(NPManager).Assembly)
                Console.WriteLine($" [{DateTime.Now.ToString("T")}] [DEBUG] " + message);
            else
                Console.WriteLine($" [{DateTime.Now.ToString("T")}] [DEBUG] [{Assembly.GetCallingAssembly().GetName().Name}] " + message);
        }

        /// <inheritdoc/>
        public override void Error(string message)
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();
            if (callingAssembly == typeof(Host).Assembly || callingAssembly == typeof(NPManager).Assembly)
                Console.WriteLine($" [{DateTime.Now.ToString("T")}] [ERROR] " + message);
            else
                Console.WriteLine($" [{DateTime.Now.ToString("T")}] [ERROR] [{Assembly.GetCallingAssembly().GetName().Name}] " + message);
        }

        /// <inheritdoc/>
        public override void Info(string message)
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();
            if (callingAssembly == typeof(Host).Assembly || callingAssembly == typeof(NPManager).Assembly)
                Console.WriteLine($" [{DateTime.Now.ToString("T")}] [INFO] " + message);
            else
                Console.WriteLine($" [{DateTime.Now.ToString("T")}] [INFO] [{Assembly.GetCallingAssembly().GetName().Name}] " + message);
        }
    }
}
