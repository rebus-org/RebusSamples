using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.SimpleInjector;
using Rebus.Transport.InMem;
using SimpleInjector;

namespace SimpleInjectorTest
{
    class Program
    {
        static void Main()
        {
            using (var container = new Container())
            {
                container.RegisterCollection<IHandleMessages<string>>(new []{ typeof(StringHandler) });

                var bus = Configure.With(new SimpleInjectorContainerAdapter(container))
                    .Transport(t => t.UseInMemoryTransport(new InMemNetwork(), "simple-injector-test"))
                    .Start();

                bus.Advanced.SyncBus.SendLocal("Good day, sir.");

                Console.WriteLine("Press ENTER to quit");
                Console.ReadLine();
            }
        }
    }

    class StringHandler : IHandleMessages<string>
    {
        public async Task Handle(string message)
        {
            Console.WriteLine($@"              ___.                 
_______   ____\_ |__  __ __  ______
\_  __ \_/ __ \| __ \|  |  \/  ___/
 |  | \/\  ___/| \_\ \  |  /\___ \ 
 |__|    \___  >___  /____//____  >
             \/    \/           \/ 
got message: '{message}'
");
        }
    }
}