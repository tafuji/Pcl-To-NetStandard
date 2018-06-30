using EnvDTE;
using Microsoft.VisualStudio.Shell.Flavor;
using Microsoft.VisualStudio.Shell.Interop;
using NuGet.VisualStudio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PclToNetStandard.Extensions
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

        /// <summary>
        /// Get weather the project is portable class library or not.
        /// </summary>
        /// <param name="project"></param>
        /// <param name="solution"></param>
        /// <returns></returns>
        public static bool IsPclProject(this Project project, IVsSolution solution)
        {
            solution.GetProjectOfUniqueName(project.UniqueName, out IVsHierarchy hierarchy);
            IVsAggregatableProjectCorrected ap = hierarchy as IVsAggregatableProjectCorrected;
            string projTypeGuids = null;
            ap?.GetAggregateProjectTypeGuids(out projTypeGuids);
            return projTypeGuids == Constants.PclProjectTypeGuids;
        }

        /// <summary>
        /// Get project references with relative path.
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetProjectReferencesWithRelativePath(this Project project)
        {
            var referencePaths = new List<string>();
            VSLangProj.VSProject vSProject = project.Object as VSLangProj.VSProject;
            VSLangProj.References references = vSProject.References;
            foreach (VSLangProj.Reference reference in references)
            {
                if (reference.SourceProject != null)
                {
                    Project refProject = reference.SourceProject;
                    Uri u1 = new Uri($"{project.GetProjectRootPath()}{Path.DirectorySeparatorChar}");
                    Uri u2 = new Uri(refProject.FullName);
                    Uri relativeUri = u1.MakeRelativeUri(u2);
                    string relativePath = relativeUri.ToString();
                    relativePath.Replace('/', Path.DirectorySeparatorChar);
                    referencePaths.Add(relativePath);
                }
            }
            return referencePaths;
        }

        /// <summary>
        /// Get package references of the project.
        /// </summary>
        /// <param name="project"></param>
        /// <param name="vsPackageInstallerServices"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetPackageReferences(this Project project, IVsPackageInstallerServices vsPackageInstallerServices)
        {
            var packages = new Dictionary<string, string>();
            var nugetpackages = vsPackageInstallerServices.GetInstalledPackages().ToList();
            foreach (var item in nugetpackages)
            {
                if (vsPackageInstallerServices.IsPackageInstalledEx(project, item.Id, item.VersionString))
                {
                    packages.Add(item.Id, item.VersionString);
                }
            }
            return packages;
        }

        /// <summary>
        /// Get the project property values.
        /// </summary>
        /// <param name="project"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static string GetPropertyValue(this Project project, string propertyName)
        {
            string value = null;
            try
            {
                Property prop = project.Properties.Item(propertyName);
                value = prop.Value.ToString();
            }
            catch
            {
                return value;
            }
            return value;
        }
    }
}
