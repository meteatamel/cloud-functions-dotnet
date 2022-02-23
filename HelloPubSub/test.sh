set -v

curl -X POST \
    -H "content-type: application/json"  \
    -d @pubsub_text.json \
  http://localhost:8080 -v