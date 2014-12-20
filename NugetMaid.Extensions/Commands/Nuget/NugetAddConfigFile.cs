using System;
using System.ComponentModel.Design;
using System.IO;
using System.Windows.Forms;
using LogikBlitz.NugetMaid.Models;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace LogikBlitz.NugetMaid.Commands.Nuget
{
    internal class NugetAddConfigFile : NugetCommand
    {
        public NugetAddConfigFile(IVsUIShell uiViewShell) : base(uiViewShell) {}


        public OleMenuCommand AddNugetConfigToSolutionOleMenuCommand()
        {
            var menuCommandId = new CommandID(GuidList.GuidAddConfigToSolutionCommand,
                (int) PkgCmdIDList.cmdidAddNugetConfigToSolution);
            var menuItem = new OleMenuCommand(OnDoAddNugetConfigToSolution, menuCommandId);
            return menuItem;
        }

        #region Dialogs

        #endregion

        #region Methods

        private void OnDoAddNugetConfigToSolution(object sender, EventArgs e)
        {
            try
            {
                var pathOrEmptyString = AddNugetConfigToSolution(GetSolution);
                if (!string.IsNullOrEmpty(pathOrEmptyString))
                {
                    MessageBox.Show(
                        string.Format(
                            "Saved nuget.config at path:\n{0}.\n You need to close the solution for nuget to register the config.",
                            pathOrEmptyString),
                        "nuget.config saved to disk.");
                }
            }
            catch (Exception ex)
            {
                ShowErrorOccurredDialog(ex);
            }
        }

        private string AddNugetConfigToSolution(IVsSolution solution)
        {
            if (!ApplicationStateIsValid()) return string.Empty;

            var solutionDirectory = base.GetPropertyValue<string>(solution, __VSPROPID.VSPROPID_SolutionDirectory);

            return WriteConfigToSolutionDirectory(solutionDirectory);
        }

        private string WriteConfigToSolutionDirectory(string solutionDirectoryPath)
        {
            if (string.IsNullOrEmpty(solutionDirectoryPath))
                throw new ArgumentNullException("solutionDirectoryPath",
                    "The path to the solution directory cannot be null or empty.");

            var configPath = Path.Combine(solutionDirectoryPath, "nuget.config");
            NugetConfigFile.SaveNugetConfigAtPath(configPath);
            return configPath;
        }

        #endregion
    }
}