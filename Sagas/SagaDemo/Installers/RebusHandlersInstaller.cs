using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Rebus.CastleWindsor;
using SagaDemo.Handlers;

namespace SagaDemo.Installers
{
    public class RebusHandlersInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // just register all Rebus handlers from this assembly
            container.RegisterHandler<PayoutSaga>();
        }
    }
}