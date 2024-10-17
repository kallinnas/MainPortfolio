# Use the official .NET image as the base for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

# Set working directory inside the container
WORKDIR /app

# Copy the project files
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o /out

# Use the official .NET runtime image for production
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Set working directory inside the runtime container
WORKDIR /app

# Copy the built files from the previous stage
COPY --from=build-env /app/out ./

# Expose the port that your application runs on
EXPOSE 80

# Start the application
ENTRYPOINT ["dotnet", "MainPortfolio.dll"]
