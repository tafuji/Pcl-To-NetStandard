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
            var rootPath = project.GetProjectRootPath();
            return System.IO.Path.Combine(rootPath, PackageConfigFileName);
        }

        /// <summary>
        /// Get project file name.
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public static string GetProjectFileName(this Project project)
        {
            return System.IO.Path.GetFileName(project.FullName);
        }

        /// <summary>
        /// Get the root path of the projcet
        /// </summary>
        /// <param name="project">project</param>
        /// <returns></returns>
        public static string GetProjectRootPath(this Project project)
        {
            return System.IO.Path.GetDirectoryName(project.FileName);
        }
    }
}
