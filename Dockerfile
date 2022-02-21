FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["HerokuApiDemo/HerokuApiDemo.csproj", "HerokuApiDemo/"]
RUN dotnet restore "HerokuApiDemo/HerokuApiDemo.csproj"
COPY . .
WORKDIR "/src/HerokuApiDemo"
RUN dotnet build "HerokuApiDemo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HerokuApiDemo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet HerokuApiDemo.dll
