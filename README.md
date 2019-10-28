# PanoramicSystems.DotNetTemplates
dotnet core templates

To install
```
dotnet new -i PanoramicSystems.Templates.WindowsService
dotnet new -i PanoramicSystems.Templates.ConsoleApp
```
To update use the same command and specify a version
```
dotnet new -i PanoramicSystems.Templates.WindowsService::1.0.5
```
or you may clear your nuget cache
```
dotnet nuget locals all --clear
```

Example of usage:
```
mkdir NewApp.NameSpace
cd NewApp.NameSpace
dotnet new windowsservice
```

# Compatibility

## PanoramicSystems.Templates.WindowsService

- v1.0.4+ - dotnet 3.x, C#8
- v1.0.3 - Last version on dotnet 2.x and C#7

## PanoramicSystems.Templates.ConsoleApp

- v1.0.3+ - dotnet 3.x, C#8
- v1.0.2 - Last version on dotnet 2.x and C#7
