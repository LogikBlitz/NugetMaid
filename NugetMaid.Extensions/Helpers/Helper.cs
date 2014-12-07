using System;
using System.Collections.Generic;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace LogikBlitz.NugetMaid.Helpers
{
    internal class Helper
    {
        public static IEnumerable<IVsProject> LoadedProjectsForSolution(IVsSolution solution)
        {
            IEnumHierarchies enumerator = null;
            var guid = Guid.Empty;
            solution.GetProjectEnum((uint) __VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION, ref guid, out enumerator);
            var hierarchy = new IVsHierarchy[1] {null};
            uint fetched = 0;
            for (enumerator.Reset();
                enumerator.Next(1, hierarchy, out fetched) == VSConstants.S_OK && fetched == 1;
                /*nothing*/)
            {
                yield return (IVsProject) hierarchy[0];
            }
        }
    }
}