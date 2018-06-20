using System;
using System.ComponentModel.Design;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Flavor;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;
using PclToNetStandard.Templates;
using Microsoft.VisualStudio;
using VSLangProj;
using NuGet.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;

namespace PclToNetStandard
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class ConvertCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("6553808a-4ee1-4e84-b99c-02f13a2e3246");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// DTE object
        /// </summary>
        private EnvDTE.DTE Dte;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConvertCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private ConvertCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new OleMenuCommand(this.Execute, menuCommandID);
            menuItem.BeforeQueryStatus += new EventHandler(OnBeforeQueryStatus);
            commandService.AddCommand(menuItem);
        }

        private void OnBeforeQueryStatus(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (Dte == null)
                return;
            Project project = (Project)((object[])Dte.ActiveSolutionProjects)[0];
            if (project == null)
                return;
            Solution.GetProjectOfUniqueName(project.UniqueName, out IVsHierarchy hierarchy);
            IVsAggregatableProjectCorrected ap = hierarchy as IVsAggregatableProjectCorrected;
            string projTypeGuids = null;
            ap?.GetAggregateProjectTypeGuids(out projTypeGuids);
            OleMenuCommand cmd = (OleMenuCommand)sender;
            cmd.Visible = projTypeGuids == Constants.PclProjectTypeGuids;
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static ConvertCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        private IVsSolution _solution;
        private IVsSolution Solution
        {
            get
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                if (_solution == null)
                    _solution = (IVsSolution)Package.GetGlobalService(typeof(SVsSolution));
                return _solution;
            }
        }

        private IVsPackageInstallerServices PackageInstallerService;

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Verify the current thread is the UI thread - the call to AddCommand in ConvertCommand's constructor requires
            // the UI thread.
            ThreadHelper.ThrowIfNotOnUIThread();
            OleMenuCommandService commandService = await package.GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;
            Instance = new ConvertCommand(package, commandService);
            Instance.Dte = await package.GetServiceAsync(typeof(DTE)) as DTE;
            var componentModel = await package.GetServiceAsync(typeof(SComponentModel)) as IComponentModel;
            Instance.PackageInstallerService = componentModel.GetService<IVsPackageInstallerServices>();
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Project project = (Project)((object[])Dte.ActiveSolutionProjects)[0];

            var nugetpackages = PackageInstallerService.GetInstalledPackages().ToList();
            var packagelist = new List<PackageReference>();
            foreach(var item in nugetpackages)
            {
                if(PackageInstallerService.IsPackageInstalled(project, item.Id))
                {
                    packagelist.Add(new PackageReference()
                    {
                        Name = item.Id,
                        Version = item.VersionString
                    });
                }
            }


            var service = ProjectConverterRepository.GetService(project);
            service.BackupOldVersionFiles();
            service.Convert();

            //ProjectItem assemblyInfo = project.GetAssemblyInfo();
            //var code = assemblyInfo.FileCodeModel;

            IVsSolution4 solution4 = Solution as IVsSolution4;
            Solution.GetProjectOfUniqueName(project.UniqueName, out IVsHierarchy hierarchy);
            int hr = 0;
            hierarchy.GetGuidProperty(Constants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ProjectIDGuid, out Guid guid);
            ErrorHandler.ThrowOnFailure(hr);
            solution4.UnloadProject(guid, (uint)_VSProjectUnloadStatus.UNLOADSTATUS_UnloadedByUser);
            solution4.ReloadProject(guid);
            service.DeleteOldVersionFiles();
        }

        private void BackupOldVersionFiles(Project project)
        {
            // Create backup folder.
            var backupTime = DateTime.UtcNow;
            var projectFolder = project.GetProjectRootPath();
            var backupDirectory = Directory.CreateDirectory(Path.Combine(projectFolder, $"backup-{backupTime.ToString("yyyy-MM-dd-hhmmss")}"));

            // Backup csproject file and package.config file.
            File.Copy(project.FullName, Path.Combine(backupDirectory.FullName, project.GetProjectFileName()));
            if(File.Exists(project.GetPackageConfigFilePath()))
                File.Copy(project.GetPackageConfigFilePath(), Path.Combine(backupDirectory.FullName, Constants.PackageConfigFileName));

            // Backup Properties folder and AssemblyInfo.cs file.
            var propertiesDestination = Path.Combine(backupDirectory.FullName, Constants.PropertiesFolderName);
            if (Directory.Exists(project.GetPropertiesFolderPath()))
                Directory.CreateDirectory(propertiesDestination);
            if(File.Exists(project.GetAssemblyInfoPath()))
                File.Copy(project.GetAssemblyInfoPath(), Path.Combine(propertiesDestination, Constants.AssemblyInfoCsFileName));
        }

        private void RemoveOldVersionFiles(Project project)
        {
            // Backup csproject file and package.config file.
            if (File.Exists(project.GetPackageConfigFilePath()))
                File.Delete(project.GetPackageConfigFilePath());

            // Backup Properties folder and AssemblyInfo.cs file.
            if (File.Exists(project.GetAssemblyInfoPath()))
                File.Delete(project.GetAssemblyInfoPath());
            if (Directory.Exists(project.GetPropertiesFolderPath()))
                Directory.Delete(project.GetPropertiesFolderPath());

        }
    }
}
