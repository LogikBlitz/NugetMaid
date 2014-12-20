using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace LogikBlitz.NugetMaid.Commands.Nuget
{
    internal abstract class NugetCommand
    {
        private readonly IVsUIShell _uiShell;


        protected NugetCommand(IVsUIShell uiViewShell)
        {
            if (uiViewShell == null) throw new ArgumentNullException("uiViewShell", "The UI shell cannot be null");
            _uiShell = uiViewShell;
        }

        #region Methods

        internal void ShowErrorOccurredDialog(Exception ex)
        {
            var clsid = Guid.Empty;
            int result;
            ErrorHandler.ThrowOnFailure(_uiShell.ShowMessageBox(
                0,
                ref clsid,
                "An unknow error occurred.",
                ex.Message,
                string.Empty,
                0,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                OLEMSGICON.OLEMSGICON_INFO,
                0, // false
                out result));
        }


        internal IVsSolution GetSolution
        {
            get { return Package.GetGlobalService(typeof (SVsSolution)) as IVsSolution; }
        }

        internal bool ApplicationStateIsValid()
        {
            var application = (DTE) Package.GetGlobalService(typeof (SDTE));
            if (application.Solution == null || !application.Solution.IsOpen)
            {
                MessageBox.Show("Please open a solution first. ", "No solution");
                return false;
            }
            if (application.Solution.IsDirty)
                // solution must be saved otherwise adding/removing projects will raise errors
            {
                MessageBox.Show("Please save your solution first. \n" +
                                "Select the solution in the Solution Explorer and press Ctrl-S. ",
                    "Solution not saved");
                return false;
            }
            return true;
        }

        internal XDocument GetXDocumentAtPath(string filepath)
        {
            if (string.IsNullOrWhiteSpace(filepath))
            {
                throw new ArgumentNullException("filepath");
            }
            if (!File.Exists(filepath)) throw new FileNotFoundException("File not found", filepath);

            var document = XDocument.Load(filepath);
            return document;
        }


        protected T GetPropertyValue<T>(IVsSolution solutionInterface, __VSPROPID solutionProperty)
        {
            object value = null;
            T result = default(T);

            if (solutionInterface.GetProperty((int)solutionProperty, out value) == Microsoft.VisualStudio.VSConstants.S_OK)
            {
                result = (T)value;
            }
            return result;
        }

        #endregion
    }
}