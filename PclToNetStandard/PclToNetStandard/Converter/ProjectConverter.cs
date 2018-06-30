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
    internal abstract class ProjectConverter : IProjectConverter
    {
        protected string BackupFolderName { get; set; }
        protected string ProjectFolder { get; set; }
        protected string ProjectFileName { get; set; }
        protected string ProjectFullName { get; set; }
        protected string PackageConfigFilePath { get; set; }
        protected string PropertiesFolder { get; set; }
        protected string AssemblyInfoFilePath { get; set; }

        protected IVsSolution VsSolution { get; set; }
        protected Project DteProject { get; set; }
        protected IVsPackageInstallerServices PackageInstaller { get; set; }

        protected abstract void BackupOldVersionFiles();
        protected abstract void DeleteOldVersionFiles();
        protected abstract void Convert();
        protected abstract void ReloadProject();

        public void Execute()
        {
            BackupOldVersionFiles();
            Convert();
            ReloadProject();
            DeleteOldVersionFiles();
        }
    }
}
