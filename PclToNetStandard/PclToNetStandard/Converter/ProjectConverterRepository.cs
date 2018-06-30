using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using NuGet.VisualStudio;

namespace PclToNetStandard
{
    internal class ProjectConverterRepository
    {
        public static IProjectConverter GetService(Project project, IVsPackageInstallerServices packageInstaller, IVsSolution vsSolution)
        {
            return new ProjectWithPackageConfigConverter(project, packageInstaller, vsSolution);
        }
    }
}
