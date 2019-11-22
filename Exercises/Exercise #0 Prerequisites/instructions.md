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

***This workshop material has been developed, tested and confirmed to work with .NET Core 2.2.7, Visual Studio Code, Nuget and Powershell running on Windows 10. Other environment configurations (ie, Visual Studio 2015, 2017, 2019, Command Prompt, bash, Paket, Mac OS) are expected to and should work but have not been explicitly tested***

1. Visit <https://www.microsoft.com/net/download> to download and install the latest version of the .NET Core Runtime and SDK.

    ***.NET Core Version 2.2.x  is required for this workshop. For consistency we recommend every attendee have the 2.2.7 [release](https://dotnet.microsoft.com/download/dotnet-core/thank-you/sdk-2.2.402-windows-x64-installer).***

2. Confirm that the runtime is installed correctly by opening a command line and typing:

    ```Windows
    dotnet --version
    ```

3. Install Visual Studio Code using your favorite package manager.

4. Open Visual Studio Code and go to `View → Extensions`

5. Search for C# and choose the top C# for Visual Studio Code option and click `Install`. This gives you type-ahead support for C#.

6. *(Optional) Search for 'cloudfoundry' and choose the entry for 'Cloudfoundry Manifest YML Support' and click `Install`. This extension is highly recommended and provides basic validation, content assist and hover infos for editing Cloud Foundry Manifest Files*

## Login into the Cloud Foundry CLI and target your Foundation

1. In the Terminal, type in `cf login -a api.sys.[uri] --skip-ssl-validation` and provide your credentials (these credentials will be provided during the workshop). Now you are connected to Pivotal Cloud Foundry.  Note the `-a` flag corresponds to the api endpoint of your instance of Pivotal Cloud Foundry.  Since this is a POC environment, certificates have not been installed and we include the `--skip-ssl-validation` flag
