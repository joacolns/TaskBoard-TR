# Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TaskBoardApi.csproj", "./"]
RUN dotnet restore "./TaskBoardApi.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "TaskBoardApi.csproj" -c Release -o /app/build
RUN dotnet publish "TaskBoardApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TaskBoardApi.dll"]
