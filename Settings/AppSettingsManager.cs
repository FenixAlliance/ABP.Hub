using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FenixAlliance.ABP.Hub.Plugins;
using Newtonsoft.Json;

namespace FenixAlliance.ABS.Portal.Core.AppSettingHelpers
{
    public static class AppSettingsManager<T>
    {
        public static T GetAppSettings()
        {
            string rootPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var pluginsPath = Path.Combine(rootPath, "appsettings.json");

            using (StreamReader r = new StreamReader(pluginsPath))
            {
                return JsonConvert.DeserializeObject<T>(r.ReadToEnd());
            }
        }

        public static T GetSettings(string settingsName = "appsettings")
        {
            string rootPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var settingsPath = Path.Combine(rootPath, settingsName + ".json");

            if (File.Exists(settingsPath))
            {
                using (StreamReader r = new StreamReader(settingsPath))
                {
                    return JsonConvert.DeserializeObject<T>(r.ReadToEnd());
                }
            }
            return default(T);
        }


        public static T UpdateAppSettings(T AppSettings)
        {
            var NewSettings = JsonConvert.SerializeObject(AppSettings);

            string rootPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var pluginsPath = Path.Combine(rootPath, "appsettings.json");

            //Write new file or append on existing file
            using (StreamWriter writer = new StreamWriter(pluginsPath, false))
            {
                writer.Write(NewSettings);
                writer.Close();
            }

            return JsonConvert.DeserializeObject<T>(NewSettings);
        }

    }
}
