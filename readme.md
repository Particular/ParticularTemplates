## Usage

The documentation for the `dotnet new` templates is located [here](https://github.com/dotnet/templating/wiki/%22Runnable-Project%22-Templates).

The templates are installed locally in

```
C:\Users\[USERNAME]\.templateengine\dotnetcli\
```

To use them, open the Developer Command Prompt for Visual Studio 2017.

NOTE: The current version of `dotnet new` command does not list the `--install` option in the help output.

## Templates

### NServiceBus Windows Service host

This template contains the code necessary to host an NServiceBus endpoint in a Windows Service. To install it use

```
dotnet new --install NServiceBus.Templates.WindowsService::*
```

This installs the template under `nsbservice` alias. To add a project containing the host use

```
dotnet new nsbservice --endpointname TheNewEndpoint
```

### ServiceControl Transport Adapter Windows Service host

This template contains the code necessary to host a ServiceControl Transport Adapter in a Windows Service. To install it use

```
dotnet new --install NServiceBus.Templates.TransportAdapter.WindowsService::*
```

This installs the template under `sc-adapter-service` alias. To add a project containing the host use

```
dotnet new sc-adapter-service --name MyAdapter
```
