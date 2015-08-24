using System.Threading.Tasks;
using IntegrationSample.IntegrationService.Messages;
using IntegrationSample.IntegrationService.SomethingExternal;
using Rebus.Bus;
using Rebus.Handlers;

namespace IntegrationSample.IntegrationService.Handlers
{
    public class GetGreetingRequestHandler : IHandleMessages<GetGreetingRequest>
    {
        readonly IBus _bus;

        public GetGreetingRequestHandler(IBus bus)
        {
            _bus = bus;
        }

        public async Task Handle(GetGreetingRequest message)
        {
            using (var client = new Service1Client())
            {
                var greeting = client.GetGreeting();

                var reply = new GetGreetingReply {TheGreeting = greeting};

                await _bus.Reply(reply);
            }
        }
    }
}