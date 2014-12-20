// Guids.cs
// MUST match guids.h

using System;

namespace LogikBlitz.NugetMaid
{
    internal static class GuidList
    {
        public const string guidVS_ExtensionsPkgString = "0e707c91-7054-48b7-9612-6e7d040054e4";

        #region Nuget Locking and unlocking packages

        public const string NugetMaidMenuGroupingGuid = "38f3e154-a747-41a7-960c-7c3d4ef5ac29";
        public const string GuidNugetLockVersionCommandString = "68ce72a7-a177-4818-b1e9-4fa9ffc5c7f8";
        public const string GuidNugetUnlockVersionCommandString = "260978c3-582c-487d-ab12-c1fdde07c555";
        public static readonly Guid GuidNugetLockVersionCommand = new Guid(GuidNugetLockVersionCommandString);
        public static readonly Guid GuidNugetUnlockVersionCommand = new Guid(GuidNugetUnlockVersionCommandString);

        #endregion

        #region Nuget Configfile command

        public const string GuidNugetAddConfigFileToSolutionCommandString = "38ea8a9b-b505-40d0-a3ec-5a01ea04b401";

        public static readonly Guid GuidAddConfigToSolutionCommand = new Guid(GuidNugetAddConfigFileToSolutionCommandString);

        #endregion
    };
}