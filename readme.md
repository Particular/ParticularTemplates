[`dotnet new` templates](https://github.com/dotnet/templating/wiki/%22Runnable-Project%22-Templates) for NServiceBus.


## Docuemntation

 * [Windows Service endpoint host](https://docs.particular.net/nservicebus/hosting/windows-service-template)
 * [ServiceControl Transport Adapter host](https://docs.particular.net/servicecontrol/transport-adapter/template)


## Test locally

After build templates can be installed using the following:


### WindowsService

```
dotnet new -u NServiceBus.Template.WindowsService
dotnet new -i C:\Code\Particular\NServiceBus.Templates\nugets\NServiceBus.Template.WindowsService.xxx.nupkg
dotnet new nsbservice
```


### TransportAdapter

```
dotnet new -u NServiceBus.Template.TransportAdapter.WindowsService
dotnet new -i C:\Code\Particular\NServiceBus.Templates\nugets\NServiceBus.Template.TransportAdapter.WindowsService.xxx.nupkg
dotnet new sc-adapter-service
```

## Install location


`%USERPROFILE%\.templateengine\dotnetcli\v2.0.0`