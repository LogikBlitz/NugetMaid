using System;
using System.Runtime.InteropServices;
using InfomediaAS.VS_Extensions.Models;
using Microsoft.VisualStudio.Shell.Interop;

namespace InfomediaAS.VS_Extensions.Helpers
{
    internal static class HierachyExtension
    {
        public static void Iterate(this IVsHierarchy hierachy, Action<HierachyItem> processHierachyCallBack)
        {
            ProcessHierarchyNodeRecursively(hierachy, VSITEMID_ROOT, processHierachyCallBack);
        }

        private const int S_OK = 0;
        private const uint VSITEMID_NIL = 0xFFFFFFFF;
        private const uint VSITEMID_ROOT = 0xFFFFFFFE;

        private static void ProcessHierarchyNodeRecursively(IVsHierarchy hierarchy, uint itemId,
           Action<HierachyItem> processNode)
        {
            int result;
            var nestedHiearchyValue = IntPtr.Zero;
            uint nestedItemIdValue = 0;
            object value = null;
            uint visibleChildNode;
            Guid nestedHierarchyGuid;
            IVsHierarchy nestedHierarchy;

            // First, guess if the node is actually the root of another hierarchy (a project, for example)
            nestedHierarchyGuid = typeof(IVsHierarchy).GUID;
            result = hierarchy.GetNestedHierarchy(itemId, ref nestedHierarchyGuid, out nestedHiearchyValue,
                out nestedItemIdValue);

            if (result == S_OK && nestedHiearchyValue != IntPtr.Zero && nestedItemIdValue == VSITEMID_ROOT)
            {
                // Get the new hierarchy
                nestedHierarchy = Marshal.GetObjectForIUnknown(nestedHiearchyValue) as IVsHierarchy;
                Marshal.Release(nestedHiearchyValue);

                if (nestedHierarchy != null)
                {
                    nestedHierarchy.Iterate(processNode);
                    //IterateHierachy(nestedHierarchy, processNode);
                }
            }
            else // The node is not the root of another hierarchy, it is a regular node
            {
                processNode(new HierachyItem(hierarchy, itemId));

                // Get the first visible child node
                result = hierarchy.GetProperty(itemId, (int)__VSHPROPID.VSHPROPID_FirstVisibleChild, out value);

                while (result == S_OK && value != null)
                {
                    if (value is int && (uint)(int)value == VSITEMID_NIL)
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
                        result = hierarchy.GetProperty(visibleChildNode, (int)__VSHPROPID.VSHPROPID_NextVisibleSibling,
                            out value);
                    }
                }
            }
        }
    }


}