using System;
using Castle.Windsor;
using Castle.Windsor.Installer;

namespace EmailSender
{
    class Program
    {
        static void Main()
        {
            using (var container = new WindsorContainer())
            {
                container.Install(FromAssembly.This());

                Console.WriteLine("Press ENTER to exit");
                Console.ReadLine();
            }
        }
    }
}
