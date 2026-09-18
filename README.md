# Fortuity

A deliberately undocumented .NET solution for practising how to get into an unfamiliar codebase with an AI coding agent.

The exercise is described in [harjoitus.md](harjoitus.md) (in Finnish). In short: find out what this application is, get it running, and document it, using an AI agent rather than reading the code yourself.

## Running

Docker is the only requirement.

```bash
docker compose up
```

- Web UI: http://localhost:5044
- API: http://localhost:5015 (API reference at http://localhost:5015/scalar)

Stop with `docker compose down`. Add `-v` to also reset the database.

## Notes

- All data is fictional. The application does not represent any real organisation.
- The lack of further documentation is intentional. Producing it is the exercise.
