using System;
using System.Linq;
using FullBlownConsoleApplication.Models;
using Rebus;
using Rebus.Configuration;

namespace FullBlownConsoleApplication.Config
{
    public static class RebusEx
    {
        /// <summary>
        /// Key of the special encoded user context header
        /// </summary>
        const string UserContextHeaderKey = "my-funky-user-context";

        /// <summary>
        /// Key of <see cref="UserContext"/> instance made available via message context items
        /// </summary>
        const string UserContextItemKey = "current-user-context";

        const string EncodedUserIdKey = "userId";
        const string EncodedNameKey = "name";

        public static RebusConfigurer AutomaticallyTransferUserContext(this RebusConfigurer configurer)
        {
            return configurer.Events(e =>
            {
                e.MessageSent += EncodeUserContextHeadersIfPossible;
                e.MessageContextEstablished += DecodeUserContextHeadersIfPossible;
            });
        }

        public static UserContext GetUserContext(this IMessageContext messageContext)
        {
            return (UserContext) messageContext.Items[UserContextItemKey];
        }

        static void EncodeUserContextHeadersIfPossible(IBus bus, string destination, object message)
        {
            var current = AmbientUserContext.Current;
            if (current == null) return;

            var encodedUserContext = Encode(current);
            
            bus.AttachHeader(message, UserContextHeaderKey, encodedUserContext);
        }

        static void DecodeUserContextHeadersIfPossible(IBus bus, IMessageContext messageContext)
        {
            var headers = messageContext.Headers;
            if (!headers.ContainsKey(UserContextHeaderKey)) return;

            var encodedUserContext = (string) headers[UserContextHeaderKey];
            
            messageContext.Items[UserContextItemKey] = Decode(encodedUserContext);
        }

        static string Encode(UserContext current)
        {
            return string.Format("{0}={1};{2}={3}",
                EncodedUserIdKey, current.UserId,
                EncodedNameKey, current.Name);
        }

        static UserContext Decode(string encodedUserContext)
        {
            try
            {
                var dictionary = encodedUserContext
                    .Split(';')
                    .Select(kvp =>
                    {
                        var tokens = kvp.Split('=');

                        return new {Key = tokens[0], Value = tokens[1]};
                    })
                    .ToDictionary(a => a.Key, a => a.Value, StringComparer.InvariantCultureIgnoreCase);

                return new UserContext
                {
                    UserId = int.Parse(dictionary[EncodedUserIdKey]),
                    Name = dictionary[EncodedNameKey]
                };
            }
            catch (Exception exception)
            {
                throw new FormatException(string.Format("Could not decode user context from string '{0}'", encodedUserContext), exception);
            }
        }
    }
}