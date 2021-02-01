using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Serialization;
using FenixAlliance.ABP.Hub.Plugins.Specifications;
using FenixAlliance.ACL.Configuration.Interfaces;
using FenixAlliance.ACL.Configuration.Interfaces.ABP.Modular;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;

namespace FenixAlliance.ABP.Hub.Plugins
{
    public static class PluginManager
    {

        /// <summary>
        /// Modules Folder Path
        /// </summary>
        static string ModulesFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location), "Modules");

        /// <summary>
        /// 
        /// </summary>
        static List<FenixAlliance.ACL.Configuration.Types.ABP.Modular.Module> Plugins = new List<FenixAlliance.ACL.Configuration.Types.ABP.Modular.Module>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="Configuration"></param>
        /// <param name="Environment"></param>
        /// <param name="Options"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigureModuleServices(IServiceCollection services, IConfiguration Configuration, IHostEnvironment Environment, ISuiteOptions Options)
        {
            IEnumerable<IModule> Modules = GetModulesManifest();


            foreach (var Module in Modules)
            {
                var ModuleConfiguration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();

                Module.ConfigureServices(services, ModuleConfiguration, Environment);
            }

            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="Configuration"></param>
        /// <param name="Environment"></param>
        /// <param name="Options"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseModuleServices(IApplicationBuilder app, IConfiguration Configuration, IHostEnvironment Environment, ISuiteOptions Options)
        {
            /*
             * Flow:
             *  1. We need to get the modules manifest.
             */

            IEnumerable<IModule> Modules = GetModulesManifest();

            foreach (var Module in Modules)
            {
                var ModuleConfiguration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();

                Module.Configure(app, ModuleConfiguration, Environment);
            }

            return app;
        }
        /// <summary>
        /// This method registers all application parts.
        /// </summary>
        /// <param name="apm"></param>
        public static async Task<ApplicationPartManager> AddApplicationParts(ApplicationPartManager apm)
        {
            // Get the List of Modules registered
            var ModulesManifest = GetModulesManifest();
            // Get the entry assembly
            var entryAssembly = Assembly.GetEntryAssembly();
            // Logging intent.
            Console.WriteLine($"Trying to load plugins for entry assembly {entryAssembly?.FullName} at path {ModulesFolder} ");
            // Extracting Nuget Packages available at the Modules Root Folder
            ExtractNugetPackages(Directory.GetFiles(ModulesFolder, "*.nupkg", SearchOption.AllDirectories).ToList());
            // Getting every folder inside the Modules Root Folder
            var pluginFolders = Directory.GetDirectories(ModulesFolder);
            // Iterate for each folder on the Modules Folder and attempt to register each as an application part.
            foreach (var pluginFolder in pluginFolders)
            {
                try
                {
                    // Logging intent.
                    Console.WriteLine($"Trying to load plugin at: {pluginFolder}.");
                    // Trying to register the current module as an application part.
                    apm = await AddApplicationPart(apm, pluginFolder);
                }
                catch (Exception ex)
                {
                    // Logging error
                    Console.WriteLine($"Failed to load plugin at: {pluginFolders}. Error: {ex}.");
                }
            }
            // TODO: add Feature Providers
            apm?.FeatureProviders.Add(new ViewComponentFeatureProvider());

            // Return the modified application part.
            return apm;
        }

        /// <summary>
        /// This method registers an application part.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="apm"></param>
        /// <returns></returns>
        public static async Task<ApplicationPartManager> AddApplicationPart(ApplicationPartManager apm, string path)
        {
            // Flow: 
            // 1. Determine Form
            //      1.1 Extract nuget packages
            // 2. Prepare Plugin for consumption 
            //      2.1 Deserialize plugin manifest
            //      2.2 Check for trust configuration
            //      2.3 Load entry assembly
            // 3. Register plugin 
            //      4.1 Add assemblies to plugin context
            //      4.1 Place Static Assets where they belong
            //      4.2 Register Application Parts
            //      4.3 Register Application Features

            // System.Reflection.Assembly
            // In the.NET framework Assembly is the minimum unit of deployment and it is the form of an EXE or a DLL.
            //    - An assembly can contain one or more files, they can include things like resource files or modules(embedded).
            //    - An assembly always contains an assembly manifest.Assembly manifest is the metadata of an assembly. 
            //          It contains assembly definition identity, files in the assembly, type reference information and more.
            //    - The Assembly Linker(Al.exe) generates a file that has the assembly manifest in it.
            //    - You can view the topology of an assembly by using the IL Disassembler(ildasm.exe).

            // System.Reflection.Module
            // The code files in an assembly are called modules.
            //   - A module is a unit of compilation.
            //   - Module contains type metadata.
            //   - A module can not be deployed alone, it has to be linked into an assembly(using Al.exe).


            if (Directory.Exists(path))
            {
                var assemblyFiles = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

                if (assemblyFiles is null || !assemblyFiles.Any())
                {
                    Console.WriteLine($"No files where found in folder {assemblyFiles}");
                }
                else
                {
                    var NuSpecFile = assemblyFiles?.Where(c => c.ToLowerInvariant().EndsWith(".nuspec"))?.First();
                    var DllFiles = assemblyFiles?.Where(c => c.ToLowerInvariant().EndsWith(".dll"))?.ToList();

                    var CssFiles = assemblyFiles?.Where(c => c.ToLowerInvariant().EndsWith(".css"))?.ToList();
                    var JsFiles = assemblyFiles?.Where(c => c.ToLowerInvariant().EndsWith(".js"))?.ToList();
                    var HtmlFiles = assemblyFiles?.Where(c => c.ToLowerInvariant().EndsWith(".html"))?.ToList();
                    var JsonFiles = assemblyFiles?.Where(c => c.ToLowerInvariant().EndsWith(".json"))?.ToList();

                    var PluginSettings = new Plugin()
                    {
                        AssemblyPaths = new List<string>()
                    };

                    var pluginSpecSerializer = new XmlSerializer(typeof(Package));
                    var pluginSpecFileStream = new FileStream(NuSpecFile, FileMode.Open);
                    var pluginSpec = (Package)pluginSpecSerializer.Deserialize(pluginSpecFileStream);

                    PluginSettings.NuSpecPath = NuSpecFile;
                    PluginSettings.ID = pluginSpec.Metadata?.Id;
                    PluginSettings.Name = pluginSpec.Metadata?.Title;
                    PluginSettings.Version = pluginSpec.Metadata?.Version;
                    PluginSettings.Manifest = JsonConvert.SerializeObject(pluginSpec);

                    int DllErrors = 0;

                    try
                    {
                        // Try to load Plugin DLLs
                        foreach (var assemblyFile in DllFiles)
                        {
                            try
                            {
                                Console.WriteLine($"Parsing plugin file: {assemblyFile} ");
                                Console.WriteLine($"Loading plugin assembly: {assemblyFile} ");

                                var assembly = Assembly.LoadFrom(assemblyFile);

                                var managedAssembly = PluginLoadContext.LoadPlugin(assemblyFile);

                                var assemblyTypes = assembly.GetTypes();
                                var assemblyLocation = assembly.Location;
                                var assemblyIsDynamic = assembly.IsDynamic;
                                var assemblyModules = assembly.GetModules(true);
                                var assemblyIsCollectible = assembly.IsCollectible;
                                var assemblyReflectionOnly = assembly.ReflectionOnly;
                                var assemblyExecutionOnly = !assembly.ReflectionOnly;
                                var assemblyIsFullyTrusted = assembly.IsFullyTrusted;
                                var assemblySecurityRuleSet = assembly.SecurityRuleSet;
                                var assemblyExportedTypes = assembly.GetExportedTypes();
                                var assemblyReferencedAssemblies = assembly.GetReferencedAssemblies();
                                var assemblyManifestResourceNames = assembly.GetManifestResourceNames();

                                PluginSettings.AssemblyPaths.ToList().Add(assemblyLocation);

                                Plugins.AddRange(PluginManager.InstantiatePlugin(managedAssembly));

                                if (PluginSettings.AssemblyPaths.All(c => c != assemblyLocation))
                                {
                                    PluginSettings.AssemblyPaths.ToList().Add(assemblyLocation);
                                }

                                if (!(apm is null))
                                {
                                    if (assemblyFile.EndsWith(".Views.dll"))
                                    {
                                        apm.ApplicationParts.Add(new CompiledRazorAssemblyPart(assembly));
                                    }
                                    else
                                    {
                                        apm.ApplicationParts.Add(new AssemblyPart(assembly));
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Error($"There was an error while loading module assembly: {assemblyFile}. Error: {ex}.");
                            }
                        }

                        PluginManager.AddModuleToManifest(PluginSettings);

                    }
                    catch
                    {
                        Console.WriteLine($"Failed to Extract {DllErrors}/{DllFiles?.Count() ?? 0} nuget package at {path}");
                    }
                }
            }
            else
            {
                Console.WriteLine($"No folder could be found at path: {path}");
            }

            return apm;
        }

        /// <summary>
        /// This method search for each .nupkg present on the Modules Root folder, it extracts it's files to a folder named after the plugin manifest's id value.
        /// </summary>
        /// <param name="NugetPackagePaths"></param>
        /// <returns></returns>
        public static void ExtractNugetPackages(List<string> NugetPackagePaths)
        {
            // Initialize errors to 0.
            var NugetPackageErrors = 0;

            // Logging Intent
            Console.WriteLine($"Trying to extract nuget plugin files at path: {ModulesFolder} ");
            // Foreach Nuget Package
            foreach (var nugetPackage in NugetPackagePaths)
            {
                try
                {
                    // Logging intent
                    Console.WriteLine($"Trying to extract nuget package file: {nugetPackage} ");
                    // Determine extraction folder path
                    var newPath = Path.Join(Path.GetDirectoryName(nugetPackage), Path.GetFileNameWithoutExtension(nugetPackage));
                    // Unzip .nupkg file.
                    ZipFile.ExtractToDirectory(nugetPackage, newPath, true);
                }
                catch (Exception ex)
                {
                    // +1 to errors
                    NugetPackageErrors++;
                    // Logging error
                    Console.WriteLine($"Failed to Extract nuget package at {nugetPackage}. Ex: {ex}");
                }
            }

            // If more than 0 Extraction Errors, log all errors.
            if (NugetPackageErrors != 0)
            {
                // Logging Extraction Errors
                Console.WriteLine($"Failed to Extract {NugetPackageErrors} of {NugetPackagePaths?.Count() ?? 0}.");
            }
        }




        /// <summary>
        /// This method returns a List of Assemblies
        /// </summary>
        /// <param name="applicationPartManager"></param>
        /// <param name="additionalAssemblies"></param>
        /// <returns></returns>
        public static IEnumerable<Assembly> GetModulesAssemblies(ApplicationPartManager applicationPartManager, List<Assembly> additionalAssemblies = null)
        {
            // Get the modules manifest 
            var ModulesManifest = PluginManager.GetModulesManifest();
            // Get the list of assemblies from every registered application part.
            var ApplicationPartAssemblies = applicationPartManager.ApplicationParts.ToList().Select(c => c.GetType().Assembly).ToList();
            // Create a list of Assemblies to populate & return
            var ModuleAssemblies = new List<Assembly>();
            // Add incoming additional assemblies
            ModuleAssemblies.AddRange(additionalAssemblies);
            // Add application part's assemblies
            ModuleAssemblies.AddRange(ApplicationPartAssemblies);
            // Iterate for reach module in the modules manifest & add their assemblies as well.
            foreach (var module in ModulesManifest)
            {
                // Iterate through each assembly for this module and try to load.
                foreach (var assemblyPath in module.AssemblyPaths) { 
                    // Try to load the assembly
                    try
                    {
                        // Check if the file exists
                        if (File.Exists(assemblyPath))
                        {
                            // Add it to our response.
                            ModuleAssemblies.Add(Assembly.LoadFile(assemblyPath));
                        }
                    }
                    catch (Exception ex)
                    {
                        // Logging error
                        Console.WriteLine($"There was an error while loading {assemblyPath}.");
                    }
                }
            }
            // Returning assemblies
            return ModuleAssemblies;
        }

        /// <summary>
        /// Returns a list of invokable modules
        /// </summary>
        /// <returns>A List of IModule descendants deserialized from the Modules Manifest File. It will return Null if Assembly.GetEntryAssembly() returns null (When called from unmanaged code).</returns>
        public static List<IModule> GetModulesManifest()
        {
            // Get the Application's Root Path
            var EntryAssembly = Assembly.GetEntryAssembly();

            if (EntryAssembly != null)
            {
                // Get the Application's Root Path
                string rootPath = Path.GetDirectoryName(EntryAssembly.Location);
                // Get the path for the modules manifest
                var modulesManifestPath = Path.Join(rootPath, "Modules", "manifest.json");
                // Make sure that the path and file for the modules manifest does in fact exist
                EnsureModulesManifestExists(modulesManifestPath);
                // Read the entire content for the modules manifest
                string Content = File.ReadAllText(modulesManifestPath);
                // Return the deserialized content of the modules manifest as a List of IModule
                return JsonConvert.DeserializeObject<List<IModule>>(Content);
            }
            // Return null
            return null;
        }

        /// <summary>
        /// This method ensures that the Modules Manifest exists; it it does not, it'll create it.
        /// </summary>
        /// <param name="modulesManifestPath"></param>
        private static void EnsureModulesManifestExists(string modulesManifestPath)
        {
            // Get the executing folder for this application
            var EntryAssembly = Assembly.GetEntryAssembly();
            // If Entry Assembly is not null
            if (EntryAssembly != null)
            {
                // Get the applications root path
                string rootPath = Path.GetDirectoryName(EntryAssembly.Location);
                // If the modules folder does not exists, we need to create it
                if (!Directory.Exists(ModulesFolder))
                {
                    // Creating Modules Folder
                    Directory.CreateDirectory(ModulesFolder);
                }
                // If Modules Manifest does not exists, we need to create it
                if (!File.Exists(modulesManifestPath))
                {
                    // Creating blank Modules Manifest
                    File.WriteAllText(modulesManifestPath, JsonConvert.SerializeObject(new List<IModule>()));
                }
            }
        }
        /// <summary>
        /// This methods adds a module to the modules manifest.
        /// </summary>
        /// <param name="Extension"></param>
        /// <returns></returns>
        public static List<IModule> AddModuleToManifest(IModule Extension)
        {
            // Get the application's root path.
            var rootPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            // Determine the modules manifest path
            var modulesManifestPath = Path.Join(rootPath, "Modules", "manifest.json");
            // Let's make sure that both the Modules folder and the Modules manifest do in fact exist.
            EnsureModulesManifestExists(modulesManifestPath);
            // Get the current Modules Manifest a a list of invokable Modules
            var currentManifest = GetModulesManifest();
            // If the manifest does not contain an extension whose id is the same as the id of the module we're attempting to register
            if (!currentManifest.Exists(c => c?.ID == Extension?.ID ))
            {
                // If no module with the same id and the same version is present
                if(!currentManifest.Exists(c => c?.ID == Extension?.ID && c?.Version == Extension?.Version))
                {
                    // Let's add it to our Modules MAnifest
                    currentManifest.Add(Extension);
                }
                else
                {
                    // Logging status
                    Console.WriteLine("A plugin with this ID & version is already present.");
                }
            }
            else
            {
                // Logging status
                Console.WriteLine("A plugin with this ID is already present.");
            }
            // Update the Modules manifest
            File.WriteAllText(modulesManifestPath, JsonConvert.SerializeObject(currentManifest));
            // Return the updated Modules manifest
            return currentManifest;
        }
        /// <summary>
        /// This method returns a Module Configuration Object
        /// </summary>
        /// <param name="moduleName">The name of the module we want to load settings from.</param>
        /// <returns>The configuration object for this module.</returns>
        public static IModule GetModuleSettings(string moduleName)
        {
            // Get the executing folder for this application
            var EntryAssembly = Assembly.GetEntryAssembly();
            // If the Entry Assembly is not null
            if (EntryAssembly != null)
            {
                // Get the root path for the executing assembly
                var rootPath = Path.GetDirectoryName(EntryAssembly.Location);
                // Get the Modules Folder Path
                var modulesPath = Path.Combine(rootPath, "Modules");
                if (Directory.Exists(modulesPath))
                {
                    // If the Modules Folder Path exists
                    if (Directory.Exists(Path.Join(modulesPath, moduleName)))
                    {
                        // If the default appsettings.json file exists for this module.
                        if (File.Exists(Path.Join(modulesPath, moduleName, "appsettings" + ".json")))
                        {
                            // Read the Module Configuration file
                            using StreamReader r = new StreamReader(Path.Join(modulesPath, moduleName, "appsettings" + ".json"));
                            // Return the Deserialized Module Object
                            return JsonConvert.DeserializeObject<IModule>(r.ReadToEnd());
                        }
                    }
                }
            }
            // Return null
            return null;
        }
        /// <summary>
        /// This method returns a Deserialized Package Object with the respective nuget package metadata for the given module.
        /// </summary>
        /// <param name="moduleName"></param>
        /// <returns>An instance of the  </returns>
        public static Package GetModuleSpec(string moduleName)
        {

            var rootPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static IEnumerable<ACL.Configuration.Types.ABP.Modular.Module> InstantiatePlugin(Assembly assembly)
        {
            int count = 0;

            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(Module).IsAssignableFrom(type))
                {
                    
                    ACL.Configuration.Types.ABP.Modular.Module result = Activator.CreateInstance(type) as ACL.Configuration.Types.ABP.Modular.Module;

                    if (result != null)
                    {
                        count++;
                        //result.(ap);
                        yield return result;
                    }
                }
            }

            if (count == 0)
            {
                string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));

                Console.WriteLine($"Can't find any type which implements IModule in {assembly}. Check the list of available types: {availableTypes} .");
            }
        }
    }
}
