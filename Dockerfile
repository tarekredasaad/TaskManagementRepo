FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["TaskManagement/TaskManagement.csproj", "TaskManagement/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["MyProject.Infrastructure/Infrastructure.csproj", "MyProject.Infrastructure/"]

RUN dotnet restore "TaskManagement/TaskManagement.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/TaskManagement"
RUN dotnet build "TaskManagement.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TaskManagement.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskManagement.dll"]
