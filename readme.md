[`dotnet new` templates](https://github.com/dotnet/templating/wiki/%22Runnable-Project%22-Templates) for NServiceBus.


## Documentation

 * [NServiceBus Endpoint Windows Service](https://docs.particular.net/nservicebus/dotnet-templates#nservicebus-endpoint-windows-service)
 * [ServiceControl Transport Adapter](https://docs.particular.net/nservicebus/dotnet-templates#servicecontrol-transport-adapter)


## Test locally

After building the project, the templates can be installed using the following:


### Re-Install

```
dotnet new -u ParticularTemplates
dotnet new -i <path to project>\nugets\ParticularTemplates.xxx.nupkg
```

### WindowsService

```
dotnet new nsbwinservice
```


### TransportAdapter

```
dotnet new scadapterwinservice
```

### Docker endpoint
```
dotnet new nsbdockerendpoint
```

## Install location


`%USERPROFILE%\.templateengine\dotnetcli\<SDK version>`
