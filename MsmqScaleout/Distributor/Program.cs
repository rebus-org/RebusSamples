using System;
using Rebus.Logging;
using Rebus.MsmqLoadBalancer;

namespace Distributor
{
    class Program
    {
        static void Main()
        {
            RebusLoggerFactory.Current = new ConsoleLoggerFactory(false) { MinLevel = LogLevel.Warn };

            var loadBalancer = new LoadBalancerService("distributor")
                .AddDestinationQueue("consumer1")
                .AddDestinationQueue("consumer2");

            using (loadBalancer.Start())
            {
                Console.WriteLine("Rebus load balancer is running - press ENTER to quit");
                Console.ReadLine();
            }
        }
    }
}
