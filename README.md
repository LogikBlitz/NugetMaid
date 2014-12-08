NugetMaid
=========

Visual studio 2013 extension that aims to help making Nuget tasks easier.

## Features
The extension exposes a contextmenu on the Solutionlevel, called **NugetMaid**
From here you have access to Solution level commands.

### Handeling Nuget versions in packages.config
Nuget is covering a lot of scenarios already thru their own extensions, but some edge cases are not covered.
I have had scenarios where I needed to lock the versions of all project in a solution.
This is quite tedious so enter NugetMaid.

#### Lock Nuget Versions command
This menu item will find **all** packages.config files in a solution and then use the package versions for each package entry and append ``allowedVersions=[VERSION]`` to the package entry, thus locking the version.
Use this to lock your package versions to ensure that they cannot be updated by mistake.

#### Unlock Nuget Versions command
This is the opposite of the above. It will remove any ``allowedVersions=[VERSION]`` from all packages.config files found in the solution, thus allowing Nuget to roam free again...

### How to get NugetMaid

Your can get the latest **vsix** installer from the **releases** page which you can find here: https://github.com/LogikBlitz/NugetMaid/releases

Alternative you can build the solution manually.

#### Building the soultion
Open the solution in Visual Studio 2013. Please mark that the [Visual Studio 2013 SDK](https://visualstudiogallery.msdn.microsoft.com/842766ba-1f32-40cf-8617-39365ebfc134) is a requirement.
Simply build the solution in releasemode and get the **vsix** installer from the bin folder.

##### Debugging the solution.
To debug the solution you need to do make the following setup of the ``NugetMaid.Extensions`` project.

1. Select properties of the project.
1. Select the **debug** pane
1. Choose **Start external program** and set the path to you Visual Studio executable. On my machine it is: ``C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe``
1. In the **Start Options** set the **Command line arguments** to ``/rootsuffix Exp``. This will ensure that hitting **F5** will start an experimental instance of Visual Studio that allows you to safely debug the extension.

