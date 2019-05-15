using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Orleans;
using Orleans.Hosting;
using System.IO;
using System;
using WebBookmarks.Grains;
using System.Net;
using System.Net.Sockets;

namespace WebBookmarks.Silo
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            IHostBuilder host;
            IConfiguration configuration = GetConfiguration();

            IPAddress ipAddress = IPAddress.Parse(GetLocalIPAddress());
            IPEndPoint iPEndPoint = new IPEndPoint(ipAddress, 5101);

            try
            {
                host = new HostBuilder()
                    .UseEnvironment(environment)
                    .ConfigureAppConfiguration(config =>
                    {
                        config.AddConfiguration(configuration);
                    })
                    .UseOrleans((context, siloBuilder) =>
                    {


                        siloBuilder
                            .ConfigureEndpoints(siloPort: 11111, gatewayPort: 30000)
                            //.UseDevelopmentClustering(iPEndPoint)
                            .UseLocalhostClustering(/*gatewayPort: 5101, siloPort: 5101*/)
                            .Configure<ClusterOptions>(options =>
                            {
                                options.ClusterId = "dev";
                                options.ServiceId = "WebBookmarks";
                            })
                            .AddMongoDBGrainStorage("mongodb", options =>
                            {
                                options.ConnectionString = "mongodb://localhost:27017";
                                options.DatabaseName = "WebBookmarksDB";
                            })
                            .ConfigureApplicationParts(parts =>
                            {
                                parts.AddApplicationPart(typeof(BookmarkStorage).Assembly).WithReferences();
                            });
                            //.Configure<EndpointOptions>(options =>
                            //{
                            //    options.GatewayListeningEndpoint = iPEndPoint;
                            //    options.SiloListeningEndpoint = iPEndPoint;
                            //    options.SiloPort = 5101;
                            //});
                    })
                    //.ConfigureServices((context, services) =>
                    //{
                    //    //var config = context.Configuration;
                    //    //var conStringbookmarksDB = config.GetConnectionString("BookmarkDb");

                    //})
                    .ConfigureLogging((context, logging) =>
                    {
                        logging.AddConfiguration(context.Configuration.GetSection("Logging"));
                        logging.AddConsole();
                    });

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error building silo host: " + ex.InnerException.Message);
                throw;
            }

            // Make Ctrl-C stop our process
            Console.CancelKeyPress += (sender, e) =>
            {
                Environment.Exit(0);
            };

            try
            {
                Console.WriteLine("Silo starting...");
                await host.Build().StartAsync();
                Console.WriteLine("Silo started");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Silo could not be started with exception: " + ex.InnerException.Message);
            }
        }

        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                //.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
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
