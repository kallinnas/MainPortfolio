# Use official .NET SDK for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy everything
COPY . ./

# Restore dependencies
RUN dotnet restore

# Build the application
RUN dotnet publish -c Release -o out --no-restore

# Use official .NET runtime for running the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the published files from the build step
COPY --from=build /app/out ./

# Expose port
EXPOSE 80

# Run the application
ENTRYPOINT ["dotnet", "MainPortfolio.dll"]
