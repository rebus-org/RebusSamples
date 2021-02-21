using System;
using System.Threading.Tasks;
using Rebus.Handlers;
using SqlAllTheWay.Repositories;

namespace SqlAllTheWay.Handlers
{
    class StringMessageHandler : IHandleMessages<string>
    {
        static readonly Random Random = new Random(DateTime.Now.GetHashCode());

        readonly IReceivedStringsRepository _receivedStringsRepository;

        public StringMessageHandler(IReceivedStringsRepository receivedStringsRepository)
        {
            _receivedStringsRepository = receivedStringsRepository;
        }

        public async Task Handle(string message)
        {
            // do some work
            await _receivedStringsRepository.Insert(message);

            // throw randomly
            if (Random.Next(2) == 0)
            {
                throw new ApplicationException("UH OH AN ERROR OCCURRED");
            }
        }
    }
}