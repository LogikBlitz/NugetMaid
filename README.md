NugetMaid
=========

Visual studio 2013 extension that aims to help making Nuget tasks easier.

## Features
The extension exposes a contextmenu on the Solutionlevel, called **NugetMaid**
From here you have access to Solution level commands.

### Handeling Nuget versions in packages.config
Nuget is covering a lot of scenarios already thru their own extensions, but some edge cases are not covered.
I have has scenarios where I needed to lock the versions of all project in a solution.
This is quite tedious so enter NugetMaid.

#### Lock Nuget Versions command
This menu item will find **all** packages.config files in a solution and then use the package versions for each package entry and append ``allowedVersions=[VERSION]`` to the package entry, thus locking the version.
Use this to lock your package versions to ensure that they can not be updated by mistake.

#### Unlock Nuget Versions command
This is the opposite of the above. It will remove any ``allowedVersions=[VERSION]`` from all packages.config files found in the solution, thus allowing Nuget to roam free again...

