using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Rebus.CastleWindsor;
using Rebus.Config;

namespace EmailSender.Installers
{
    public class RebusInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.AutoRegisterHandlersFromThisAssembly();

            Configure.With(new CastleWindsorContainerAdapter(container))
                .Transport(t => t.UseMsmq("emailsender"))
                .Start();
        }
    }
}