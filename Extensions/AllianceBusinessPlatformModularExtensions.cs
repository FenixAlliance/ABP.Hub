using FenixAlliance.ABP.Hub.Plugins;
using FenixAlliance.ACL.Configuration.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FenixAlliance.ABP.Hub.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class AllianceBusinessPlatformModularExtensions
    {
        /// <summary>
        /// This method adds services to the container for each enabled module. This method gets called by the runtime trough the Alliance Business Platform.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="Configuration"></param>
        /// <param name="Environment"></param>
        /// <param name="Options"></param>
        public static void AddAllianceBusinessPlatformModular(this IServiceCollection services, IConfiguration Configuration, IHostEnvironment Environment, ISuiteOptions Options)
        {
            PluginManager.ConfigureModuleServices(services, Configuration, Environment, Options);
        }

        /// <summary>
        /// Use this method to configure the HTTP request pipeline. This method gets called by the runtime trough the Alliance Business Platform.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="Configuration"></param>
        /// <param name="Environment"></param>
        /// <param name="Options"></param>
        public static void UseAllianceBusinessPlatformModular(this IApplicationBuilder app, IConfiguration Configuration, IHostEnvironment Environment, ISuiteOptions Options)
        {
            PluginManager.UseModuleServices(app, Configuration, Environment, Options);
        }
    }
}