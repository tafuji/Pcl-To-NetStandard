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

        public List<ProjectReference> ProjectReferences { get; set; } = new List<ProjectReference>();

        public string BackupFolderName { get; set; }
    }

   




}
