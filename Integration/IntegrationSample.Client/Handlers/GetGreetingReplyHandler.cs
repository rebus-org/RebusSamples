using System;
using System.Threading.Tasks;
using IntegrationSample.IntegrationService.Messages;
using Rebus.Handlers;

namespace IntegrationSample.Client.Handlers
{
    public class GetGreetingReplyHandler : IHandleMessages<GetGreetingReply>
    {
        public async Task Handle(GetGreetingReply message)
        {
            Console.WriteLine("Got greeting reply: {0}", message.TheGreeting);
        }
    }
}