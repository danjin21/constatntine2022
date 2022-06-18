using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AccountServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {




                    webBuilder.ConfigureKestrel(options =>
                    {
                        options.ListenAnyIP(5000);
                        options.ListenAnyIP(5001, (httpsOpt) =>
                        {
                            httpsOpt.UseHttps();
                        });
                    });


                    webBuilder.UseStartup<Startup>();

                    //webBuilder.ConfigureKestrel(serverOptions =>
                    //{
                    //    serverOptions.ListenAnyIP(5000);
                    //    serverOptions.ListenAnyIP(5001);
                    //});





                    //webBuilder.UseKestrel(opts =>
                    //{
                    //    // Bind directly to a socket handle or Unix socket
                    //    // opts.ListenHandle(123554);
                    //    // opts.ListenUnixSocket("/tmp/kestrel-test.sock");

                    //    opts.Listen(IPAddress.Loopback, port: 5002);
                    //    opts.ListenAnyIP(5003);
                    //    opts.ListenLocalhost(5004, opts => opts.UseHttps());
                    //    opts.ListenLocalhost(5005, opts => opts.UseHttps());

                    //});

                });
    }
}
