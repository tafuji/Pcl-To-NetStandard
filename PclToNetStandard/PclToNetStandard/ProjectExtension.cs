using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PclToNetStandard
{
    internal static class ProjectExtension
    {
        /// <summary>
        /// Package config file name.
        /// </summary>
        private const string PackageConfigFileName = "packages.config";

        /// <summary>
        /// Package config file path of the project.
        /// </summary>
        /// <param name="project">project</param>
        /// <returns></returns>
        public static string GetPackageConfigFilePath(this Project project)
        {
            var rootPath = System.IO.Path.GetDirectoryName(project.FileName);
            return System.IO.Path.Combine(rootPath, PackageConfigFileName);
        }
    }
}
