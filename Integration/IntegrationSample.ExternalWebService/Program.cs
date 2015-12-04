using System;
using Microsoft.Owin.Hosting;

namespace IntegrationSample.ExternalWebService
{
    class Program
    {
        static void Main()
        {
            const string listenUrl = "http://localhost:12345";

            using (WebApp.Start<OwinApi>(listenUrl))
            {
                Console.WriteLine($"Listening on {listenUrl} - press ENTER to quit");
                Console.ReadLine();
            }
        }
    }
}
