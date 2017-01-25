using System;
using System.Threading.Tasks;
using Rebus.Bus;
using Rebus.Handlers;
using Rebus.Sagas;
using SagaDemo.Messages;
using Serilog;

namespace SagaDemo.Handlers
{
    public class PayoutSaga : Saga<PayoutSagaData>, IAmInitiatedBy<AmountsCalculated>, IAmInitiatedBy<TaxesCalculated>, IAmInitiatedBy<PayoutMethodSelected>, IHandleMessages<VerifyComplete>
    {
        static readonly ILogger Logger = Log.ForContext<PayoutSaga>();

        readonly IBus _bus;

        public PayoutSaga(IBus bus)
        {
            _bus = bus;
        }

        protected override void CorrelateMessages(ICorrelationConfig<PayoutSagaData> config)
        {
            // events of interest
            config.Correlate<AmountsCalculated>(m => m.CaseNumber, d => d.CaseNumber);
            config.Correlate<TaxesCalculated>(m => m.CaseNumber, d => d.CaseNumber);
            config.Correlate<PayoutMethodSelected>(m => m.CaseNumber, d => d.CaseNumber);

            // internal verification message
            config.Correlate<VerifyComplete>(m => m.CaseNumber, d => d.CaseNumber);
        }

        public async Task Handle(AmountsCalculated message)
        {
            await Pre();

            Logger.Information("Setting {FieldName} to true for case {CaseNumber}", "AmountsCalculated", Data.CaseNumber);

            Data.AmountsCalculated = true;

            await Post();
        }

        public async Task Handle(TaxesCalculated message)
        {
            await Pre();

            Logger.Information("Setting {FieldName} to true for case {CaseNumber}", "TaxesCalculated", Data.CaseNumber);

            Data.TaxesCalculated = true;

            await Post();
        }

        public async Task Handle(PayoutMethodSelected message)
        {
            await Pre();

            Logger.Information("Setting {FieldName} to true for case {CaseNumber}", "PayoutMethodSelected", Data.CaseNumber);

            Data.PayoutMethodSelected = true;

            await Post();
        }

        public async Task Handle(VerifyComplete message)
        {
            Logger.Warning("The saga for case {CaseNumber} was not completed within {TimeoutSeconds} s timeout", Data.CaseNumber, 20);

            await _bus.Publish(new PayoutNotReady(Data.CaseNumber));

            MarkAsComplete();
        }

        async Task Pre()
        {
            if (!IsNew) return;

            Logger.Information("Ordering wake-up call in {TimeoutSeconds} s for case {CaseNumber}", 20, Data.CaseNumber);

            await _bus.Defer(TimeSpan.FromSeconds(20), new VerifyComplete(Data.CaseNumber));
        }

        async Task Post()
        {
            if (!Data.IsDone) return;

            Logger.Information("Publishing ready event and marking saga for case {CaseNumber} as complete", Data.CaseNumber);

            await _bus.Publish(new PayoutReady(Data.CaseNumber));

            MarkAsComplete();
        }
    }

    public class PayoutSagaData : SagaData
    {
        public string CaseNumber { get; set; }

        public bool AmountsCalculated { get; set; }
        public bool TaxesCalculated { get; set; }
        public bool PayoutMethodSelected { get; set; }

        public bool IsDone => AmountsCalculated
                              && TaxesCalculated
                              && PayoutMethodSelected;
    }
}