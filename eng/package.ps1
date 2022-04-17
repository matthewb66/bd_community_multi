param(
  [string] $configuration = 'Debug',
  [string] $msbuild
)

$ErrorActionPreference = "Stop"
Write-Host "-msbuild: $msbuild"
Write-Host "MSBUILD_EXE: $env:MSBUILD_EXE"

$artifacts = Join-Path $PSScriptRoot ../artifacts
$logsDirectory = Join-Path $artifacts logs
$sln = Join-Path $PSScriptRoot ../Microsoft.Maui.Packages-net6.slnf
$slnMac = Join-Path $PSScriptRoot ../Microsoft.Maui.Packages-mac.slnf

# Bootstrap ./bin/dotnet/
$csproj = Join-Path $PSScriptRoot ../src/DotNet/DotNet.csproj
& dotnet build $csproj -bl:$logsDirectory/dotnet-$configuration.binlog

# Full path to dotnet folder
$dotnet = Join-Path $PSScriptRoot ../bin/dotnet/
$dotnet = (Get-Item $dotnet).FullName

if ($IsWindows)
{
    if (-not $msbuild)
    {
        $msbuild = $env:MSBUILD_EXE
    }

    if (-not $msbuild)
    {
        # If MSBuild path isn't specified, use the standard location of 'vswhere' to determine an appropriate MSBuild to use.
        # Learn more about VSWhere here: https://github.com/microsoft/vswhere/wiki/Find-MSBuild
        $msbuild = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -prerelease -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe

        if (-not $msbuild)
        {
            throw 'Could not locate MSBuild automatically. Set the $msbuild parameter of this script to provide a location.'
        }
        Write-Host "Found MSBuild at ${msbuild}"
    }

    # Modify global.json, so the IDE can load
    $globaljson = Join-Path $PSScriptRoot ../global.json
    [xml] $xml = Get-Content (Join-Path $PSScriptRoot Versions.props)
    $json = Get-Content $globaljson | ConvertFrom-Json
    $json | Add-Member sdk (New-Object -TypeName PSObject) -Force
    $json.sdk | Add-Member version ([string]$xml.Project.PropertyGroup.MicrosoftDotnetSdkInternalPackageVersion).Trim() -Force
    $json | ConvertTo-Json | Set-Content $globaljson

    # NOTE: I've not found a better way to do this
    # see: https://github.com/PowerShell/PowerShell/issues/3316
    $oldDOTNET_INSTALL_DIR=$env:DOTNET_INSTALL_DIR
    $oldDOTNET_ROOT=$env:DOTNET_ROOT
    $oldDOTNET_MSBUILD_SDK_RESOLVER_CLI_DIR=$env:DOTNET_MSBUILD_SDK_RESOLVER_CLI_DIR
    $oldDOTNET_MULTILEVEL_LOOKUP=$env:DOTNET_MULTILEVEL_LOOKUP
    $oldMSBuildEnableWorkloadResolver=$env:MSBuildEnableWorkloadResolver
    $oldPATH=$env:PATH
    try
    {
        $env:DOTNET_INSTALL_DIR=$dotnet

        # This tells .NET to use the bootstrapped runtime
        $env:DOTNET_ROOT=$dotnet

        # This tells MSBuild to load the SDK from the directory of the bootstrapped SDK
        $env:DOTNET_MSBUILD_SDK_RESOLVER_CLI_DIR=$env:DOTNET_ROOT

        # This tells .NET not to go looking for .NET in other places
        $env:DOTNET_MULTILEVEL_LOOKUP=0

        # This enables workload support inside the IDE
        $env:MSBuildEnableWorkloadResolver=$true

        # Put our local dotnet.exe on PATH first so Visual Studio knows which one to use
        $env:PATH=($dotnet + [IO.Path]::PathSeparator + $env:PATH)

        # Have to build the solution first so the xbf files are there for pack
        & $msbuild $sln `
            /p:configuration=$configuration `
            /p:SymbolPackageFormat=snupkg `
            /restore `
            /t:Build `
            /p:Packing=true `
            /bl:"$logsDirectory/maui-build-$configuration.binlog"
        if (!$?) { throw "Build .NET MAUI failed." }

        & $msbuild $sln `
            /p:configuration=$configuration `
            /p:SymbolPackageFormat=snupkg `
            /t:Pack `
            /p:Packing=true `
            /bl:"$logsDirectory/maui-pack-$configuration.binlog"
        if (!$?) { throw "Pack .NET MAUI failed." }
    }
    finally
    {
        $env:DOTNET_INSTALL_DIR = $oldDOTNET_INSTALL_DIR
        $env:DOTNET_ROOT=$oldDOTNET_ROOT
        $env:DOTNET_MSBUILD_SDK_RESOLVER_CLI_DIR=$oldDOTNET_MSBUILD_SDK_RESOLVER_CLI_DIR
        $env:DOTNET_MULTILEVEL_LOOKUP=$oldDOTNET_MULTILEVEL_LOOKUP
        $env:MSBuildEnableWorkloadResolver=$oldMSBuildEnableWorkloadResolver
        $env:PATH=$oldPATH
    }
}
else
{
    $oldPATH=$env:PATH
    try
    {
        # Put our local dotnet on $PATH
        $env:PATH=($dotnet + [IO.Path]::PathSeparator + $env:PATH)
        $dotnet_tool = Join-Path $dotnet dotnet

        # Build with ./bin/dotnet/dotnet
        & $dotnet_tool pack $slnMac `
            -c:$configuration `
            -p:SymbolPackageFormat=snupkg `
            -bl:$logsDirectory/maui-pack-$configuration.binlog
        if (!$?) { throw "Pack failed." }
    }
    finally
    {
        $env:PATH=$oldPATH
    }
}
