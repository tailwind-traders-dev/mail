FROM node:20-bookworm-slim AS node_base
FROM mcr.microsoft.com/dotnet/sdk:7.0-bookworm-slim AS runtime
# upgrade to: mcr.microsoft.com/dotnet/aspnet:8.0.0-bookworm-slim-amd64
COPY --from=node_base . .
WORKDIR /app
COPY . .
CMD ["node", "run", "dev"]
