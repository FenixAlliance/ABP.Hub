using System.Collections.Generic;
using System.Reflection;
using FenixAlliance.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace FenixAlliance.ABS.Portal.Core.Plugins
{
    public interface IPlugin
    {
        bool Active { get; }
        string Name { get; }
        string Author { get; }
        string License { get; }
        string Version { get; }
        string Description { get; }
        string Repository { get; }

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        IEnumerable<Assembly> Assemblies { get; }

        int ConfigureServices(IConfiguration Configuration, IHostEnvironment Environment, PlatformOptions Options);

        int Configure(IConfiguration Configuration, IHostEnvironment Environment, PlatformOptions Options);
    }
}
