#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Logs.WebHost/Logs.WebHost.csproj", "Logs.WebHost/"]
COPY ["Logs.Core/Logs.Core.csproj", "Logs.Core/"]
COPY ["Logs.DataAccess/Logs.DataAccess.csproj", "Logs.DataAccess/"]
RUN dotnet restore "Logs.WebHost/Logs.WebHost.csproj"
COPY . .
WORKDIR "/src/Logs.WebHost"
RUN dotnet build "Logs.WebHost.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Logs.WebHost.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Logs.WebHost.dll"]