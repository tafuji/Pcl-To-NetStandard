using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PclToNetStandard.Templates
{
    internal partial class NetStandardTemplate
    {
        public List<PackageReference> Packages { get; set; } =  new List<PackageReference>();
    }

    internal class PackageReference
    {
        public string Name { get; set; }
        public string Version { get; set; }
    }
}
