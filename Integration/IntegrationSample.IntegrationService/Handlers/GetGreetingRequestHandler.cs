using System.Net.Http;
using System.Threading.Tasks;
using IntegrationSample.IntegrationService.Messages;
using Rebus.Bus;
using Rebus.Handlers;

namespace IntegrationSample.IntegrationService.Handlers
{
    public class GetGreetingRequestHandler : IHandleMessages<GetGreetingRequest>
    {
        readonly IBus _bus;
        readonly HttpClient _httpClient;

        public GetGreetingRequestHandler(IBus bus, HttpClient httpClient)
        {
            _bus = bus;
            _httpClient = httpClient;
        }

        public async Task Handle(GetGreetingRequest message)
        {
            var greeting = await _httpClient.GetStringAsync("http://localhost:12345/api/get/greeting");

            var reply = new GetGreetingReply { TheGreeting = greeting };

            await _bus.Reply(reply);
        }
    }
}