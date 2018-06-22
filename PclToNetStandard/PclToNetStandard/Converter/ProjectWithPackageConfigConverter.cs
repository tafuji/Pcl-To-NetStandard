using EnvDTE;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PclToNetStandard.Templates;
using NuGet.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;

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
            var backupTime = DateTime.UtcNow;
            var backupDirectory = Directory.CreateDirectory(Path.Combine(ProjectFolder, BackupFolderName));

            // Backup csproject file and package.config file.
            File.Copy(ProjectFullName, Path.Combine(backupDirectory.FullName, ProjectFileName));
            if (File.Exists(PackageConfigFilePath))
                File.Copy(PackageConfigFilePath, Path.Combine(backupDirectory.FullName, Constants.PackageConfigFileName));

            // Backup Properties folder and AssemblyInfo.cs file.
            var propertiesDestination = Path.Combine(backupDirectory.FullName, Constants.PropertiesFolderName);
            if (Directory.Exists(PropertiesFolder))
                Directory.CreateDirectory(propertiesDestination);
            if (File.Exists(AssemblyInfoFilePath))
                File.Copy(AssemblyInfoFilePath, Path.Combine(propertiesDestination, Constants.AssemblyInfoCsFileName));
        }

        protected override void Convert()
        {
            var converter = new NetStandardTemplate();
            var projectInformation = new ProjectInformation()
            {
                AssemblyName = DteProject.Properties.Item($"{nameof(ProjectInformation.AssemblyName)}").Value?.ToString(),
                RootNamespace = DteProject.Properties.Item($"{nameof(ProjectInformation.RootNamespace)}")?.Value?.ToString(),
                Company = DteProject.Properties.Item($"{nameof(ProjectInformation.Company)}")?.Value?.ToString(),
                Product = DteProject.Properties.Item($"{nameof(ProjectInformation.Product)}")?.Value?.ToString(),
                AssemblyVersion = DteProject.Properties.Item($"{nameof(ProjectInformation.AssemblyVersion)}")?.Value?.ToString(),
                AssemblyFileVersion = DteProject.Properties.Item($"{nameof(ProjectInformation.AssemblyFileVersion)}")?.Value?.ToString(),
                Description = DteProject.Properties.Item($"{nameof(ProjectInformation.Description)}")?.Value?.ToString(),
                Copyright = DteProject.Properties.Item($"{nameof(ProjectInformation.Copyright)}")?.Value?.ToString()
            };
            converter.Information = projectInformation;

            VSLangProj.VSProject vSProject = DteProject.Object as VSLangProj.VSProject;
            VSLangProj.References references = vSProject.References;
            foreach(VSLangProj.Reference reference in references)
            {
                if (reference.SourceProject != null)
                {
                    Project refProject = reference.SourceProject;
                    Uri u1 = new Uri($"{ProjectFolder}{Path.DirectorySeparatorChar}");
                    Uri u2 = new Uri(refProject.FullName);
                    Uri relativeUri = u1.MakeRelativeUri(u2);
                    string relativePath = relativeUri.ToString();
                    relativePath.Replace('/', Path.DirectorySeparatorChar);
                    converter.ProjectReferences.Add(new ProjectReference() { Include = relativePath });
                }
            }
            converter.BackupFolderName = BackupFolderName;
            var nugetpackages = PackageInstaller.GetInstalledPackages().ToList();
            var packagelist = new List<PackageReference>();
            foreach (var item in nugetpackages)
            {
                if (PackageInstaller.IsPackageInstalledEx(DteProject, item.Id, item.VersionString))
                {
                    var reference = new PackageReference()
                    {
                        Name = item.Id,
                        Version = item.VersionString
                    };
                    converter.Packages.Add(reference);
                }
            }
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
    }
}
