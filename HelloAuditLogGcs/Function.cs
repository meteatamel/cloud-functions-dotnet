using CloudNative.CloudEvents;
using Google.Cloud.Functions.Framework;
using Google.Events.Protobuf.Cloud.Audit.V1;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HelloAuditLogGcs
{
    /// <summary>
    /// A function that can be triggered in responses to changes in Google Cloud
    /// Storage via AuditLogs.
    /// The type argument (LogEntryData in this case) determines how the event payload is deserialized.
    /// The function must be deployed so that the trigger matches the expected payload type. (For example,
    /// deploying a function expecting a StorageObject payload will not work for a trigger that provides
    /// a FirestoreEvent.)
    /// </summary>
    public class Function : ICloudEventFunction<LogEntryData>
    {
        private readonly ILogger _logger;

        public Function(ILogger<Function> logger) =>
            _logger = logger;

        /// <summary>
        /// Logic for your function goes here. Note that a CloudEvent function just consumes an event;
        /// it doesn't provide any response.
        /// </summary>
        /// <param name="cloudEvent">The CloudEvent your function should consume.</param>
        /// <param name="data">The deserialized data within the CloudEvent.</param>
        /// <param name="cancellationToken">A cancellation token that is notified if the request is aborted.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task HandleAsync(CloudEvent cloudEvent, LogEntryData data, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Event: {event}", cloudEvent.Id);
            _logger.LogInformation("Event Type: {type}", cloudEvent.Type);
            var tokens = data.ProtoPayload.ResourceName.Split('/');
            var bucket = tokens[3];
            var name = tokens[5];
            _logger.LogInformation("Bucket: {bucket}", bucket);
            _logger.LogInformation("File: {file}", name);
            return Task.CompletedTask;
        }
    }
}
