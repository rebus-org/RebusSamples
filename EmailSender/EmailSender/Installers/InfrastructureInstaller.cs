using System;
using System.IO;
using System.Net.Mail;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace EmailSender.Installers
{
    public class InfrastructureInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<SmtpClient>()
                    .UsingFactoryMethod(k =>
                    {
                        var pickupDirectoryLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "emails");

                        if (!Directory.Exists(pickupDirectoryLocation))
                        {
                            Directory.CreateDirectory(pickupDirectoryLocation);
                        }

                        return new SmtpClient
                        {
                            DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
                            PickupDirectoryLocation = pickupDirectoryLocation
                        };
                    })
            );
        }
    }
}