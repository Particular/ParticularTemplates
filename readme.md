https://github.com/dotnet/templating/wiki/%22Runnable-Project%22-Templates

## Local Development

### Install location

```
C:\Users\[USERNAME]\.templateengine\dotnetcli\
```

### Testing

Open a Visual Studio 2017 command prompt.

#### Installation

```
dotnet new --debug:reinit
dotnet new --install C:\Code\Particular\NServiceBus.Templates\src\WindowsService\Template
```

#### Running

New project

```
dotnet new nsbservice --endpointname TheNewEndpoint
```