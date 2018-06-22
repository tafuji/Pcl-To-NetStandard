using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PclToNetStandard.Templates
{
    internal class ProjectInformation
    {
        public string AssemblyName { get; set; }
        public string RootNamespace { get; set; }
        public string Company { get; set; }
        public string Product { get; set; }
        public string AssemblyVersion { get; set; }
        public string AssemblyFileVersion { get; set; }
        public string Description { get; set; }
        public string Copyright { get; set; }
    }
}
