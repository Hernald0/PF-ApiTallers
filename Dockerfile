FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["UTNApiTalleres/UTNApiTalleres.csproj", "UTNApiTalleres/"]
RUN dotnet restore "UTNApiTalleres/UTNApiTalleres.csproj"
COPY . .
WORKDIR "/src/UTNApiTalleres"
RUN dotnet build "UTNApiTalleres.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UTNApiTalleres.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet UTNApiTalleres.dll
