using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using System;
using WebBookmarks.GrainInterfaces;
using Microsoft.Extensions.Logging;
using WebBookmarks.API.Infrastructure.Filters;
using Polly;
using System.Threading.Tasks;
using Orleans.Runtime;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Orleans.Configuration;
using System.IO;
using System.Collections.Generic;

namespace WebBookmarks.API
{
    public class Startup
    {
        private readonly ILogger<Startup> _logger;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            _logger = logger;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddCustomMVC()
                .AddSwagger()
                .AddClusterClient(Configuration, _logger).Wait();

        }



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger()
                  .UseSwaggerUI(c =>
                  {
                      c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebBookmarks.API V1");
                  });
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvcWithDefaultRoute();
        }
    }

    public static class CustomExtensionMethods
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
                {
                    Title = "WebBookmarks - WebBookmarks HTTP API",
                    Version = "v1",
                    Description = "The WebBookmarks HTTP API.",
                    TermsOfService = "Terms Of Service"
                });
            });

            return services;
        }

        public static IServiceCollection AddCustomMVC(this IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
            })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddControllersAsServices();

            return services;
        }

        public async static Task<IServiceCollection> AddClusterClient(this IServiceCollection services, IConfiguration config, ILogger<Startup> logger)
        {
            //var config = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddInMemoryCollection(new Dictionary<string, string> // add default settings, that will be overridden by commandline
            //    {
            //        {"ClusterId", "dev"},
            //        {"ServiceId", "WebBookmarks"}
            //    })
            //    .Build();

            //int attempt = 0;
            //int initializeAttemptsBeforeFailing = 7;
            //IClusterClient clusterClient = null;

            //while (true)
            //{
            //    // Initialize orleans client
            //    clusterClient = new ClientBuilder()
            //        .ConfigureServices((context, services) =>
            //        {
            //        // services.AddOptions();
            //        //services.AddSingleton<ILoggerFactory>(loggerFactory);
            //        // services.AddSingleton<IConfiguration>(config);
            //        services.Configure<ClusterOptions>(config);
            //        })
            //        .ConfigureApplicationParts(parts =>
            //        {
            //            parts.AddApplicationPart(typeof(IBookmarkStorageGrain).Assembly).WithReferences();
            //        })
            //        .Build();

            //    try
            //    {
            //        await clusterClient.Connect().ConfigureAwait(false);
            //        logger.LogInformation("Client successfully connected to silo host");
            //        break;
            //    }
            //    catch (OrleansException)
            //    {
            //        logger.LogWarning($"Attempt {attempt} of {initializeAttemptsBeforeFailing} failed to initialize the Orleans client.");

            //        if (clusterClient != null)
            //        {
            //            clusterClient.Dispose();
            //            clusterClient = null;
            //        }

            //        if (attempt > initializeAttemptsBeforeFailing)
            //        {
            //            throw;
            //        }

            //        // Wait 4 seconds before retrying
            //        await Task.Delay(TimeSpan.FromSeconds(4));
            //    } 
            //}






            IPAddress ipAddress = IPAddress.Parse(GetLocalIPAddress());
            IPEndPoint iPEndPoint = new IPEndPoint(ipAddress, 5101);

            var client = new ClientBuilder()
                .UseLocalhostClustering(gatewayPort: 30000, clusterId: "dev", serviceId: config["ServiceId"])
                .ConfigureApplicationParts(parts =>
                {
                    parts.AddApplicationPart(typeof(IBookmarkStorageGrain).Assembly).WithReferences();
                })
                .ConfigureLogging(_ => _.AddConsole())
                .Build();

            var retry = Policy.Handle<AggregateException>()
                            .WaitAndRetryAsync(new TimeSpan[]
                            {
                                    TimeSpan.FromSeconds(3),
                                    TimeSpan.FromSeconds(5),
                                    TimeSpan.FromSeconds(8)
                            });


            try
            {
                await retry.ExecuteAsync(async () =>
                {
                    logger.LogInformation("Trying to connect to the Silo.");
                    await client.Connect();
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "The Client can't connect to Silo.");
                throw;
            }

            services.AddSingleton(client);
            return services;
        }

        private static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}
