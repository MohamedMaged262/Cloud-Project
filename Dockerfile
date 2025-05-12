# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the .csproj file and restore dependencies
COPY ZA-PLACE.csproj ./
RUN dotnet restore

# Copy the rest of the application and publish
COPY . ./
RUN dotnet publish -c Release -o out

# Stage 2: Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy the published output
COPY --from=build /app/out ./

# Set environment variables and expose port
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

# Set the entry point
ENTRYPOINT ["dotnet", "ZA-PLACE.dll"]
