#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 443
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["src/Report/Report.Api/Report.Api.csproj", "src/Report/Report.Api/"]
COPY ["src/Report/Presentation/Presentation.csproj", "src/Report/Presentation/"]
COPY ["src/Report/Infrastructure/Infrastructure.csproj", "src/Report/Infrastructure/"]
COPY ["src/Report/Application/Application.csproj", "src/Report/Application/"]
COPY ["src/Common/Contracts/Contracts.csproj", "src/Common/Contracts/"]
COPY ["src/Report/Domain/Domain.csproj", "src/Report/Domain/"]
COPY ["src/Report/Persistence/Persistence.csproj", "src/Report/Persistence/"]
RUN dotnet restore "./src/Report/Report.Api/./Report.Api.csproj"
COPY . .
WORKDIR "/src/src/Report/Report.Api"
RUN dotnet build "./Report.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Report.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Report.Api.dll"]