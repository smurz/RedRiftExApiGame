FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ExApiGame/ExApiGame.csproj", "ExApiGame/"]
RUN dotnet restore "ExApiGame/ExApiGame.csproj"
COPY . .
WORKDIR "/src/ExApiGame"
RUN dotnet build "ExApiGame.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ExApiGame.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet NETToDoDemo.dll