NugetMaid
=========

Visual studio 2013 extension that aims to help when doing tidious or complext Nuget tasks.

## Features
The extension exposes a contextmenu on the Solutionlevel, called **NugetMaid**
From here you have access to Solution level commands.

### Handeling Nuget versions in packages.config
The use case is:
If you checkout older code and you want to build it, nuget will automatically read the packages.config in all projects in the solution.

Nugets default behaviour is to download the highest available version for a given package, if the package does not already exist locally.

This means that it is difficult to checkout old code and build it as it was at that point in time, since the nuget packages can be installed with versions higher than the versions specified in the **packages.config**, as long as the package itself has an open end on versions. For more on Nuget versioning see: https://docs.nuget.org/docs/reference/versioning

NugetMaid can fix this by using the Nuget attribute **allowedVersions** that can force Nuget to uphold the versions in the packages.config.

Manually editing all the **packages.config** in a solution with multiple projects and many packages is very tidious, and here **NugetMaid** comes to your rescue :-)

#### Nuget Lock Versions command
This menu item will find **all** packages.config files in a solution and then use the package versions for each package entry and append ``allowedVersions=[VERSION]`` to the package entry, thus locking the version.
Use this to lock your package versions before building and restoring your packages.

#### Nuget Unlock Versions command
This is the opposite of the above. It will remove any ``allowedVersions=[VERSION]`` from all packages.config files found in the solution, thus allowing Nuget to roam free again...

