using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Rebus;

namespace FullBlownConsoleApplication.Installers
{
    public class RebusHandlersInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Classes.FromThisAssembly()
                    .BasedOn<IHandleMessages>()
                    .WithServiceAllInterfaces()
                    .LifestyleTransient()
                );
        }
    }
}