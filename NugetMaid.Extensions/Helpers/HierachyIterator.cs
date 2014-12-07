using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;

namespace LogikBlitz.NugetMaid.Helpers
{
    /// <summary>
    ///     Parts of this class has been derived from the work of Carlos J. Quintero, MZ-Tools
    ///     that can be found here: http://www.mztools.com/articles/2014/MZ2014007.aspx
    /// </summary>
    internal class HierachyIterator
    {






        private const int S_OK = 0;
        private const uint VSITEMID_NIL = 0xFFFFFFFF;
        private const uint VSITEMID_ROOT = 0xFFFFFFFE;

        public void IterateHierachy(IVsHierarchy hierarchy, Action<Tuple<IVsHierarchy, uint>> processNode)
        {
            // Traverse the nodes of the hierarchy from the root node
            ProcessHierarchyNodeRecursively(hierarchy, VSITEMID_ROOT, processNode);
        }

        private void ProcessHierarchyNodeRecursively(IVsHierarchy hierarchy, uint itemId,
            Action<Tuple<IVsHierarchy, uint>> processNode)
        {
            int result;
            var nestedHiearchyValue = IntPtr.Zero;
            uint nestedItemIdValue = 0;
            object value = null;
            uint visibleChildNode;
            Guid nestedHierarchyGuid;
            IVsHierarchy nestedHierarchy;

            // First, guess if the node is actually the root of another hierarchy (a project, for example)
            nestedHierarchyGuid = typeof (IVsHierarchy).GUID;
            result = hierarchy.GetNestedHierarchy(itemId, ref nestedHierarchyGuid, out nestedHiearchyValue,
                out nestedItemIdValue);

            if (result == S_OK && nestedHiearchyValue != IntPtr.Zero && nestedItemIdValue == VSITEMID_ROOT)
            {
                // Get the new hierarchy
                nestedHierarchy = Marshal.GetObjectForIUnknown(nestedHiearchyValue) as IVsHierarchy;
                Marshal.Release(nestedHiearchyValue);

                if (nestedHierarchy != null)
                {
                    IterateHierachy(nestedHierarchy, processNode);
                }
            }
            else // The node is not the root of another hierarchy, it is a regular node
            {
                processNode(new Tuple<IVsHierarchy, uint>(hierarchy, itemId));

                // Get the first visible child node
                result = hierarchy.GetProperty(itemId, (int) __VSHPROPID.VSHPROPID_FirstVisibleChild, out value);

                while (result == S_OK && value != null)
                {
                    if (value is int && (uint) (int) value == VSITEMID_NIL)
                    {
                        // No more nodes
                        break;
                    }
                    else
                    {
                        visibleChildNode = Convert.ToUInt32(value);

                        // Enter in recursion
                        ProcessHierarchyNodeRecursively(hierarchy, visibleChildNode, processNode);

                        // Get the next visible sibling node
                        value = null;
                        result = hierarchy.GetProperty(visibleChildNode, (int) __VSHPROPID.VSHPROPID_NextVisibleSibling,
                            out value);
                    }
                }
            }
        }
    }
}