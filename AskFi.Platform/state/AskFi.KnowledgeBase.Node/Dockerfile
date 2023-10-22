# syntax=docker/dockerfile:experimental
# This assumes the build context is the solution root folder.
FROM mcr.microsoft.com/dotnet/sdk:7.0.203-bullseye-slim-amd64 AS build
ARG CommitHash=""
WORKDIR /solution

# Copy full build context. NuGet restore is cached via RUN mount
COPY . .

RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages dotnet tool restore
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages dotnet restore "src/hosting/Rabot.ObservationPool.Node/Rabot.ObservationPool.Node.csproj"

# Build and publish
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages dotnet publish "src/hosting/Rabot.ObservationPool.Node/Rabot.ObservationPool.Node.csproj" --no-restore -c Release -o /app/publish "/p:CommitHash=$CommitHash"

# Create a final image with the runtime only
FROM mcr.microsoft.com/dotnet/aspnet:7.0.5-bullseye-slim-amd64 AS final

WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Rabot.ObservationPool.Node.dll"]
