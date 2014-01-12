using System;
using System.Runtime.InteropServices;
using Rebus.Configuration;
using Rebus.Logging;
using Rebus.Transports.Msmq;

namespace EndpointWithTimeouts
{
    class Program
    {
        static void Main()
        {
            using (var adapter = new BuiltinContainerAdapter())
            {
                adapter.Handle<DeferredMessage>(msg => Console.WriteLine("Got message sent {0} - current time: {1}", msg.Sent, DateTime.Now));

                Configure.With(adapter)
                    .Logging(l => l.ColoredConsole(minLevel:LogLevel.Warn))
                    .Transport(t => t.UseMsmq("test_input", "error"))
                    .CreateBus()
                    .Start();

                while (true)
                {
                    Console.WriteLine("Press 0...9 to send message n seconds into the future or Q to quit");
                    
                    var key = Console.ReadKey(true);

                    switch (key.KeyChar)
                    {
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                            var seconds = int.Parse(key.KeyChar.ToString());
                            var delay = TimeSpan.FromSeconds(seconds);
                            Console.WriteLine("Sending message deferred by {0}", delay);
                            adapter.Bus.Defer(delay, new DeferredMessage {Sent = DateTime.Now});
                            break;

                        case 'q':
                        case 'Q':
                            goto this_is_the_end; //< omg omg omg!!!!1
                    }
                }

            this_is_the_end:
                Console.WriteLine("Quitting...");

            }
        }

        class DeferredMessage
        {
            public DateTime Sent { get; set; }
        }
    }
}
