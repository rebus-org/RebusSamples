using System;
using FullBlownConsoleApplication.Messages;
using FullBlownConsoleApplication.Models;
using Rebus;

namespace FullBlownConsoleApplication.Handlers
{
    public class SomeRandomMessageHandler : IHandleMessages<SomeRandomMessage>
    {
        readonly UserContext _userContext;

        public SomeRandomMessageHandler(UserContext userContext)
        {
            _userContext = userContext;
        }

        public void Handle(SomeRandomMessage message)
        {
            Console.WriteLine("Got message with greeting: {0} - current user context: {1}", message.Greeting, _userContext.Name);
        }
    }
}