# # Use the .NET 6.0 SDK Docker image as the base image
# FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
# WORKDIR /app

# # Copy csproj and restore dependencies
# # COPY ./Repositories/*.csproj ./
# # COPY ./Services/*.csproj ./
# COPY ./WebAPI/*.csproj ./
# RUN dotnet restore

# # Copy everything else and build
# COPY . ./
# RUN dotnet publish -c Release -o out

# # Use the .NET 6.0 runtime Docker image as the base image
# FROM mcr.microsoft.com/dotnet/aspnet:6.0
# WORKDIR /app
# COPY --from=build-env /app/out .

# # Expose port 80 for the application
# EXPOSE 7296
# ENTRYPOINT ["dotnet", "WebAPI.dll"]


FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /source

# Copy everything
COPY . .
RUN dotnet restore "./WebAPI/WebAPI.csproj" --disable-parallel
RUN dotnet build "./WebAPI/WebAPI.csproj" -c release -o /app --no-restore

RUN dotnet restore "./Repositories/Repositories.csproj" --disable-parallel
RUN dotnet build "./Repositories/Repositories.csproj" -c release -o /app --no-restore

RUN dotnet restore "./Services/Services.csproj" --disable-parallel
RUN dotnet build "./Services/Services.csproj" -c release -o /app --no-restore

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal 
WORKDIR /app
COPY --from=build /app ./
EXPOSE 7296
ENTRYPOINT ["dotnet", "WebAPI.dll"]