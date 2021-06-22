# .NET Core on Google Cloud Functions

## Functions Framework

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

Change [Function.cs](HelloHttp/Function.cs) to use logger.

Deploy to Cloud Functions using `--trigger-http`:

```sh
gcloud functions deploy hello-http-function \
    --runtime dotnet3 \
    --trigger-http \
    --entry-point HelloHttp.Function \
    --allow-unauthenticated
```

Trigger the function:

```sh
gcloud functions call hello-http-function
```

## CloudEvent Function - GCS

Create a Google Cloud Storage (GCS) bucket:

```sh
GOOGLE_CLOUD_PROJECT=$(gcloud config get-value core/project)
BUCKET_NAME="cloud-functions-bucket-${GOOGLE_CLOUD_PROJECT}"
gsutil mb gs://${BUCKET_NAME}
```

Create a CloudEvent function listening for GCS events:

```sh
mkdir HelloGcs
cd HelloGcs
dotnet new gcf-event
```

Change [Function.cs](HelloGcs/Function.cs) to use logger.

Deploy to Cloud Functions using `--trigger-event` and `trigger-resource`:

```sh
gcloud functions deploy hello-gcs-function \
    --runtime dotnet3 \
    --entry-point HelloGcs.Function \
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
gcloud functions logs read hello-gcs-function
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

Change [Function.cs](HelloPubSub/Function.cs) to use logger. Also make sure the
type argument is `MessagePublishedData` to parse Pub/Sub messages.

Deploy to Cloud Functions using `--trigger-topic`:

```sh
gcloud functions deploy hello-pubsub-function \
    --runtime dotnet3 \
    --entry-point HelloPubSub.Function \
    --trigger-topic ${TOPIC_NAME} \
    --allow-unauthenticated
```

Trigger the function:

```sh
gcloud pubsub topics publish ${TOPIC_NAME} --message="Mete"
```

See the logs:

```sh
gcloud functions logs read hello-pubsub-function
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

Change [Function.cs](HelloUntyped/Function.cs) to use logger.

Deploy to Cloud Functions using `--trigger-event` and `trigger-resource`:

```sh
gcloud functions deploy hello-untyped-function \
    --runtime dotnet3 \
    --entry-point HelloUntyped.Function \
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
gcloud functions logs read hello-untyped-function
```

