FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Install Node.js and ffmpeg
# Note: Consider using a specific version of Node.js for better stability and compatibility
RUN curl -fsSL https://deb.nodesource.com/setup_18.x | bash - \
    && apt-get install -y \
        nodejs \
        ffmpeg \
    && rm -rf /var/lib/apt/lists/*

ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TrailerDownloader/TrailerDownloader.csproj", "TrailerDownloader/"]
RUN dotnet restore "TrailerDownloader/TrailerDownloader.csproj"
COPY . .
WORKDIR "/src/TrailerDownloader"
RUN dotnet build "TrailerDownloader.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TrailerDownloader.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM base AS final
WORKDIR /app
COPY --from=publish /app/build .
ENTRYPOINT ["dotnet", "TrailerDownloader.dll"]
