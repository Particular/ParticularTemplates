[`dotnet new` templates](https://github.com/dotnet/templating/wiki/%22Runnable-Project%22-Templates) for NServiceBus.

## Docuemntation

 * [Windows Service endpoint host](https://docs.particular.net/nservicebus/hosting/windows-service-template)
 * [ServiceControl Transport Adapter host](https://docs.particular.net/servicecontrol/transport-adapter/template)


## Test locally

After build templates can be installed using the following:

```
dotnet new -i C:\Code\Particular\NServiceBus.Templates\src\binaries\WindowsService
dotnet new -i C:\Code\Particular\NServiceBus.Templates\src\binaries\TransportAdapter.WindowsService
```