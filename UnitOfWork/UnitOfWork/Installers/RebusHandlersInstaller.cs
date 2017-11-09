using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Rebus.CastleWindsor;
using UnitOfWork.Handlers;

namespace UnitOfWork.Installers
{
    public class RebusHandlersInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.AutoRegisterHandlersFromAssemblyOf<FailSometimes>();
        }
    }
}