# Exercise #0

## Goal

Set up the lab environment.

## Expected Results

Laptop configured with Cloud Foundry CLI, Visual Studio Code, .NET Core SDK.

## Introduction

In this exercise, we'll set up our workstation and cloud environment so that we're ready to build and run modern .NET applications. Your instructor will provide the url and credentials for the Pivotal Cloud Foundry (PCF) instance you will be using.

## Package manager

We suggest using a package manager to install bootcamp software.  Alternatively use the latest release for your given operating system found at: <https://github.com/cloudfoundry/cli/releases>

- MacOS: Homebrew\
 ( brew search PACKAGE to search)

- Windows: Chocolatey\
 ( choco search PACKAGE to search)

- Debian-Based Linux: Apt\
 ( apt search PACKAGE to search)

- Fedora-Based Linux: Yum\
 ( yum search PACKAGE to search)

## Install Cloud Foundry CLI

You can interact with Cloud Foundry via Dashboard, REST API, or command line interface (CLI). Here, we install the CLI and ensure it's configured correctly.

- **Windows**:

    ```Windows
    choco install cloudfoundry-cli
    ```

- MacOS:

    ```command
    brew install cloudfoundry/tap/cf-cli
    ```

- Debian and Ubuntu:

    ```Linux
    wget -q -O https://packages.cloudfoundry.org/debian/cli.cloudfoundry.org.key
    sudo apt-key add echo "deb https://packages.cloudfoundry.org/debian stable main"
    sudo tee /etc/apt/sources.list.d sudo apt-get update sudo apt-get install cf-cli
    ```

- RHEL, CentOS, and Fedora:

    ```Linux
    sudo wget -O /etc/yum.repos.d/cloudfoundry-cli.repo https://packages.cloudfoundry.org/fedora/cloud`
    sudo yum install cf-cli
    ```

Confirm that it installed successfully by going to a command line, and typing:

```Windows
    cf -v
```

## Install .NET Core and Visual Studio Code

.NET Core represents a modern way to build .NET apps, and here we make sure we have everything needed to build ASP.NET Core apps.

***This workshop material has been developed, tested and confirmed to work with .NET Core 3.1.4, Visual Studio Code, Nuget and Powershell running on Windows 10. Other environment configurations (ie, Visual Studio 2015, 2017, 2019, Command Prompt, bash, Paket, Mac OS) are expected to and should work but have not been explicitly tested***

1. Visit <https://www.microsoft.com/net/download> to download and install the latest version of the .NET Core Runtime and SDK.

    ***.NET Core Version 3.1.x  is required for this workshop. For consistency we recommend every attendee have the 3.1.4 [release](https://dotnet.microsoft.com/download/dotnet-core/3.1).***

2. Confirm that the runtime is installed correctly by opening a command line and typing:

    ```Windows
    dotnet --version
    ```

3. Install Entity Framework Core Command Line Tools

   ```cmd
    dotnet tool install --global dotnet-ef
    ```

    *Note: If you get this error 'Tool "dotnet-ef' is already installed." run the following command to update the tool to the 3.1.4 version*  

    ```cmd
    dotnet tool update --global dotnet-ef --version 3.1.4
    ```

4. Install Visual Studio Code using your favorite package manager.

5. Open Visual Studio Code and go to `View → Extensions`

6. Search for C# and choose the top C# for Visual Studio Code option and click `Install`. This gives you type-ahead support for C#.

7. *(Optional) Search for 'cloudfoundry' and choose the entry for 'Cloudfoundry Manifest YML Support' and click `Install`. This extension is highly recommended and provides basic validation, content assist and hover infos for editing Cloud Foundry Manifest Files*

## Login into the Cloud Foundry CLI and target your Foundation

1. In the Terminal, type in `cf login -a api.sys.[uri] --skip-ssl-validation` and provide your credentials (these credentials will be provided during the workshop). Now you are connected to Pivotal Cloud Foundry.  Note the `-a` flag corresponds to the api endpoint of your instance of Pivotal Cloud Foundry.  Since this is a POC environment, certificates have not been installed and we include the `--skip-ssl-validation` flag

## Nuget Packages (Optional)

During this workshop we will create projects that access external libraries.  Our package manager of choice is nuget.  If you do not have access to [Official Nuget Server API](https://api.nuget.org/v3/index.json) please gather these libraries and store them locally or in your team's package manager (local Nuget server, Artifactory, etc).  The libraries we will consume are:

|Libraries|Version|
| ------------- |-------------|
|Pomelo.EntityFrameworkCore.MySql|3.1.1|
|Steeltoe.CloudFoundry.ConnectorCore|2.4.4|
|Steeltoe.CloudFoundry.Connector.EFCore|2.4.4|
|Microsoft.EntityFrameworkCore.Sqlite|3.1.4|
|Microsoft.EntityFrameworkCore.Design|3.1.4|
|NSwag.AspNetCore|13.6.2|
|Steeltoe.Extensions.Configuration.ConfigServerCore|2.4.4|
|Steeltoe.Discovery.ClientCore|2.4.4|
|package Steeltoe.Extensions.Configuration.CloudFoundryCore|2.4.4|
|Steeltoe.Discovery.ClientCore|2.4.4|
|Steeltoe.Common.Hosting|2.4.4|
|RabbitMQ.Client|5.2.0|
|Steeltoe.CircuitBreaker.Hystrix.MetricsStreamCore|2.4.4|
|Steeltoe.CircuitBreaker.HystrixCore|2.4.4|
