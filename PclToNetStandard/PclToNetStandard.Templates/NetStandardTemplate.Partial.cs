using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PclToNetStandard.Templates
{
    internal partial class NetStandardTemplate
    {
        public List<PackageReference> Packages { get; set; } = new List<PackageReference>();

        public List<ProjectReference> ProjectRegerences { get; set; } = new List<ProjectReference>();
    }

   




}
