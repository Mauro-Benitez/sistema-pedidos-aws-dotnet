FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src


COPY ["OrderService.csproj", "."]
RUN dotnet restore "./OrderService.csproj"


COPY . .
WORKDIR "/src/."
RUN dotnet build "./OrderService.csproj" -c $BUILD_CONFIGURATION -o /app/build


FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./OrderService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .


EXPOSE 8080

ENTRYPOINT ["dotnet", "OrderService.dll"]