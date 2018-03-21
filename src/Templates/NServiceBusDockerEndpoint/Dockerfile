# Build runtime image
FROM microsoft/dotnet:2.0-runtime
WORKDIR /NServiceBusDockerEndpoint
COPY ./bin/Release/netcoreapp2.0/publish .
ENTRYPOINT ["dotnet", "NServiceBusDockerEndpoint.dll"]