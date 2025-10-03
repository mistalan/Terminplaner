# 🐳 Docker Setup for Terminplaner API

This document explains how to run the Terminplaner API using Docker.

## Quick Start

### Using Docker Compose (Recommended)

The easiest way to run the API:

```bash
docker-compose up
```

The API will be available at **http://localhost:5215**

To run in detached mode:
```bash
docker-compose up -d
```

To stop:
```bash
docker-compose down
```

### Using Docker directly

Build the image:
```bash
docker build -t terminplaner-api .
```

Run the container:
```bash
docker run -p 5215:5215 terminplaner-api
```

## Docker Image

The Dockerfile uses a multi-stage build:

1. **Build stage**: Compiles the .NET application and runs tests
2. **Runtime stage**: Creates a minimal production image with only the compiled app

### Image Features

- ✅ **Multi-stage build** - Optimized image size
- ✅ **Tests included** - Tests run during build to ensure quality
- ✅ **Non-root user** - Runs as unprivileged user for security
- ✅ **Health check** - Built-in health monitoring
- ✅ **Production-ready** - Uses ASP.NET Core production runtime

## GitHub Container Registry

Docker images are automatically built and pushed to GitHub Container Registry on every push to main.

Pull the latest image:
```bash
docker pull ghcr.io/mistalan/terminplaner/api:latest
```

Run the image from GitHub Container Registry:
```bash
docker run -p 5215:5215 ghcr.io/mistalan/terminplaner/api:latest
```

## Environment Variables

Configure the container using environment variables:

```bash
docker run -p 5215:5215 \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -e ASPNETCORE_URLS=http://+:5215 \
  terminplaner-api
```

Available variables:
- `ASPNETCORE_ENVIRONMENT` - Environment (Development/Production)
- `ASPNETCORE_URLS` - URL binding (default: http://+:5215)

## Volumes

Currently, the API uses in-memory storage. When persistent storage is added, you can mount volumes:

```bash
docker run -p 5215:5215 \
  -v $(pwd)/data:/app/data \
  terminplaner-api
```

## Health Check

The container includes a built-in health check that queries the API every 30 seconds:

Check health status:
```bash
docker inspect --format='{{.State.Health.Status}}' <container-id>
```

## Building for Production

Build optimized production image:
```bash
docker build -t terminplaner-api:prod --target runtime .
```

## Troubleshooting

**Container won't start:**
- Check logs: `docker logs <container-id>`
- Verify port 5215 is not already in use
- Ensure .NET 9.0 runtime is correctly installed in image

**Tests fail during build:**
- Review test output in build logs
- Run tests locally first: `dotnet test`
- Fix failing tests before building image

**Cannot connect to API:**
- Verify port mapping: `-p 5215:5215`
- Check firewall settings
- Ensure API is listening on all interfaces: `ASPNETCORE_URLS=http://+:5215`

## Security Considerations

- ✅ Container runs as non-root user (appuser)
- ✅ Minimal attack surface (runtime image only)
- ✅ Regular security scans with Trivy
- ⚠️ Use HTTPS in production (configure reverse proxy)
- ⚠️ Set secure environment variables
- ⚠️ Keep base images updated

## Next Steps

- [ ] Add HTTPS support with certificates
- [ ] Configure persistent storage
- [ ] Add database container (when DB support is added)
- [ ] Set up container orchestration (Kubernetes/Docker Swarm)
- [ ] Configure logging and monitoring

## Resources

- [Docker Documentation](https://docs.docker.com/)
- [ASP.NET Core on Docker](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/docker/)
- [GitHub Container Registry](https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-container-registry)
