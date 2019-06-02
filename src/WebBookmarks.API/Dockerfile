FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["src/WebBookmarks.API/WebBookmarks.API.csproj", "src/WebBookmarks.API/"]
COPY ["src/WebBookmarks.GrainInterfaces/WebBookmarks.GrainInterfaces.csproj", "src/WebBookmarks.GrainInterfaces/"]
COPY ["src/SharedKernel/SharedKernel.csproj", "src/SharedKernel/"]
COPY ["src/WebBookmarks.Domain/WebBookmarks.Domain.csproj", "src/WebBookmarks.Domain/"]
RUN dotnet restore "src/WebBookmarks.API/WebBookmarks.API.csproj"
COPY . .
WORKDIR "/src/src/WebBookmarks.API"
RUN dotnet build "WebBookmarks.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "WebBookmarks.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebBookmarks.API.dll"]