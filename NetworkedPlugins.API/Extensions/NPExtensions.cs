namespace NetworkedPlugins.API.Extensions
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    using LiteNetLib.Utils;
    using NetworkedPlugins.API.Events;

    /// <summary>
    /// Extensions.
    /// </summary>
    public static class NPExtensions
    {
        /// <summary>
        /// Get backing field.
        /// </summary>
        /// <param name="property">The target object.</param>
        /// <returns>Field.</returns>
        public static FieldInfo GetBackingField(this PropertyInfo property)
        {
            if (!property.CanRead || !property.GetGetMethod(nonPublic: true).IsDefined(typeof(CompilerGeneratedAttribute), inherit: true))
                return null;
            var backingField = property.DeclaringType.GetField($"<{property.Name}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
            if (backingField == null)
                return null;
            if (!backingField.IsDefined(typeof(CompilerGeneratedAttribute), inherit: true))
                return null;
            return backingField;
        }

        /// <summary>
        /// Copy all properties from the source class to the target one.
        /// </summary>
        /// <param name="target">The target object.</param>
        /// <param name="source">The source object to copy properties from.</param>
        public static void CopyProperties(this object target, object source)
        {
            Type type = target.GetType();

            if (type != source.GetType())
                throw new InvalidTypeException("Target and source type mismatch!");

            foreach (var sourceProperty in type.GetProperties())
                type.GetProperty(sourceProperty.Name)?.SetValue(target, sourceProperty.GetValue(source, null), null);
        }

        public static string GetAddonId(this Assembly ass)
        {
            if (NPManager.Singleton.AddonAssemblies.TryGetValue(ass, out string id))
                return id;
            return "";
        }

        public static void InvokeSafely<T>(this NPEventHandler.CustomEventHandler<T> ev, T arg)
                where T : EventArgs
        {
            if (ev == null)
                return;

            var eventName = ev.GetType().FullName;
            foreach (NPEventHandler.CustomEventHandler<T> handler in ev.GetInvocationList())
            {
                try
                {
                    handler(arg);
                }
                catch (Exception ex)
                {
                    LogException(ex, handler.Method.Name, handler.Method.ReflectedType.FullName, eventName);
                }
            }
        }

        public static void InvokeSafely(this NPEventHandler.CustomEventHandler ev)
        {
            if (ev == null)
                return;

            string eventName = ev.GetType().FullName;
            foreach (NPEventHandler.CustomEventHandler handler in ev.GetInvocationList())
            {
                try
                {
                    handler();
                }
                catch (Exception ex)
                {
                    LogException(ex, handler.Method.Name, handler.Method.ReflectedType?.FullName, eventName);
                }
            }
        }

        private static void LogException(Exception ex, string methodName, string sourceClassName, string eventName)
        {
            NPManager.Singleton.Logger.Error($"Method \"{methodName}\" of the class \"{sourceClassName}\" caused an exception when handling the event \"{eventName}\"");
            NPManager.Singleton.Logger.Error($"{ex}");
        }
    }
}
