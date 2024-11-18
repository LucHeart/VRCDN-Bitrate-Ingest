FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

COPY ["VRCDN-Bitrate-Ingest/VRCDN-Bitrate-Ingest.csproj", "VRCDN-Bitrate-Ingest/"]
RUN dotnet restore "VRCDN-Bitrate-Ingest/VRCDN-Bitrate-Ingest.csproj"

COPY . .
WORKDIR "/src/VRCDN-Bitrate-Ingest"


RUN dotnet publish "VRCDN-Bitrate-Ingest.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:9.0-alpine
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LucHeart.VRCDN.BI.dll"]