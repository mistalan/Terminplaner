# Dockerfile for Terminplaner API
# Multi-stage build for optimized production image

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["TerminplanerApi/TerminplanerApi.csproj", "TerminplanerApi/"]
COPY ["TerminplanerApi.Tests/TerminplanerApi.Tests.csproj", "TerminplanerApi.Tests/"]

# Restore dependencies
RUN dotnet restore "TerminplanerApi/TerminplanerApi.csproj"

# Copy remaining source code
COPY TerminplanerApi/ TerminplanerApi/
COPY TerminplanerApi.Tests/ TerminplanerApi.Tests/

# Build and test
WORKDIR /src/TerminplanerApi
RUN dotnet build "TerminplanerApi.csproj" -c Release -o /app/build

# Run tests
WORKDIR /src
RUN dotnet test "TerminplanerApi.Tests/TerminplanerApi.Tests.csproj" -c Release --no-restore

# Publish stage
WORKDIR /src/TerminplanerApi
RUN dotnet publish "TerminplanerApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Create non-root user for security
RUN useradd -m -u 1000 appuser && chown -R appuser:appuser /app
USER appuser

# Copy published app from build stage
COPY --from=build /app/publish .

# Expose port
EXPOSE 5215

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:5215/api/appointments || exit 1

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5215
ENV ASPNETCORE_ENVIRONMENT=Production

# Run the application
ENTRYPOINT ["dotnet", "TerminplanerApi.dll"]
