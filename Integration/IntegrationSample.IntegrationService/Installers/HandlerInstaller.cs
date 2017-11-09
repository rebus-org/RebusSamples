using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using IntegrationSample.IntegrationService.Handlers;
using Rebus.CastleWindsor;

namespace IntegrationSample.IntegrationService.Installers
{
    public class HandlerInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.RegisterHandler<GetGreetingRequestHandler>();
        }
    }
}