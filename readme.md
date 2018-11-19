[`dotnet new` templates](https://github.com/dotnet/templating/wiki/%22Runnable-Project%22-Templates) for NServiceBus.


## Documentation

 * [NServiceBus Windows Service](https://docs.particular.net/nservicebus/dotnet-templates#nservicebus-windows-service)
 * [ServiceControl Transport Adapter Windows Service](https://docs.particular.net/nservicebus/dotnet-templates#servicecontrol-transport-adapter-windows-service)
 * [NServiceBus Docker Container](https://docs.particular.net/nservicebus/dotnet-templates#nservicebus-docker-container)


## Testing locally

After building the project, the templates can be installed using the following:

```
dotnet new -u ParticularTemplates
dotnet new -i <path to project>\nugets\ParticularTemplates.<version>.nupkg
```
