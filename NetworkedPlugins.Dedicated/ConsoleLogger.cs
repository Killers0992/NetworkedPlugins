namespace NetworkedPlugins.Dedicated
{
    using NetworkedPlugins.API;
    using System;
    using System.Reflection;

    /// <inheritdoc/>
    public class ConsoleLogger : NPLogger
    {
        /// <inheritdoc/>
        public override void Debug(string message)
        {
            Console.WriteLine($" [{DateTime.Now.ToString("T")}] [DEBUG] [{Assembly.GetCallingAssembly().GetName().Name}] " + message);
        }

        /// <inheritdoc/>
        public override void Error(string message)
        {
            Console.WriteLine($" [{DateTime.Now.ToString("T")}] [ERROR] [{Assembly.GetCallingAssembly().GetName().Name}] " + message);
        }

        /// <inheritdoc/>
        public override void Info(string message)
        {
            Console.WriteLine($" [{DateTime.Now.ToString("T")}] [INFO] [{Assembly.GetCallingAssembly().GetName().Name}] " + message);
        }
    }
}
