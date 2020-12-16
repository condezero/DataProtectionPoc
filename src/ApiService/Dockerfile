FROM mcr.microsoft.com/dotnet/sdk:5.0.101-alpine3.12-amd64 AS build
WORKDIR /src
COPY ["*.csproj", "/app/ApiService/"]
WORKDIR "/app/ApiService"
RUN dotnet restore -r linux-x64

COPY /* ./
RUN dotnet publish -c release -o out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:5.0.1-alpine3.12-amd64
ENV ASPNETCORE_URLS http://+:5000
WORKDIR /app
COPY --from=build /app/ApiService/out .
ENTRYPOINT ["./ApiService"]