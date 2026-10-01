# Architecture Decision Record

## Problem

The Compose file committed database and pgAdmin passwords, including the database password embedded in the API connection string. Developers need a repeatable local setup without committing their own credentials.

## Alternatives

| Option | Advantages | Disadvantages | Complexity | Security | Maintenance |
|---|---|---|---|---|---|
| A. Compose variables supplied from an ignored `.env` file or shell | Small change; works with existing container configuration and ASP.NET Core connection string; easy local setup | Values remain visible in container environment and resolved Compose configuration | Low | Keeps new passwords out of Git, but does not isolate them inside the container | Document two variables and maintain a placeholder example |
| B. Compose secrets mounted as files | Grants access per service and avoids putting the password directly in the container environment | Requires secret files and changes to how the API constructs its connection string; more setup for this demonstration | Medium | Better container-level handling of sensitive values | Maintain secret files, Compose mappings and application configuration |

## Decision

Use Compose variable interpolation with required `POSTGRES_PASSWORD` and `PGADMIN_PASSWORD` values. Keep developer-specific values in a Git-ignored `.env` file and commit `.env.example` with placeholders.

## Why?

The project is mostly a learning exercise with one API and one database. Option A solves the immediate problem in the repository while keeping the setup understandable and the implementation small. Option B is better if the application is deployed beyond local development or needs stricter separation of secrets.

## Consequences

- The API, PostgreSQL and pgAdmin receive consistent values from one local configuration file. Compose fails early if either variable is missing.
- A developer must create `.env` before running `docker compose up`; the README documents the step.
- Existing PostgreSQL volumes keep their previously initialized database password; changing `.env` alone does not rotate it.
- Environment values can still be exposed through local Docker access or configuration output. `.env` is not a production secret store.
- Replacing committed literals does not erase Git history. Credentials reused outside this demo need rotation.
