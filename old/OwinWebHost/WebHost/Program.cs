using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Owin;
using Rebus.Transport.InMem;

namespace WebHost
{
    class Program
    {
        const string ListenUrl = "http://localhost:3000";

        static void Main()
        {
            // serve the web app out of the "site" directory directly in the source code - allows for
            // quicker edit/reload iterations
            var webAppBaseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "site");

            using (var activator = new BuiltinHandlerActivator())
            {
                Configure.With(activator)
                    .Transport(t => t.UseInMemoryTransport(new InMemNetwork(), "owin-test"))
                    .Options(o =>
                    {
                        // add the web host
                        o.AddWebHost(ListenUrl, app =>
                        {
                            // serve web application out of the configured base directory
                            app.UseFileServer(new FileServerOptions
                            {
                                FileSystem = new PhysicalFileSystem(webAppBaseDir),
                                DefaultFilesOptions = { DefaultFileNames = { "index.html" } }
                            });

                            // host a simple API
                            app.Map("/api/hello", a => a.Run(GetTimedGreeting));
                        });
                    })
                    .Start();

                // invoke default browser and navigate to the URL
                Process.Start(ListenUrl);

                Console.WriteLine("Press ENTER to quit");
                Console.ReadLine();
            }
        }

        static async Task GetTimedGreeting(IOwinContext context)
        {
            var response = context.Response;
            response.ContentType = "application/json;charset=utf-8";
            var jsonText = $@"{{""message"": ""The time is {DateTime.Now:hh:mm:ss} on the server""}}";
            await response.WriteAsync(jsonText);
        }
    }
}
