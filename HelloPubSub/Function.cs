using CloudNative.CloudEvents;
using Google.Cloud.Functions.Framework;
using Google.Events.Protobuf.Cloud.PubSub.V1;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HelloPubSub
{
    public class Function : ICloudEventFunction<MessagePublishedData>
    {
        public Task HandleAsync(CloudEvent cloudEvent, MessagePublishedData data, CancellationToken cancellationToken)
        {
            var nameFromMessage = data.Message?.TextData;
            var name = string.IsNullOrEmpty(nameFromMessage) ? "world" : nameFromMessage;
            Console.WriteLine($"Hello {name}");
            return Task.CompletedTask;
        }
    }
}
