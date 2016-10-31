using System.Linq;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Rebus.Bus;
using SagaDemo.Messages;

namespace SagaDemo.Installers
{
    public class StartupActions : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var eventTypes = new[]
            {
                typeof(AmountsCalculated),
                typeof(TaxesCalculated),
                typeof(PayoutMethodSelected),
            };

            var bus = container.Resolve<IBus>();

            Task.WaitAll(eventTypes.Select(type => bus.Subscribe(type)).ToArray());
        }
    }
}