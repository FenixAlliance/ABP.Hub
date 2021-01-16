using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AspNetCoreRateLimit;
using AutoMapper;
using FenixAlliance.ABM.Hub.Extensions;
using FenixAlliance.ABP.API.GraphQl.Core.Extensions;
using FenixAlliance.ABP.API.REST.Core.Extensions;
using FenixAlliance.ABP.BotEngine.Core.Extensions;
using FenixAlliance.ABP.HealthChecks.Core.Extensions;
using FenixAlliance.ABP.Hub.Plugins;
using FenixAlliance.ABP.i18n.Resources;
using FenixAlliance.ABP.SignalR;
using FenixAlliance.ACL.Configuration.Interfaces;
using FenixAlliance.ACL.Configuration.Types;
using FenixAlliance.ACL.Configuration.Types.ABS.SPAs;
using FenixAlliance.APS.Core.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using reCAPTCHA.AspNetCore;
using Serilog;
using Directory = System.IO.Directory;
using Package = FenixAlliance.ABP.Hub.Plugins.Package;

namespace FenixAlliance.ABP.Hub.Extensions
{

    public static class AllianceBusinessPlatformServiceExtensions
    {
        /// <summary>
        ///
        /// </summary>
        static string CorsPolicy = "AllowAllOrigins";
        /// <summary>
        /// Modules Folder
        /// </summary>
        static string ModulesFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location), "Modules");

        /// <summary>
        /// Supported Languages
        /// </summary>
        static string DefaultCulture = "en-US";

        /// <summary>
        /// 
        /// </summary>
        static List<IPlugin> Plugins = new List<IPlugin>();

        /// <summary>
        /// 
        /// </summary>
        static List<CultureInfo> supportedCultures = new List<CultureInfo>
        {
            new CultureInfo(DefaultCulture),
            new CultureInfo("es-CO"),
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="Configuration"></param>
        /// <param name="Environment"></param>
        /// <param name="Options"></param>
        public static void AddAllianceBusinessPlatform(this IServiceCollection services, IConfiguration Configuration,
            IHostEnvironment Environment, ISuiteOptions Options)
        {
            try
            {
                services.AddSingleton<ISuiteOptions>(Options);

                #region Auth

                if (Options.APS?.Enable ?? false)
                {
                    // Adds Azure AD B2C Authentication
                    //services.AddAlliancePassportServices(Configuration, Environment, Options);

                    #region Auth

                    if (Options.APS?.Enable ?? false)
                    {

                        if (Options.APS?.AzureADB2C?.DefaultProvider ?? false)
                        {
                            if (!Options.APS?.AzureAd.DefaultProvider ?? false)
                            {
                                // Adds Azure AD B2C Authentication
                                services.AddAuthentication(o =>
                                {
                                    o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                                    o.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                                })
                                .AddAzureAdB2C(options => Configuration.Bind($"APS:{Options.APS?.Provider}", options))
                                //.AddAzureADB2CBearer(options => Configuration.Bind("AzureAdB2C", options))
                                //.AddCertificate();
                                .AddCookie();

                                services.AddAuthorization();
                            }
                        }

                        if (Options.APS?.AzureAd.DefaultProvider ?? false)
                        {
                            if (!Options.APS?.AzureADB2C?.DefaultProvider ?? false)
                            {
                                services.AddAuthentication(AzureADDefaults.AuthenticationScheme)
                                    .AddAzureAD(options => Configuration.Bind(
                                        $"APS:{Options.APS?.Provider}",
                                        options))
                                    .AddCookie();
                            }
                        }
                    }
                    #endregion

                    #region GDPR
                    if (Options.ABP?.Privacy.Gdpr?.Enable ?? false)
                    {

                        // Adds Cookies Consent for GDPR Compliance
                        services.Configure<CookiePolicyOptions>(options =>
                        {
                            // This lambda determines whether user consent for non-essential cookies is 
                            // needed for a given request.
                            options.CheckConsentNeeded = context => true;
                            options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;
                        });
                    }
                    #endregion
                }

                #endregion

                #region Data

                if (Options.ABM?.Enable ?? false)
                {
                    services.AddAllianceBusinessModelServices(Configuration, Environment, Options);
                }

                #endregion

                #region REST

                if (Options.ABP?.Apis?.RestApi?.Enable ?? false)
                {
                    services.AddRestApiService(Configuration, Environment, Options);
                }

                #endregion

                #region ACS

                if (Options.ABP?.Cognitive?.BotEngine?.Enable ?? false)
                {
                    services.AddBotEngine(Configuration, Environment, Options);
                }

                #endregion

                #region CORS

                if (Options.ABP?.Cors?.Enable ?? false)
                {
                    // Add CORS Policies
                    services.AddCors(options =>
                    {
                        options.AddPolicy(name: CorsPolicy,
                            builder =>
                            {
                                builder.WithOrigins("http://suite.fenixalliance.com.co", "http://fenixalliance.com.co");
                            });
                    });
                }

                #endregion

                #region HTTP

                if (Options.ABP?.Http?.Enable ?? false)
                {
                    if (Options.ABP?.Http?.Enable ?? false)
                    {
                        // Set Base Path to /wwwroot
                        services.AddSingleton<IFileProvider>(
                        new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));
                    }

                    if (Options.ABP?.Http?.Hsts.Enable ?? false)
                    {
                        // Configure HSTS
                        services.Configure<HstsOptions>(options =>
                        {
                            options.IncludeSubDomains = Options.ABP?.Http?.Hsts?.IncludeSubDomains ?? false;
                            options.MaxAge = TimeSpan.FromDays(Options.ABP?.Http?.Hsts?.MaxAge ?? 365);
                        });
                    }

                    if (Options.ABP?.Server?.KestrelServer?.Enable ?? true)
                    {
                        // kestrel AllowSynchronousIO
                        services.Configure<KestrelServerOptions>(options =>
                        {
                            options.AllowSynchronousIO = Options.ABP?.Server?.KestrelServer.AllowSynchronousIO ?? true;
                        });
                    }

                    if (Options.ABP?.Server?.IisServer.Enable ?? true)
                    {
                        // IIS AllowSynchronousIO
                        services.Configure<IISServerOptions>(options => { options.AllowSynchronousIO = Options.ABP?.Server?.IisServer.AllowSynchronousIO ?? true; });
                    }

                    if (Options.ABP?.Http?.HttpSession.Enable ?? false)
                    {
                        // Set a short timeout for easy testing.
                        services.AddSession(options =>
                        {
                            options.Cookie.HttpOnly = Options.ABP?.Http?.HttpSession.HttpOnlyCookie ?? true;
                            options.IdleTimeout = Environment.IsDevelopment() ? TimeSpan.FromSeconds(Options.ABP?.Http?.HttpSession.DevIdleTimeout ?? 10) : TimeSpan.FromSeconds(Options.ABP?.Http?.HttpSession?.IdleTimeout ?? 3600);
                        });
                    }

                    if (Options.ABP?.Http?.EnableContextAccessor ?? false)
                    {
                        // Adds HTTP Context Accessor
                        services.AddHttpContextAccessor();
                    }
                }

                #endregion

                #region GraphQL

                if (Options.ABP?.Apis?.GraphQlApi?.Enable ?? false)
                {
                    services.AddGraphQlApiService(Configuration, Environment, Options);
                }

                #endregion

                #region Caching

                if (Options.ABP?.Caching?.Enable ?? false)
                {
                    services.AddMemoryCache(options =>
                    {

                    });

                    // TODO: ADD Distributed Memory Cache Options
                    // Adds Distributed Memory Cache
                    services.AddDistributedMemoryCache(options =>
                    {

                    });

                    services.AddDistributedSqlServerCache(options =>
                    {
                        options.ConnectionString = Configuration.GetConnectionString("FenixAllianceCacheContextMSSQL");
                        options.SchemaName = "dbo";
                        options.TableName = "ABSCache";
                    });

                    if (Options.ABP?.Caching?.ResponseCaching?.Enable ?? false)
                    {
                        services.AddResponseCaching();
                    }
                }

                #endregion

                #region AppInsights

                if (Options.ABP?.Integrations?.Enable ?? false)
                {
                    if (Options.ABP?.Integrations?.Microsoft?.Azure?.AzureAppInsight?.Enable ?? false)
                    {
                        // Adds Azure App Insights Telemetry
                        services.AddApplicationInsightsTelemetry(options =>
                            {
                                options.InstrumentationKey = Options.ABP?.Integrations?.Microsoft?.Azure
                                    ?.AzureAppInsight?.Options.InstrumentationKey;
                            });
                    }
                }

                #endregion

                #region APIRateLimit
                if (Options.ABP?.Apis?.IpRateLimiting?.Enable ?? false)
                {
                    services.Configure<IpRateLimitOptions>(Configuration.GetSection("ABP:APIS:IpRateLimiting"));
                    services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
                    services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
                    services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
                }

                #endregion

                #region HealthChecks

                if (Options.ABP?.HealthChecks?.Enable ?? false)
                {
                    services.AddHealthChecksPlatform(Configuration, Environment, Options);
                }
                #endregion

                #region Localization

                if (Options.ABP?.Localization?.Enable ?? false)
                {
                    // Adds localization to the App
                    services.AddLocalization();

                    // Configure App Localization
                    services.Configure<RequestLocalizationOptions>(options =>
                    {
                        options.DefaultRequestCulture = new RequestCulture(culture: DefaultCulture, uiCulture: DefaultCulture);
                        options.SupportedCultures = supportedCultures;
                        options.SupportedUICultures = supportedCultures;
                        options.RequestCultureProviders = new List<IRequestCultureProvider>
                        {
                            new QueryStringRequestCultureProvider(),
                            new CookieRequestCultureProvider()
                        };
                    });
                }

                #endregion

                #region Recaptcha

                if (Options.ABP?.Integrations?.Google.GoogleRecaptcha.Enable ?? false)
                {
                    // Adds Google recaptcha initialization
                    services.Configure<RecaptchaSettings>(Configuration.GetSection("ABP:Integrations:GoogleRecaptcha"));
                    services.AddTransient<IRecaptchaService, RecaptchaService>();
                }

                #endregion

                #region SignalR
                if (Options.ABP?.WebSockets?.Enable ?? false)
                {
                    services.AddSignalR();
                }

                #endregion

                #region AutoMapper
                services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
                #endregion

                #region MVC

                if (Options.ABS?.ControllersWithViews?.Enable ?? false)
                {
                    // Adds MVC Service
                    services.AddControllersWithViews(
                        options =>
                        {
                            options.RespectBrowserAcceptHeader = true; // false by default
                            options.Filters.Add(new ProducesAttribute("application/xml"));
                            options.Filters.Add(new ProducesAttribute("application/json"));
                            options.InputFormatters.Add(new XmlSerializerInputFormatter(options));
                            options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
                        })
                        .AddRazorRuntimeCompilation()
                        .AddXmlSerializerFormatters()
                        .AddXmlDataContractSerializerFormatters()
                        .AddJsonOptions(c =>
                        {
                            c.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                            c.JsonSerializerOptions.PropertyNamingPolicy = null;
                        })
                        .AddNewtonsoftJson(options =>
                        {
                            options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                            options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                            options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
                            options.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;
                        })
                        .ConfigureApplicationPartManager(async apm =>
                        {
                            await ConfigureApplicationParts(apm);
                        })
                        .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                        .AddDataAnnotationsLocalization(options =>
                        {
                            options.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(SharedResources));
                        });
                }
                #endregion

                #region Razor

                if (Options.ABS?.RazorPages?.Enable ?? false)
                {
                    // Adds Razor Pages
                    services.AddRazorPages()
                        .AddRazorPagesOptions(options =>
                        {

                        })
                        .AddRazorRuntimeCompilation()
                        .AddXmlSerializerFormatters()
                        .ConfigureApplicationPartManager(async apm => await ConfigureApplicationParts(apm))
                        .AddNewtonsoftJson(options =>
                        {
                            options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                            options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                            options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
                            options.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;

                        })
                        .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                        .AddDataAnnotationsLocalization(options =>
                        {
                            options.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(SharedResources));
                        });
                }

                #endregion

                #region Blazor
                if (Options.ABS?.Blazor?.Enable ?? false)
                {
                    // Adds Server-Side Blazor
                    services.AddServerSideBlazor();

                    services.AddServerSideBlazor()
                        .AddCircuitOptions(options => { options.DetailedErrors = Environment.IsDevelopment(); });

                    services.AddServerSideBlazor()
                        .AddHubOptions(o =>
                        {
                            o.EnableDetailedErrors = Environment.IsDevelopment();
                            o.MaximumReceiveMessageSize = 102400000;
                        });
                }
                #endregion

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="Configuration"></param>
        /// <param name="Environment"></param>
        /// <param name="Options"></param>
        public static void UseAllianceBusinessPlatform(this IApplicationBuilder app, IConfiguration Configuration,
            IHostEnvironment Environment, SuiteOptions Options)
        {

            // Enable Sensitive Exceptions on dev
            if (Environment.IsDevelopment())
            {
                app.UseDatabaseErrorPage();
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        context.Response.Redirect("/Error/E500");
                    });
                });
                // The default HSTS value is 30 days.
                app.UseHsts();
            }

            if (Options.ABP?.Http?.Enable ?? false)
            {

                // Pre-Routing Middleware
                app.Use(async (context, next) =>
                {
                    var parameters = context.Request.RouteValues;
                    var url = context.Request.Path.Value;
                    var host = context.Request.Host;

                    // Manage Headers
                    foreach (var Header in Options.ABP?.Http?.HttpHeaders)
                    {
                        context.Response.Headers.Add(Header.Key, Header.Value);
                    }

                    foreach (var Redirection in Options.ABP?.Http?.Redirections)
                    {
                        // Redirect to an external URL
                        if (url.ToLower().Contains(Redirection?.Contains) && !url.ToLower().Contains(Redirection?.NotContains))
                        {
                            context.Response.Redirect(Redirection?.RedirectTo?.ToString());
                            // short circuit
                            return;
                        }
                    }

                    await next();
                });
            }

            if (Options.ABP?.Apis?.RestApi?.Enable ?? false)
            {
                // Use REST API Service
                app.UseRestApiService(Configuration, Environment, Options);
            }

            if (Options.ABP?.Apis?.GraphQlApi?.Enable ?? false)
            {
                // Use GraphQL API Service
                app.UseGraphQlApiService(Configuration, Environment, Options);
            }

            if (Options.ABP?.Localization?.Enable ?? false)
            {
                // Use Localization
                app.UseRequestLocalization(new RequestLocalizationOptions
                {
                    // Set Default Culture
                    DefaultRequestCulture = new RequestCulture(culture: DefaultCulture, uiCulture: DefaultCulture),
                    // Formatting numbers, dates, etc.
                    SupportedCultures = supportedCultures,
                    // UI strings that we have localized.
                    SupportedUICultures = supportedCultures
                });
            }

            // UseCors must be called before UseResponseCaching when using UseResponseCaching.
            if (Options.ABP?.Cors?.Enable ?? false)
            {
                app.UseCors(CorsPolicy);
            }

            if (Options.ABP?.Routing?.Enable ?? false)
            {
                app.UseRouting();
            }

            if (Options.ABP?.Apis?.IpRateLimiting?.Enable ?? false)
            {
                app.UseIpRateLimiting();
            }

            if (Options.ABP?.Caching?.Enable ?? false)
            {
                if (Options.ABP?.Caching?.ResponseCaching?.Enable ?? false)
                    app.UseResponseCaching();
            }

            if (Options.ABP?.StaticFiles?.Enable ?? false)
            {
                app.UseStaticFiles();

                Options.ABP.Modular.Modules = PluginManager.GetModulesManifest();

                foreach (var Module in Options.ABP.Modular.Modules)
                {
                    app.UseStaticFiles(new StaticFileOptions
                    {
                        FileProvider = new PhysicalFileProvider(Path.Combine(Environment.ContentRootPath, $"{Module.Path}", "staticwebassets")),
                        RequestPath = $"/_content/{Module.ID}"
                    });
                }
            }

            if (Options.ABP?.Logging?.Enable ?? false)
            {
                if (Options.ABP?.Logging?.Serilog.Enable ?? false)
                {
                    app.UseSerilogRequestLogging();
                }
            }

            if (Options.APS?.Enable ?? false)
            {
                app.UseAlliancePassportServices(Configuration, Environment, Options);
            }

            if (Options.ABP?.Cookies?.CookiePolicy?.Enable ?? true)
            {
                app.UseCookiePolicy();
            }

            if (Options.ABP?.WebSockets?.Enable ?? false)
            {
                app.UseWebSockets();
            }

            if (Options.ABP?.Http?.HttpsRedirection?.Enable ?? false)
            {
                app.UseHttpsRedirection();
            }

            if ((Options.ABP?.HealthChecks?.Enable ?? false))
            {
                app.UseHealthChecksPlatform(Configuration, Environment, Options);
            }

            if ((Options.ABP?.Routing?.Redirections?.StatusCodePages?.Enable ?? false))
            {
                if (!Environment.IsDevelopment())
                {
                    app.UseStatusCodePagesWithRedirects("/Error/E{0}");
                }
            }

            if ((Options.ABS?.ControllersWithViews?.Endpoints?.Enable ?? false))
            {
                try
                {
                    app.UseEndpoints(routes =>
                    {
                        if ((Options.ABP?.HealthChecks.Enable ?? false))
                        {
                            if ((Options.ABP?.HealthChecks?.MapHealthChecks ?? false))
                            {
                                routes.MapHealthChecks(Options.ABP?.HealthChecks?.Endpoint ?? "/health");
                            }

                            if (Options.ABP?.HealthChecks?.HealthChecksUi?.Enable ?? false)
                            {
                                routes.MapHealthChecksUI(setup =>
                                {
                                    foreach (var style in Options.ABP?.HealthChecks?.HealthChecksUi?.Styles)
                                    {
                                        setup.AddCustomStylesheet(style.Path);
                                    }
                                });
                            }
                        }

                        if (Options.ABS?.RazorPages?.Enable ?? false)
                        {
                            routes.MapRazorPages();
                        }

                        if (Options.ABS?.Blazor?.Enable ?? false)
                        {
                            // if (Options.ABS?.Blazor?.BlazorHub. ?? false)
                            //{
                            routes.MapBlazorHub();
                            //}
                        }

                        if (Options.ABS?.Blazor != null && (Options.ABS?.Blazor.Enable ?? false))
                        {
                            foreach (var Page in Options.ABS?.Blazor?.BlazorFallbackPages ?? new List<BlazorFallbackPage>())
                            {
                                // Using overloads
                                if (String.IsNullOrEmpty(Page.Pattern))
                                {
                                    routes.MapFallbackToPage(Page.Page);
                                }
                                else
                                {
                                    routes.MapFallbackToPage(Page.Pattern, Page.Page);
                                }
                            }
                        }

                        if ((Options.ABP?.WebSockets?.Enable ?? false))
                        {
                            // TODO: Add types based on typename
                            routes.MapHub<ChatHUB>("/ChatHUB");
                        }

                        if (Options.ABS?.ControllersWithViews == null)
                            return;

                        if (Options.ABS?.ControllersWithViews?.Endpoints?.AreaControllerRoutes != null)
                        {
                            foreach (var area in Options.ABS?.ControllersWithViews?.Endpoints?.AreaControllerRoutes)
                            {
                                routes.MapAreaControllerRoute(area.Name, area.AreaName, area.Pattern);
                            }
                        }

                        if (Options.ABS?.ControllersWithViews?.Endpoints?.ControllerRoutes == null)
                            return;

                        foreach (var controllerRoute in Options.ABS?.ControllersWithViews?.Endpoints?.ControllerRoutes)
                        {
                            routes.MapControllerRoute(controllerRoute.Name, controllerRoute.Pattern);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="apm"></param>
        public static async Task<ApplicationPartManager> ConfigureApplicationParts(ApplicationPartManager apm)
        {

            var ModulesManifest = PluginManager.GetModulesManifest();

            var entryAssembly = Assembly.GetEntryAssembly();


            Console.WriteLine($"Trying to load plugins for entry assembly {entryAssembly.FullName} at path {ModulesFolder} ");

            if (!Directory.Exists(ModulesFolder))
            {
                Directory.CreateDirectory(ModulesFolder);
            }

            await ExtractNugetPackages(Directory.GetFiles(ModulesFolder, "*.nupkg", SearchOption.AllDirectories).ToList());

            var pluginFolders = Directory.GetDirectories(ModulesFolder);

            foreach (var pluginFolder in pluginFolders)
            {
                Console.WriteLine($"Trying to load plugin at: {pluginFolder}.");
                try
                {
                    apm = await ConfigureApplicationPart(pluginFolder, apm);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to load plugin at: {pluginFolders}. Error: {ex}.");
                }
            }

            apm?.FeatureProviders.Add(new ViewComponentFeatureProvider());

            
            return apm;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="apm"></param>
        public static async Task<ApplicationPartManager> ConfigureApplicationPart(string path, ApplicationPartManager apm = null, SuiteOptions Options = null)
        {
            if (Directory.Exists(path))
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


                    var PluginSettings = new ACL.Configuration.Types.ABP.Modular.Module()
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

                                // Assembly
                                // In the.NET framework Assembly is the minimum unit of deployment and it is the form of an EXE or a DLL.
                                //    - An assembly can contain one or more files, they can include things like resource files or modules(embedded).
                                //    - An assembly always contains an assembly manifest.Assembly manifest is the metadata of an assembly. 
                                //          It contains assembly definition identity, files in the assembly, type reference information and more.
                                //    - The Assembly Linker(Al.exe) generates a file that has the assembly manifest in it.
                                //    - You can view the topology of an assembly by using the IL Disassembler(ildasm.exe).

                                // Modules
                                // The code files in an assembly are called modules.
                                //   - A module is a unit of compilation.
                                //   - Module contains type metadata.
                                //   - A module can not be deployed alone, it has to be linked into an assembly(using Al.exe).

                                var assembly = Assembly.LoadFrom(assemblyFile);
                                var managedAssembly = PluginManager.LoadPlugin(assemblyFile);

                                var assemblyManifestResourceNames = assembly.GetManifestResourceNames();
                                var assemblyReferencedAssemblies = assembly.GetReferencedAssemblies();
                                var assemblyModules = assembly.GetModules(true);
                                var assemblyTypes = assembly.GetTypes();
                                var assemblyExportedTypes = assembly.GetExportedTypes();
                                var assemblySecurityRuleSet = assembly.SecurityRuleSet;
                                var assemblyReflectionOnly = assembly.ReflectionOnly;
                                var assemblyExecutionOnly = !assembly.ReflectionOnly;
                                var assemblyIsFullyTrusted = assembly.IsFullyTrusted;
                                var assemblyIsDynamic = assembly.IsDynamic;
                                var assemblyLocation = assembly.Location;
                                var assemblyIsCollectible = assembly.IsCollectible;
                                PluginSettings.AssemblyPaths.ToList().Add(assemblyLocation);


                                Plugins.AddRange(PluginLoadContext.InstantiatePlugin(managedAssembly));

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
        /// 
        /// </summary>
        /// <param name="NugetPackagePaths"></param>
        /// <returns></returns>
        public static async Task ExtractNugetPackages(List<string> NugetPackagePaths)
        {

            int NugetPackageErrors = 0;

            try
            {
                Console.WriteLine($"Trying to extract nuget plugin files at path: {ModulesFolder} ");

                foreach (var nugetPackage in NugetPackagePaths)
                {
                    try
                    {
                        Console.WriteLine($"Trying to extract nuget package file: {nugetPackage} ");
                        var newPath = Path.Join(Path.GetDirectoryName(nugetPackage), Path.GetFileNameWithoutExtension(nugetPackage));
                        // TODO: Extract nuget package
                        ZipFile.ExtractToDirectory(nugetPackage, newPath, true);

                    }
                    catch
                    {
                        Console.WriteLine($"Failed to Extract nuget package at {nugetPackage}");
                        NugetPackageErrors++;
                    }
                }
            }
            catch
            {
                Console.WriteLine($"Failed to Extract {NugetPackageErrors}/{NugetPackagePaths?.Count() ?? 0} nuget package at {ModulesFolder}");
            }
        }


    }
}