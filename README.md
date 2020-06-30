# .NET Core on Google Cloud Functions

> **NOTE:**
> The `dotnet3` runtime is currently available on an "allow
> list" basis. Please fill in [this
> form](https://docs.google.com/forms/d/e/1FAIpQLSe7qB5vNrgFtZZ3ZUfIwkbsDMGsA1fXY52GzmGmnhwdReHuOQ/viewform)
> to register your interest in being part of the public alpha.

## Functions Framework

[Functions Framework for
.NET](https://github.com/GoogleCloudPlatform/functions-framework-dotnet) is the
easiest way to deploy `HTTP` or `CloudEvent` consuming functions.

Install the template package into the .NET tooling:

```sh
dotnet new -i Google.Cloud.Functions.Templates::1.0.0-alpha07
```

## HTTP Function

Create an HTTP function:

```sh
mkdir HelloHttpFunction
cd HelloHttpFunction
dotnet new gcf-http
```

Deploy to Cloud Functions using `--trigger-http`:

```sh
gcloud functions deploy hello-http-function \
    --runtime dotnet3 \
    --trigger-http \
    --entry-point HelloHttpFunction.Function
```

Trigger the function:

```sh
gcloud functions call hello-http-function
```

## CloudEvent Function - Storage

Create a Cloud Storage bucket:

```sh
export BUCKET_NAME="cloud-functions-bucket-$(gcloud config get-value core/project)"
gsutil mb gs://${BUCKET_NAME}
```

Create a CloudEvent function listening for Storage events:

```sh
mkdir HelloCloudEventStorageFunction
cd HelloCloudEventStorageFunction
dotnet new gcf-event
```

Deploy to Cloud Functions using `--trigger-event` and `trigger-resource`:

```sh
gcloud functions deploy hello-cloudevent-storage-function \
    --runtime dotnet3 \
    --entry-point HelloCloudEventStorageFunction.Function \
    --trigger-event google.storage.object.finalize \
    --trigger-resource ${BUCKET_NAME} \
    --allow-unauthenticated
```

Trigger the function by uploading a file:

```sh
echo "Hello from Storage" > random.txt
gsutil cp random.txt gs://${BUCKET_NAME}
```

See the logs:

```sh
gcloud functions logs read
```

## CloudEvent Function - PubSub

Create a PubSub topic:

```sh
export TOPIC_NAME=cloud-functions-topic
gcloud pubsub topics create ${TOPIC_NAME}
```

Create a CloudEvent function listening for PubSub messages:

```sh
mkdir HelloCloudEventPubSubFunction
cd HelloCloudEventPubSubFunction
dotnet new gcf-untyped-event
```

Deploy to Cloud Functions using `--trigger-topic`:

```sh
gcloud functions deploy hello-cloudevent-pubsub-function \
    --runtime dotnet3 \
    --entry-point HelloCloudEventPubSubFunction.Function \
    --trigger-topic ${TOPIC_NAME} \
    --allow-unauthenticated
```

Trigger the function:

```sh
gcloud pubsub topics publish ${TOPIC_NAME} --message="Hello World"
```

See the logs:

```sh
gcloud functions logs read
```
