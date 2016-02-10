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
        const string ListeUrl = "http://localhost:3000";

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
                        o.AddWebHost(ListeUrl, app =>
                        {
                            app.UseFileServer(new FileServerOptions
                            {
                                FileSystem = new PhysicalFileSystem(webAppBaseDir),
                                DefaultFilesOptions = { DefaultFileNames = { "index.html" } }
                            });

                            app.Map("/api/hello", a => a.Use(GetTimedGreeting));
                        });
                    })
                    .Start();

                Process.Start(ListeUrl);

                Console.WriteLine("Press ENTER to quit");
                Console.ReadLine();
            }
        }

        static async Task GetTimedGreeting(IOwinContext context, Func<Task> next)
        {
            var response = context.Response;
            response.ContentType = "application/json;charset=utf-8";
            var jsonText = @"{""message"": ""The time is " + DateTime.Now.ToString("hh:mm:ss") + @" on the server""}";
            await response.WriteAsync(jsonText);
        }
    }
}
