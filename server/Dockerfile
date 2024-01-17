# Use Microsoft's official build .NET image.
# https://hub.docker.com/_/microsoft-dotnet-sdk/
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory
WORKDIR /app

# Copy csproj and restore
COPY *.csproj ./
RUN dotnet restore "mail.csproj"

# Copy everything else and build the project
COPY . ./
RUN dotnet publish "mail.csproj" -c Release -o out

# Use Microsoft's official runtime .NET image.
# https://hub.docker.com/_/microsoft-dotnet-aspnet/
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app/server
COPY --from=build /app/out ./

# Make sure the app binds to port 80
ENV ASPNETCORE_URLS http://*:80

# Start the app
ENTRYPOINT ["dotnet", "mail.dll"]