# .NET on Google Cloud Functions

## Functions Framework

[Functions Framework for
.NET](https://github.com/GoogleCloudPlatform/functions-framework-dotnet) is the
easiest way to deploy `HTTP` or `CloudEvent` consuming functions.

Install the template package into the .NET tooling:

```sh
dotnet new --install Google.Cloud.Functions.Templates::2.0.0-beta01
```

## Before you begin

Before you begin, enable required services:

```sh
gcloud services enable \
  artifactregistry.googleapis.com \
  cloudbuild.googleapis.com \
  cloudfunctions.googleapis.com \
  eventarc.googleapis.com \
  run.googleapis.com
```

Grant the `pubsub.publisher` role to the Cloud Storage service account. This is
needed for Eventarc's GCS trigger:

```sh
SERVICE_ACCOUNT="$(gsutil kms serviceaccount -p $PROJECT_ID)"

gcloud projects add-iam-policy-binding $PROJECT_ID \
    --member serviceAccount:$SERVICE_ACCOUNT \
    --role roles/pubsub.publisher
```

## HTTP Function

Create an HTTP function:

```sh
mkdir HelloHttp
cd HelloHttp
dotnet new gcf-http
```

Deploy to Cloud Functions using `--trigger-http`:

```sh
gcloud functions deploy hello-http-function \
    --allow-unauthenticated \
    --entry-point HelloHttp.Function \
    --gen2 \
    --region us-central1 \
    --runtime dotnet6 \
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
