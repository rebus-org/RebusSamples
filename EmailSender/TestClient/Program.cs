using System;
using EmailSender.Messages;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Routing.TypeBased;

namespace TestClient
{
    class Program
    {
        static void Main()
        {
            using var bus = Configure.OneWayClient()
                .Transport(t => t.UseMsmqAsOneWayClient())
                .Routing(r => r.TypeBased().MapAssemblyOf<SendEmail>("emailsender"))
                .Start();

            using (bus)
            {
                while (true)
                {
                    var recipient = ReadLine("recipient");

                    if (string.IsNullOrEmpty(recipient))
                    {
                        break;
                    }

                    var subject = ReadLine("subject");
                    var body = ReadLine("body");

                    bus.Send(new SendEmail(recipient, subject, body)).Wait();
                }
            }
        }

        static string ReadLine(string what)
        {
            Console.Write($"Please enter {what} > ");
            var text = Console.ReadLine();
            return text;
        }
    }
}
