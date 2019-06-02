FROM mcr.microsoft.com/dotnet/core/runtime:2.2-stretch-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["src/WebBookmarks.Silo/WebBookmarks.Silo.csproj", "src/WebBookmarks.Silo/"]
COPY ["src/WebBookmarks.GrainInterfaces/WebBookmarks.GrainInterfaces.csproj", "src/WebBookmarks.GrainInterfaces/"]
COPY ["src/SharedKernel/SharedKernel.csproj", "src/SharedKernel/"]
COPY ["src/WebBookmarks.Domain/WebBookmarks.Domain.csproj", "src/WebBookmarks.Domain/"]
COPY ["src/WebBookmarks.Grains/WebBookmarks.Grains.csproj", "src/WebBookmarks.Grains/"]
RUN dotnet restore "src/WebBookmarks.Silo/WebBookmarks.Silo.csproj"
COPY . .
WORKDIR "/src/src/WebBookmarks.Silo"
RUN dotnet build "WebBookmarks.Silo.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "WebBookmarks.Silo.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebBookmarks.Silo.dll"]