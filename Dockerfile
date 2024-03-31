# Use the latest ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

# Use the latest .NET SDK for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["TrailerDownloader/TrailerDownloader.csproj", "TrailerDownloader/"]
RUN dotnet restore "TrailerDownloader/TrailerDownloader.csproj"

# Copy the rest of the source code
COPY . .

# Set the working directory to the project directory
WORKDIR "/src/TrailerDownloader"

# Build the project
RUN dotnet build "TrailerDownloader.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the project
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TrailerDownloader.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app

# Copy the published output from the publish stage
COPY --from=publish /app/publish .

# Set the entry point for the application
ENTRYPOINT ["dotnet", "TrailerDownloader.dll"]