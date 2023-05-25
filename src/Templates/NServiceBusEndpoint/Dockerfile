FROM mcr.microsoft.com/dotnet/core/runtime:6.0 AS base

FROM mcr.microsoft.com/dotnet/core/sdk:6.0 AS build
WORKDIR /src
COPY . .
WORKDIR /src/ProjectName
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "ProjectName.dll"]