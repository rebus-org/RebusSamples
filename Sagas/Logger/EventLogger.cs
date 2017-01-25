using System.Threading.Tasks;
using Rebus.Handlers;
using SagaDemo.Messages;
using Serilog;
#pragma warning disable 1998

namespace Logger
{
    class EventLogger : IHandleMessages<IEventWithCaseNumber>
    {
        static readonly ILogger Logger = Log.ForContext<EventLogger>();

        public async Task Handle(IEventWithCaseNumber message)
        {
            Logger.Information("Got event {EventName} for case {CaseNumber}", message.GetType().Name, message.CaseNumber);
        }
    }
}