# Use official .NET SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory inside the container
WORKDIR /app

# Copy everything to the container
COPY . . 

# Restore dependencies
RUN dotnet restore

# Build the app in release mode
RUN dotnet publish -c Release -o /app/out --no-restore

# Use official .NET runtime for running the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Set the working directory in runtime container
WORKDIR /app

# Copy published app from build stage
COPY --from=build /app/out . 

# Expose port 80 for HTTP traffic
EXPOSE 80

# Start the application
ENTRYPOINT ["dotnet", "MainPortfolio.dll"]
