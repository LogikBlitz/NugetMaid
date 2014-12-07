// Guids.cs
// MUST match guids.h

using System;

namespace LogikBlitz.NugetMaid
{
    static class GuidList
    {
        public const string guidVS_ExtensionsPkgString = "0e707c91-7054-48b7-9612-6e7d040054e4";
        public const string GuidNugetLockVersionCommandString = "68ce72a7-a177-4818-b1e9-4fa9ffc5c7f8";
        public const string GuidNugetUnlockVersionCommandString = "260978c3-582c-487d-ab12-c1fdde07c555";

        public static readonly Guid GuidNugetLockVersionCommand = new Guid(GuidNugetLockVersionCommandString);
        public static readonly Guid GuidNugetUnlockVersionCommand = new Guid(GuidNugetUnlockVersionCommandString);
    };
}