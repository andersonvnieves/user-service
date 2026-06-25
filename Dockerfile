FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["br.com.fiap.cloudgames.WebAPI/br.com.fiap.cloudgames.WebAPI.csproj", "br.com.fiap.cloudgames.WebAPI/"]
RUN dotnet restore "br.com.fiap.cloudgames.WebAPI/br.com.fiap.cloudgames.WebAPI.csproj"
COPY . .
WORKDIR "/src/br.com.fiap.cloudgames.WebAPI"
RUN dotnet build "./br.com.fiap.cloudgames.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./br.com.fiap.cloudgames.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "br.com.fiap.cloudgames.WebAPI.dll"]
