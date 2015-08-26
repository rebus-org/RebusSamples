using System;
using Castle.Windsor;
using Castle.Windsor.Installer;
using FullBlownConsoleApplication.Config;
using FullBlownConsoleApplication.Helpers;
using FullBlownConsoleApplication.Messages;
using FullBlownConsoleApplication.Models;
using Rebus;
using Rebus.Castle.Windsor;
using Rebus.Configuration;
using Rebus.Logging;
using Rebus.Transports.Msmq;

namespace FullBlownConsoleApplication
{
    class Program
    {
        static void Main()
        {
            using (var appContainer = new WindsorContainer().Install(FromAssembly.This()))
            {
                Configure.With(new WindsorContainerAdapter(appContainer))
                    .Logging(l => l.ColoredConsole(minLevel: LogLevel.Warn))
                    .Transport(t => t.UseMsmq("sample.usercontextheaders.input", "sample.usercontextheaders.error"))
                    
                    .AutomaticallyTransferUserContext()   //< this one is special :)
                    
                    .CreateBus()
                    .Start();

                while (true)
                {
                    Console.Write(@"Type name of person whose context to impersonate, or leave empty to quit
> ");
                    var name = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(name)) break;

                    var notAnActualId = name.GetHashCode();
        
                    var userContext = new UserContext {Name = name, UserId = notAnActualId};

                    using (new AmbientUserContext(userContext))
                    {
                        var bus = appContainer.Resolve<IBus>();
                        var greeting = PreFabGreetings.GetOne();

                        var message = new SomeRandomMessage(greeting);

                        bus.SendLocal(message);
                    }
                }
            }
        }
    }
}
