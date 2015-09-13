using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Rebus.CastleWindsor;

namespace UnitOfWork.Installers
{
    public class RebusHandlersInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.AutoRegisterHandlersFromThisAssembly();
        }
    }
}