using System;
using System.Reflection;
using System.Runtime.Loader;
using FenixAlliance.ACL.Configuration.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FenixAlliance.ABP.Hub.Plugins
{
    public class PluginLoadContext : AssemblyLoadContext
    {
        private AssemblyDependencyResolver _resolver;
        private IServiceCollection services;
        private IConfiguration Configuration;
        private IHostEnvironment Environment;
        private ISuiteOptions Options;

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

        /// <summary>
        /// Loads the contents of an assembly file on the specified path.
        /// </summary>
        /// <param name="assemblyPath">The absolute path of the file to load.</param>
        public static Assembly LoadPlugin(string assemblyPath)
        {
            // Logging attempt
            Console.WriteLine($"Loading commands from: {assemblyPath}");
            // New Plugin Load Context
            var loadContext = new PluginLoadContext(assemblyPath);
            // Return the loaded assembly
            return loadContext.LoadFromAssemblyPath(assemblyPath);
        }
    }
}