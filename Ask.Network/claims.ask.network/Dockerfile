FROM mcr.microsoft.com/dotnet/sdk:8.0.100-bookworm-slim-amd64 AS build
WORKDIR /solution

# Copy full build context. NuGet restore is cached via RUN mount
COPY . .

RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages dotnet tool restore
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages dotnet restore "claims.ask.network.csproj"

# Build and publish
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages dotnet publish "claims.ask.network.csproj" --no-restore -c Release -o /app/publish

# Create a final image with the runtime only
FROM mcr.microsoft.com/dotnet/aspnet:8.0.0-bookworm-slim-amd64 AS final

WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "claims.ask.network.dll"]
