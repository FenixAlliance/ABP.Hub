using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Newtonsoft.Json;

namespace FenixAlliance.ABP.Hub.Plugins
{
    public static class PluginManager
    {
        public static Assembly LoadPlugin(string relativePath)
        {
            // Navigate up to the solution root
            string rootPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            string pluginLocation = Path.GetFullPath(Path.Combine(rootPath, relativePath.Replace('\\', Path.DirectorySeparatorChar)));

            Console.WriteLine($"Loading commands from: {pluginLocation}");

            var loadContext = new PluginLoadContext(pluginLocation);

            return loadContext.LoadFromAssemblyPath(pluginLocation);
        }

        public static List<Assembly> GetModulesAssemblies(ApplicationPartManager applicationPartManager, List<Assembly> additionalAssemblies = null)
        {
            var ModulesManifest = PluginManager.GetModulesManifest();

            var ApplicationParts = applicationPartManager.ApplicationParts.ToList();

            var ApplicationPartAssemblies = ApplicationParts.Select(c => c.GetType().Assembly).ToList();

            var AdditionalAssemblies = new List<Assembly>();

            foreach (var module in ModulesManifest)
            {
                foreach (var assembly in module.AssemblyPaths)
                {
                    try
                    {
                        var LoadedAssembly = Assembly.LoadFile(assembly);
                        AdditionalAssemblies.Add(LoadedAssembly);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"There was an error while loading {assembly} for Blazor Router.");
                    }
                }
            }

            if(AdditionalAssemblies is not null || AdditionalAssemblies.Count is not 0)
            {
                AdditionalAssemblies.AddRange(additionalAssemblies);
            }


            //foreach (var assembly in ApplicationPartAssemblies)
            //{
            //    try
            //    {
            //        AdditionalAssemblies.Add(Assembly.LoadFile(assembly.Location));
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine($"There was an error while loading {assembly.FullName} for Blazor Router.");
            //    }
            //}

            return AdditionalAssemblies;
        }

        public static List<ACL.Configuration.Types.ABP.Modular.Module> GetModulesManifest()
        {
            string rootPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var modulesManifestPath = Path.Join(rootPath, "Modules", "manifest.json");

            EnsureModulesManifestExists(modulesManifestPath);

            string Content = File.ReadAllText(modulesManifestPath);
            return JsonConvert.DeserializeObject<List<ACL.Configuration.Types.ABP.Modular.Module>>(Content);

        }

        private static void EnsureModulesManifestExists(string modulesManifestPath)
        {
            string rootPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var ModulesFolder = Path.Join(rootPath, "Modules");

            if (!Directory.Exists(ModulesFolder))
            {
                Directory.CreateDirectory(ModulesFolder);
            }

            if (!File.Exists(modulesManifestPath))
            {
                File.WriteAllText(modulesManifestPath, JsonConvert.SerializeObject(new List<ACL.Configuration.Types.ABP.Modular.Module>()));
            }
        }

        public static List<ACL.Configuration.Types.ABP.Modular.Module> AddModuleToManifest(ACL.Configuration.Types.ABP.Modular.Module Extension)
        {
            string rootPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var modulesManifestPath = Path.Join(rootPath, "Modules", "manifest.json");

            EnsureModulesManifestExists(modulesManifestPath);

            var currentManifest = GetModulesManifest();

            if (!currentManifest.Exists(c => c?.ID == Extension?.ID && c?.Version == Extension?.Version))
            {
                currentManifest.Add(Extension);
            }

            //Write new file or append on existing file
            File.WriteAllText(modulesManifestPath, JsonConvert.SerializeObject(currentManifest));

            return currentManifest;
        }

        public static ACL.Configuration.Types.ABP.Modular.Module GetModuleSettings(string moduleName)
        {
            string rootPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var modulesPath = Path.Combine(rootPath, "Modules");
            if (Directory.Exists(Path.Join(modulesPath, moduleName)))
            {
                if (File.Exists(Path.Join(modulesPath, moduleName, moduleName + ".json")))
                {
                    using (StreamReader r = new StreamReader(Path.Join(modulesPath, moduleName, moduleName + ".json")))
                    {
                        return JsonConvert.DeserializeObject<ACL.Configuration.Types.ABP.Modular.Module>(r.ReadToEnd());
                    }
                }
            }
            return null;
        }

        public static Package GetModuleSpec(string moduleName)
        {
            string rootPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var modulesPath = Path.Combine(rootPath, "Modules");
            if (Directory.Exists(Path.Join(modulesPath, moduleName)))
            {
                if (File.Exists(Path.Join(modulesPath, moduleName, moduleName + ".nuspec")))
                {
                    using (StreamReader r = new StreamReader(Path.Join(modulesPath, moduleName, moduleName + ".nuspec")))
                    {
                        return JsonConvert.DeserializeObject<Package>(r.ReadToEnd());
                    }
                }
            }
            return null;
        }
    }
}
