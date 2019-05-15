FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-stretch AS build
WORKDIR /src
COPY ["src/WebBookmarks.WebUI/WebBookmarks.WebUI.csproj", "src/WebBookmarks.WebUI/"]
RUN dotnet restore "src/WebBookmarks.WebUI/WebBookmarks.WebUI.csproj"
COPY . .
WORKDIR "/src/src/WebBookmarks.WebUI"
RUN dotnet build "WebBookmarks.WebUI.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "WebBookmarks.WebUI.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebBookmarks.WebUI.dll"]