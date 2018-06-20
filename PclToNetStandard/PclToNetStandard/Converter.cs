using EnvDTE;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PclToNetStandard.Templates;

namespace PclToNetStandard
{

    internal class ProjectConverterRepository
    {
        public static IProjectConverter GetService(Project project)
        {
            if(project.ContainsProjectJson())
            {

            }
            return new PackageConfigProjectConverter(project);
        }
    }

    internal class PackageConfigProjectConverter : IProjectConverter
    {
        private string PackageConfigFilePath;
        private string ProjectFolder;
        private string ProjectFileName;
        private string ProjectFullName;
        private string PropertiesFolder;
        private string AssemblyInfoFilePath;


        public PackageConfigProjectConverter(Project project)
        {
            PackageConfigFilePath = project.GetPackageConfigFilePath();
            ProjectFolder = project.GetProjectRootPath();
            ProjectFileName = project.GetProjectFileName();
            ProjectFullName = project.FullName;
            PropertiesFolder = project.GetPropertiesFolderPath();
            AssemblyInfoFilePath = project.GetAssemblyInfoPath();
        }

        public void BackupOldVersionFiles()
        {
            // Create backup folder.
            var backupTime = DateTime.UtcNow;
            var backupDirectory = Directory.CreateDirectory(Path.Combine(ProjectFolder, $"backup-{backupTime.ToString("yyyy-MM-dd-hhmmss")}"));

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

        public bool Convert()
        {
            var converter = new NetStandardTemplate();
            if (File.Exists(PackageConfigFilePath))
            {
                var config = new PackageConfigParser(PackageConfigFilePath).Parse();
                foreach (var item in config.Packages)
                {
                    var reference = new PackageReference()
                    {
                        Name = item.Id,
                        Version = item.Version
                    };
                    converter.Packages.Add(reference);
                }
            }
            File.Delete(ProjectFullName);
            var resultString = converter.TransformText();
            File.WriteAllText(ProjectFullName, resultString, System.Text.Encoding.UTF8);

            return true;
        }

        public void DeleteOldVersionFiles()
        {
            if (File.Exists(PackageConfigFilePath))
                File.Delete(PackageConfigFilePath);
            if (File.Exists(AssemblyInfoFilePath))
                File.Delete(AssemblyInfoFilePath);
            if (Directory.Exists(PropertiesFolder))
                Directory.Delete(PropertiesFolder);
        }
    }

    interface IProjectConverter
    {
        void BackupOldVersionFiles();
        void DeleteOldVersionFiles();
        bool Convert();
    }

}
