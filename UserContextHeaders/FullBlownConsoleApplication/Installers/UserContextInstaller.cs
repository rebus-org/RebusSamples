using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FullBlownConsoleApplication.Config;
using FullBlownConsoleApplication.Models;
using Rebus;

namespace FullBlownConsoleApplication.Installers
{
    public class UserContextInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<UserContext>()
                    .UsingFactoryMethod(k =>
                    {
                        var messageContext = MessageContext.GetCurrent();

                        return messageContext.GetUserContext();
                    })
                    .LifestyleTransient()
                );
        }
    }
}