# .NET on Google Cloud Functions

## Functions Framework

> **Note:** Functions Framework for .NET currently supports .NET Core 3.1. You
> need to change the target framework of generated projects to .NET 6.0.

[Functions Framework for
.NET](https://github.com/GoogleCloudPlatform/functions-framework-dotnet) is the
easiest way to deploy `HTTP` or `CloudEvent` consuming functions.

Install the template package into the .NET tooling:

```sh
dotnet new -i Google.Cloud.Functions.Templates
```

## HTTP Function

Create an HTTP function:

```sh
mkdir HelloHttp
cd HelloHttp
dotnet new gcf-http
```

Change the target framework to `net6.0` in the `.csproj` file:

```xml
<TargetFramework>net6.0</TargetFramework>
```

Deploy to Cloud Functions using `--trigger-http`:

```sh
gcloud functions deploy hello-http-function \
    --allow-unauthenticated \
    --entry-point HelloHttp.Function \
    --gen2 \
    --region us-central1 \
    --runtime dotnet \
    --trigger-http
```

Trigger the function:

```sh
gcloud functions call hello-http-function \
    --gen2 \
    --region us-central1
```

## CloudEvent Function - GCS

Create a Google Cloud Storage (GCS) bucket:

```sh
PROJECT_ID=$(gcloud config get-value core/project)
BUCKET_NAME="cloud-functions-bucket-${PROJECT_ID}"
gsutil mb -l us-central1 gs://${BUCKET_NAME}
```

Create a CloudEvent function listening for GCS events:

```sh
mkdir HelloGcs
cd HelloGcs
dotnet new gcf-event
```

Change the target framework to `net6.0` in the `.csproj` file:

```xml
<TargetFramework>net6.0</TargetFramework>
```

Deploy to Cloud Functions using `--trigger-event` and `trigger-resource`:

```sh
gcloud functions deploy hello-gcs-function \
    --allow-unauthenticated \
    --entry-point HelloGcs.Function \
    --gen2 \
    --region us-central1 \
    --runtime dotnet6 \
    --trigger-event google.storage.object.finalize \
    --trigger-resource ${BUCKET_NAME}
```

Trigger the function by uploading a file:

```sh
echo "Hello from Storage" > random.txt
gsutil cp random.txt gs://${BUCKET_NAME}
```

See the logs:

```sh
gcloud functions logs read hello-gcs-function \
    --gen2 \
    --region us-central1
```

## CloudEvent Function - PubSub

Create a PubSub topic:

```sh
TOPIC_NAME=cloud-functions-topic
gcloud pubsub topics create ${TOPIC_NAME}
```

Create a CloudEvent function listening for PubSub messages:

```sh
mkdir HelloPubSub
cd HelloPubSub
dotnet new gcf-event
```

Change the target framework to `net6.0` in the `.csproj` file:

```xml
<TargetFramework>net6.0</TargetFramework>
```

Change [Function.cs](HelloPubSub/Function.cs) to make sure the
type argument is `MessagePublishedData` to parse Pub/Sub messages.

Deploy to Cloud Functions using `--trigger-topic`:

```sh
gcloud functions deploy hello-pubsub-function \
    --allow-unauthenticated \
    --entry-point HelloPubSub.Function \
    --gen2 \
    --region us-central1 \
    --runtime dotnet6 \
    --trigger-topic ${TOPIC_NAME}
```

Trigger the function:

```sh
gcloud pubsub topics publish ${TOPIC_NAME} --message="Mete"
```

See the logs:

```sh
gcloud functions logs read hello-pubsub-function \
    --gen2 \
    --region us-central1
```

## CloudEvent Function - Untyped

Create a CloudEvent function with no type:

```sh
mkdir HelloUntyped
cd HelloUntyped
dotnet new gcf-untyped-event
```

This function is only parses `CloudEvent` without trying to parse the `data`.
Deploying and triggering it depends on the type of events you want to listen.

Change the target framework to `net6.0` in the `.csproj` file:

```xml
<TargetFramework>net6.0</TargetFramework>
```

Deploy to Cloud Functions using `--trigger-event` and `trigger-resource`:

```sh
gcloud functions deploy hello-untyped-function \
    --allow-unauthenticated \
    --entry-point HelloUntyped.Function \
    --gen2 \
    --region us-central1 \
    --runtime dotnet6 \
    --trigger-event google.storage.object.finalize \
    --trigger-resource ${BUCKET_NAME}
```

Trigger the function by uploading a file:

```sh
echo "Hello from Storage" > random.txt
gsutil cp random.txt gs://${BUCKET_NAME}
```

See the logs:

```sh
gcloud functions logs read hello-untyped-function \
    --gen2 \
    --region us-central1
```
