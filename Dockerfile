# Use the latest ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the latest .NET SDK for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Install Node.js
# Note: Consider using a specific version of Node.js for better stability and compatibility
RUN curl -fsSL https://deb.nodesource.com/setup_18.x | bash - \
    && apt-get install -y \
        nodejs \
    && rm -rf /var/lib/apt/lists/*

# Set the working directory
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["TrailerDownloader/TrailerDownloader.csproj", "TrailerDownloader/"]
RUN dotnet restore "TrailerDownloader/TrailerDownloader.csproj"

# Copy the rest of the source code
COPY . .

# Set the working directory to the project directory
WORKDIR "/src/TrailerDownloader"

# Build the project
RUN dotnet build "TrailerDownloader.csproj" -c Release -o /app/build

# Publish the project
FROM build AS publish
RUN dotnet publish "TrailerDownloader.csproj" -c Release -o /app/publish --no-restore --no-build

# Final stage
FROM base AS final
WORKDIR /app

# Copy the published output from the publish stage
COPY --from=publish /app/publish .

# Set the entry point for the application
ENTRYPOINT ["dotnet", "TrailerDownloader.dll"]