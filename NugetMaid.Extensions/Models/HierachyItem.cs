using System;
using Microsoft.VisualStudio.Shell.Interop;

namespace LogikBlitz.NugetMaid.Models
{
    public class HierachyItem
    {
        public IVsHierarchy Hierachy { get; private set; }
        public uint Id { get; private set; }


        public HierachyItem(IVsHierarchy hierachy, uint id)
        {
            if (hierachy == null) throw new ArgumentNullException("hierachy");
            Hierachy = hierachy;
            Id = id;
        }
    }
}