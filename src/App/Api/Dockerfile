#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["src/App/Api/Api.csproj", "src/App/Api/"]
COPY ["src/App/Presentation/Presentation.csproj", "src/App/Presentation/"]
COPY ["src/App/Infrastructure/Infrastructure.csproj", "src/App/Infrastructure/"]
COPY ["src/App/Application/Application.csproj", "src/App/Application/"]
COPY ["src/Common/Contracts/Contracts.csproj", "src/Common/Contracts/"]
COPY ["src/App/Domain/Domain.csproj", "src/App/Domain/"]
COPY ["src/App/Persistence/Persistence.csproj", "src/App/Persistence/"]
RUN dotnet restore "./src/App/Api/./Api.csproj"
COPY . .
WORKDIR "/src/src/App/Api"
RUN dotnet build "./Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]