// Copyright 2020 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using CloudNative.CloudEvents;
using Google.Cloud.Functions.Framework;
using Google.Events.SystemTextJson.Cloud.PubSub.V1;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace HelloCloudEventPubSubFunction
{
    /// <summary>
    /// A function that can be triggered in responses to changes in Google Pub/Sub.
    /// The type argument (MessagePublishedData in this case) determines how the event payload is deserialized.
    /// The event must be deployed so that the trigger matches the expected payload type. (For example,
    /// deploying a function expecting a MessagePublishedData payload will not work for a trigger that provides
    /// a FirestoreEvent.)
    /// </summary>
    public class Function : ICloudEventFunction<MessagePublishedData>
    {

        private readonly ILogger _logger;

        public Function(ILogger<Function> logger) =>
            _logger = logger;

        /// <summary>
        /// Logic for your function goes here. Note that a Cloud Event function just consumes an event;
        /// it doesn't provide any response.
        /// </summary>
        /// <param name="cloudEvent">The Cloud Event your function should consume.</param>
        /// <param name="data">The deserialized data within the Cloud Event.</param>
        /// <param name="cancellationToken">A cancellation token that is notified if the request is aborted.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task HandleAsync(CloudEvent cloudEvent, MessagePublishedData data, CancellationToken cancellationToken)
        {

            _logger.LogInformation("Message published data:");
            _logger.LogInformation($"  Message: {data.Message}");
            _logger.LogInformation($"  Subscription: {data.Subscription}");
            _logger.LogInformation($"  Message.MessageId: {data.Message.MessageId}");
            _logger.LogInformation($"  Message.TextData: {data.Message.TextData}");
            _logger.LogInformation("Cloud event information:");
            _logger.LogInformation($"  ID: {cloudEvent.Id}");
            _logger.LogInformation($"  Source: {cloudEvent.Source}");
            _logger.LogInformation($"  Type: {cloudEvent.Type}");
            _logger.LogInformation($"  Subject: {cloudEvent.Subject}");
            _logger.LogInformation($"  DataSchema: {cloudEvent.DataSchema}");
            _logger.LogInformation($"  DataContentType: {cloudEvent.DataContentType}");
            _logger.LogInformation($"  Time: {cloudEvent.Time?.ToUniversalTime():yyyy-MM-dd'T'HH:mm:ss.fff'Z'}");
            _logger.LogInformation($"  SpecVersion: {cloudEvent.SpecVersion}");

            // In this example, we don't need to perform any asynchronous operations, so the
            // method doesn't need to be declared async.
            return Task.CompletedTask;
        }
    }
}
