using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rebus.Extensions;
using Rebus.Messages;
using Rebus.Serialization;
#pragma warning disable 1998

namespace Subscriber
{
    class CustomMessageDeserializer : ISerializer
    {
        /// <summary>
        /// If the type name found in the '<see cref="Headers.Type"/>' header can be found in this dictionary, the incoming
        /// message will be deserialized into the specified type
        /// </summary>
        static readonly ConcurrentDictionary<string, Type> KnownTypes = new ConcurrentDictionary<string, Type>
        {
            ["Publisher.GreetingWasEntered_Publisher, Publisher"] = typeof(GreetingWasEntered_Subscriber)
        };

        readonly ISerializer _serializer;

        public CustomMessageDeserializer(ISerializer serializer) => _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));

        public Task<TransportMessage> Serialize(Message message) => _serializer.Serialize(message);

        public async Task<Message> Deserialize(TransportMessage transportMessage)
        {
            var headers = transportMessage.Headers.Clone();
            var json = Encoding.UTF8.GetString(transportMessage.Body);
            var typeName = headers.GetValue(Headers.Type);

            // if we don't know the type, just deserialize the message into a JObject
            if (!KnownTypes.TryGetValue(typeName, out var type))
            {
                return new Message(headers, JsonConvert.DeserializeObject<JObject>(json));
            }

            var body = JsonConvert.DeserializeObject(json, type);

            return new Message(headers, body);
        }
    }
}