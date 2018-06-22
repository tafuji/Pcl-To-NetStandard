using EnvDTE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PclToNetStandard
{
    internal static class ProjectExtension
    {

        /// <summary>
        /// Get the AssemblyInfo.cs file path.
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public static string GetAssemblyInfoPath(this Project project) => Path.Combine(project.GetPropertiesFolderPath(), Constants.AssemblyInfoCsFileName);

        /// <summary>
        /// Package config file path of the project.
        /// </summary>
        /// <param name="project">project</param>
        /// <returns></returns>
        public static string GetPackageConfigFilePath(this Project project) => Path.Combine(project.GetProjectRootPath(), Constants.PackageConfigFileName);

        /// <summary>
        /// Get the Properties directory path.
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public static string GetPropertiesFolderPath(this Project project) => Path.Combine(project.GetProjectRootPath(), Constants.PropertiesFolderName);

        /// <summary>
        /// Get project file name.
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public static string GetProjectFileName(this Project project) => Path.GetFileName(project.FullName);

        /// <summary>
        /// Get the root path of the projcet
        /// </summary>
        /// <param name="project">project</param>
        /// <returns></returns>
        public static string GetProjectRootPath(this Project project) => Path.GetDirectoryName(project.FileName);
    }
}
