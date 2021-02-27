using FenixAlliance.ACL.Configuration.Types.ABP.Modular;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace FenixAlliance.ABP.Hub.Plugins
{
    public class Plugin : Module
    {
        public override int ConfigureServices(IServiceCollection services, IConfiguration Configuration, IHostEnvironment Environment)
        {
            throw new NotImplementedException();
        }

        public override int Configure(IApplicationBuilder app, IConfiguration Configuration, IHostEnvironment Environment)
        {
            throw new NotImplementedException();
        }
    }
}
