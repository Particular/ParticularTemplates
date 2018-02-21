# Build runtime image
FROM microsoft/dotnet:2.0-runtime
WORKDIR /AnEndpointName
COPY ./bin/Debug/netcoreapp2.0/publish .
ENTRYPOINT ["dotnet", "NsbDockerEndpoint.dll"]