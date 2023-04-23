FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["FrontEndServer/FrontEndServer.csproj", "FrontEndServer/"]
RUN dotnet restore "FrontEndServer/FrontEndServer.csproj"
COPY . .
WORKDIR "/src/FrontEndServer"
RUN dotnet build "FrontEndServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FrontEndServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FrontEndServer.dll"]
