set -v

curl -X POST \
    -H "content-type: application/json"  \
    -d @storage.json \
  http://localhost:8080 -v