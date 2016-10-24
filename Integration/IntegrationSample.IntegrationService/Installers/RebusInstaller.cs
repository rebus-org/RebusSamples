using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Rebus.Config;
using Rebus.Log4net;

namespace IntegrationSample.IntegrationService.Installers
{
    public class RebusInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            Configure.With(new CastleWindsorContainerAdapter(container))
                .Logging(l => l.Log4Net())
                .Transport(t => t.UseMsmq("IntegrationSample.IntegrationService.input"))
                .Start();
        }
    }
}