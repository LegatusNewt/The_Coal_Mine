# Use the official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
RUN apt-get update && apt-get install -y curl

WORKDIR /

# Copy the global.json file to ensure the correct SDK version is used
COPY global_docker.json ./global.json

# Copy the project files and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the remaining files and build the application
COPY . ./
RUN dotnet publish -c Release -o out

# Use the official .NET runtime image to run the application
FROM mcr.microsoft.com/dotnet/runtime:8.0.11-noble AS runtime
WORKDIR /
COPY --from=build /app/out .

# Set the entry point for the container
ENTRYPOINT ["dotnet", "CoalMineApi.dll"]