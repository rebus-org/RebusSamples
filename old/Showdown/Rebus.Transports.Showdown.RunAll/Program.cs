using System;

namespace Rebus.Transports.Showdown.RunAll
{
    class Program
    {
        static void Main()
        {
            SqlServer.Program.Main();
            PostgreSql.Program.Main(); 
            Msmq.Program.Main();
            RabbitMq.Program.Main(); 
            Console.WriteLine("Showdown complete, press any key to continue....");
            Console.ReadKey();
        }
    }
}
