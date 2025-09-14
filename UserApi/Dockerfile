FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY global.json ./

COPY UserApi/UserApi.csproj UserApi/
COPY UserApplication/UserApplication.csproj UserApplication/
COPY UserDomain/UserDomain.csproj UserDomain/
COPY UserInfrastructure/UserInfrastructure.csproj UserInfrastructure/

RUN dotnet restore UserApi/UserApi.csproj

COPY . .
RUN dotnet publish UserApi/UserApi.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development
COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "UserApi.dll"]
