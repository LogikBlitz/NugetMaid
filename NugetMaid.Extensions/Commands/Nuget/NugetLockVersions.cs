using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using EnvDTE;
using LogikBlitz.NugetMaid.Helpers;
using LogikBlitz.NugetMaid.Models;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace LogikBlitz.NugetMaid.Commands.Nuget
{
    internal class NugetLockVersions : NugetCommand
    {
        private readonly List<string> _lockedFiles;
        private readonly List<string> _unlockedFiles;


        public NugetLockVersions(IVsUIShell uiViewShell) : base(uiViewShell)
        {
            _lockedFiles = new List<string>();
            _unlockedFiles = new List<string>();
        }


        public OleMenuCommand LockNugetVersionsOleMenuCommand()
        {
            var menuCommandId = new CommandID(GuidList.GuidNugetLockVersionCommand,
                (int) PkgCmdIDList.cmdidLockNugetVersions);
            var menuItem = new OleMenuCommand(OnDoLockNugetVersions, menuCommandId);
            return menuItem;
        }

        public OleMenuCommand UnLockNugetVersionsOleMenuCommand()
        {
            var menuCommandId = new CommandID(GuidList.GuidNugetLockVersionCommand,
                (int) PkgCmdIDList.cmdidUnlockNugetVersions);
            var menuItem = new OleMenuCommand(OnDoUnLockNugetVersions, menuCommandId);
            return menuItem;
        }


        private void OnDoLockNugetVersions(object sender, EventArgs e)
        {
            try
            {
                var shouldShowResultDialog = LockNugetVersionInSolution(GetSolution);
                if (shouldShowResultDialog) ShowCommandResultDialog(_lockedFiles);
            }
            catch (Exception ex)
            {
                ShowErrorOccurredDialog(ex);
            }

            finally
            {
                _lockedFiles.Clear();
            }
        }

        private void OnDoUnLockNugetVersions(object sender, EventArgs e)
        {
            try
            {
                var shouldShowResultDialog = UnLockNugetVersionInSolution(GetSolution);
                if (shouldShowResultDialog) ShowCommandResultDialog(_unlockedFiles);
            }
            catch (Exception ex)
            {
                ShowErrorOccurredDialog(ex);
            }

            finally
            {
                _unlockedFiles.Clear();
            }
        }

        #region Dialogs

        private void ShowCommandResultDialog(IEnumerable<string> filePaths)
        {
            if (!filePaths.Any())
            {
                ShowNoFilesFoundDialog();
            }
            else
            {
                ShowFilesFoundDialog(filePaths);
            }
        }

        private void ShowNoFilesFoundDialog()
        {
            MessageBox.Show("No package.config files found. Please use NuGet to install packages.",
                "No packages.config files found.");
        }

        private void ShowFilesFoundDialog(IEnumerable<string> filePaths)
        {
            MessageBox.Show(
                string.Format("Following files has been touched:\n{0}", string.Join(Environment.NewLine, filePaths)),
                string.Format("Found {0} packages.config files.", filePaths.Count()));
        }

        #endregion

        #region private methods

        private const int S_OK = 0;
        private const string AllowedVersionsDefinition = "allowedVersions";

        #endregion

        #region Locking Nuget Versions

        internal bool LockNugetVersionInSolution(IVsSolution solution)
        {
            if (!ApplicationStateIsValid()) return false;
            var hierachy = solution as IVsHierarchy;
            if (hierachy == null) return true;
            hierachy.Iterate(TryLockPackagesInHierachy);
            return true;
        }


        internal void TryLockPackagesInHierachy(HierachyItem item)
        {
            var filePath = GetPackagesConfigFilePathFromHierachy(item);
            if (filePath == null) return;
            LockVersionInPackagesFile(filePath);
        }

        internal void LockVersionInPackagesFile(string filePath)
        {
            var document = GetXDocumentAtPath(filePath);

            //Clean the document of any old version locks.
            document = UnlockVersionsInXml(document);

            document = LockVersionsInXml(document);

            document.Save(filePath);

            _lockedFiles.Add(filePath);
        }


        internal XDocument LockVersionsInXml(XDocument document)
        {
            var packages = GetPackagesElements(document).ToList();
            foreach (var xElement in packages)
            {
                var version = xElement.Attribute("version");

                var allowedVersionsAtt = new XAttribute(AllowedVersionsDefinition, string.Format("[{0}]", version.Value));

                var attributes = xElement.Attributes().ToList();

                var copy = new List<XAttribute>(attributes.Count);

                copy.AddRange(attributes.Select(xAttribute => new XAttribute(xAttribute)));

                //Add allowedversions as the last attribute
                copy.Add(allowedVersionsAtt);
                var lockedNode = new XElement(xElement.Name, copy.ToArray());

                xElement.ReplaceWith(lockedNode);
            }
            return document;
        }

        #endregion

        #region UnLocking Nuget versions

        internal bool UnLockNugetVersionInSolution(IVsSolution solution)
        {
            _unlockedFiles.Clear();
            if (!ApplicationStateIsValid()) return false;
            var hierachy = solution as IVsHierarchy;
            if (hierachy == null) return true;
            hierachy.Iterate(TryUnLockPackagesInHierachy);
            return true;
        }


        internal void TryUnLockPackagesInHierachy(HierachyItem item)
        {
            var filePath = GetPackagesConfigFilePathFromHierachy(item);
            if (filePath == null) return;
            UnLockVersionInPackagesFile(filePath);
        }

        internal void UnLockVersionInPackagesFile(string filePath)
        {
            var document = GetXDocumentAtPath(filePath);

            document = UnlockVersionsInXml(document);

            document.Save(filePath);

            _unlockedFiles.Add(filePath);
        }

        internal XDocument UnlockVersionsInXml(XDocument document)
        {
            var packages = GetPackagesElements(document).ToList();

            foreach (var xElement in packages)
            {
                var allowedVersionsAtt = xElement.Attribute(AllowedVersionsDefinition);
                if (allowedVersionsAtt == null) continue;

                var attributes = xElement.Attributes().ToList();

                var copy = new List<XAttribute>(attributes.Count);
                foreach (var xAttribute in attributes)
                {
                    //Do not copy the restriction
                    if (xAttribute.Name.ToString().Equals(AllowedVersionsDefinition)) continue;

                    copy.Add(new XAttribute(xAttribute));
                }

                var newNode = new XElement(xElement.Name, copy.ToArray());

                xElement.ReplaceWith(newNode);
            }
            return document;
        }

        #endregion

        #region Shared Logic
        
        internal IEnumerable<XElement> GetPackagesElements(XDocument document)
        {
            if (document == null) throw new ArgumentNullException("document");
            return document.Descendants("packages").Descendants("package");
        }


        internal string GetPackagesConfigFilePathFromHierachy(HierachyItem item)
        {
            var hierarchy = item.Hierachy;
            var itemId = item.Id;
            object value = null;
            var name = "";
            var canonicalName = "";

            var result = hierarchy.GetProperty(itemId, (int) __VSHPROPID.VSHPROPID_Name, out value);

            if (result == S_OK && value != null)
            {
                name = value.ToString();
            }

            if (!name.Contains("packages.config")) return null;
            result = hierarchy.GetCanonicalName(itemId, out canonicalName);
            return canonicalName;
        }

        #endregion
    }
}