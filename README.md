# VRCDN Bitrate Ingest

This is a small service designed to move VRCDN's bitrate ingestion data into a prometheus compatible database.

## Usage

`docker-compose.yml`
```yaml
services:
  vrcdn-bitrate-ingest:
    image: ghcr.io/lucheart/vrcdn-bitrate-ingest
    volumes:
     - ./config.json:/app/config.json
```

`config.json`
```json
{
  "streams": {
    "stream name go here": "VRCDN STREAM KEY HERE",
  },
  "prometheus": {
    "endpoint": "https://victoria.example.org/api/v1/import/prometheus",
    "headers": {
      "Authorization": "Basic bm90aGluZzpoZXJl"
    }
  }
}
```
