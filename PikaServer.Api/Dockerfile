#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PikaServer.Api/PikaServer.Api.csproj", "PikaServer.Api/"]
COPY ["PikaServer.Domain/PikaServer.Domain.csproj", "PikaServer.Domain/"]
COPY ["PikaServer.Infras/PikaServer.Infras.csproj", "PikaServer.Infras/"]
COPY ["PikaServer.Common/PikaServer.Common.csproj", "PikaServer.Common/"]
COPY ["PikaServer.Persistence/PikaServer.Persistence.csproj", "PikaServer.Persistence/"]
RUN dotnet restore "PikaServer.Api/PikaServer.Api.csproj"
COPY . .
WORKDIR "/src/PikaServer.Api"
RUN dotnet build "PikaServer.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PikaServer.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PikaServer.Api.dll"]