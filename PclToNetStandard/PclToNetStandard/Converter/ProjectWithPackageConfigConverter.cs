using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;
using NuGet.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using PclToNetStandard.Templates;
using PclToNetStandard.Extensions;

namespace PclToNetStandard
{
    internal class ProjectWithPackageConfigConverter : ProjectConverter
    {
        public ProjectWithPackageConfigConverter(Project project, IVsPackageInstallerServices packageInstaller, IVsSolution vsSolution)
        {
            PackageConfigFilePath = project.GetPackageConfigFilePath();
            ProjectFolder = project.GetProjectRootPath();
            ProjectFileName = project.GetProjectFileName();
            ProjectFullName = project.FullName;
            PropertiesFolder = project.GetPropertiesFolderPath();
            AssemblyInfoFilePath = project.GetAssemblyInfoPath();
            VsSolution = vsSolution;
            DteProject = project;
            PackageInstaller = packageInstaller;
            BackupFolderName = $"backup-{DateTime.UtcNow.ToString("yyyy-MM-dd-hhmmss")}";
        }

        protected override void BackupOldVersionFiles()
        {
            // Create backup folder.
            var backupDirectory = Directory.CreateDirectory(Path.Combine(ProjectFolder, BackupFolderName));

            // Backup csproject file and package.config file.
            File.Copy(ProjectFullName, Path.Combine(backupDirectory.FullName, ProjectFileName));
            if (File.Exists(PackageConfigFilePath))
                File.Copy(PackageConfigFilePath, Path.Combine(backupDirectory.FullName, Constants.PackageConfigFileName));

            // Backup Properties folder.
            if (Directory.Exists(PropertiesFolder))
            {
                var propertiesDestination = Path.Combine(backupDirectory.FullName, Constants.PropertiesFolderName);
                Directory.CreateDirectory(propertiesDestination);
                DirectoryCopy(PropertiesFolder, propertiesDestination, true);
            }
        }

        protected override void Convert()
        {
            var converter = new NetStandardTemplate();
            converter.BackupFolderName = BackupFolderName;
            converter.AssemblyName = DteProject.GetPropertyValue($"{nameof(NetStandardTemplate.AssemblyName)}");
            converter.RootNamespace = DteProject.GetPropertyValue($"{nameof(NetStandardTemplate.RootNamespace)}");
            converter.Company = DteProject.GetPropertyValue($"{nameof(NetStandardTemplate.Company)}");
            converter.Product = DteProject.GetPropertyValue($"{nameof(NetStandardTemplate.Product)}");
            converter.AssemblyVersion = DteProject.GetPropertyValue($"{nameof(NetStandardTemplate.AssemblyVersion)}");
            converter.AssemblyFileVersion = DteProject.GetPropertyValue($"{nameof(NetStandardTemplate.AssemblyFileVersion)}");
            converter.Description = DteProject.GetPropertyValue($"{nameof(NetStandardTemplate.Description)}");
            converter.Copyright = DteProject.GetPropertyValue($"{nameof(NetStandardTemplate.Copyright)}");
            converter.ProjectReferences = DteProject.GetProjectReferencesWithRelativePath().ToList();
            converter.Packages = DteProject.GetPackageReferences(PackageInstaller);

            File.Delete(ProjectFullName);
            var resultString = converter.TransformText();
            File.WriteAllText(ProjectFullName, resultString, Encoding.UTF8);
        }

        protected override void DeleteOldVersionFiles()
        {
            if (File.Exists(PackageConfigFilePath))
                File.Delete(PackageConfigFilePath);
            if (File.Exists(AssemblyInfoFilePath))
                File.Delete(AssemblyInfoFilePath);
            if (Directory.Exists(PropertiesFolder))
                Directory.Delete(PropertiesFolder);
        }

        protected override void ReloadProject()
        {
            IVsSolution4 solution4 = VsSolution as IVsSolution4;
            VsSolution.GetProjectOfUniqueName(DteProject.UniqueName, out IVsHierarchy hierarchy);
            int hr = 0;
            hierarchy.GetGuidProperty(Constants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ProjectIDGuid, out Guid guid);
            ErrorHandler.ThrowOnFailure(hr);
            solution4.UnloadProject(guid, (uint)_VSProjectUnloadStatus.UNLOADSTATUS_UnloadedByUser);
            solution4.ReloadProject(guid);
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}
