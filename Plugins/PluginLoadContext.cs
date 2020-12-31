using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using FenixAlliance.ABS.Portal.Core.Plugins;

namespace FenixAlliance.Core.Plugins
{
    public class PluginLoadContext : AssemblyLoadContext
    {
        private AssemblyDependencyResolver _resolver;

        public PluginLoadContext(string pluginPath)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }
        /// <summary>
        /// Load Managed Assembly from AssemblyName
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        protected override Assembly Load(AssemblyName assemblyName)
        {
            string assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }
        /// <summary>
        /// Load Managed Assembly from AssemblyPath
        /// </summary>
        /// <param name="assemblyPath"></param>
        /// <returns></returns>
        protected Assembly Load(string assemblyPath)
        {
            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);

            if (libraryPath != null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }

            return IntPtr.Zero;
        }

        protected  IntPtr LoadUnmanagedDllFromPath(string unmanagedDllPath)
        {

            if (unmanagedDllPath != null)
            {
                return LoadUnmanagedDllFromPath(unmanagedDllPath);
            }

            return IntPtr.Zero;
        }

        public static IEnumerable<IPlugin> InstantiatePlugin(Assembly assembly)
        {
            int count = 0;

            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(IPlugin).IsAssignableFrom(type))
                {
                    IPlugin result = Activator.CreateInstance(type) as IPlugin;
                    if (result != null)
                    {
                        count++;
                        yield return result;
                    }
                }
            }

            if (count == 0)
            {
                string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));

                Console.WriteLine($"Can't find any type which implements IPlugin in {assembly}.");
                //+ $"Available types: {availableTypes}");
            }
        }

    }
}