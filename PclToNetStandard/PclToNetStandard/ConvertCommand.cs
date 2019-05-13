﻿using EnvDTE;
using Microsoft;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using NuGet.VisualStudio;
using PclToNetStandard.Extensions;
using System;
using System.ComponentModel.Design;
using Task = System.Threading.Tasks.Task;

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
        private DTE Dte;

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
            OleMenuCommand cmd = (OleMenuCommand)sender;
            cmd.Visible = project.IsPclProject(Solution);
            cmd.Enabled = !Dte.OnBuilding() && !Dte.OnDebugging();
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
        /// <summary>
        /// Solution.
        /// </summary>
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

        /// <summary>
        /// Package installer service.
        /// </summary>
        private IVsPackageInstallerServices PackageInstallerService;

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Verify the current thread is the UI thread - the call to AddCommand in ConvertCommand's constructor requires
            // the UI thread.
            //ThreadHelper.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            OleMenuCommandService commandService = await package.GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;
            Instance = new ConvertCommand(package, commandService);
            Instance.Dte = await package.GetServiceAsync(typeof(DTE)) as DTE;
            Assumes.Present(Instance.Dte);
            var componentModel = await package.GetServiceAsync(typeof(SComponentModel)) as IComponentModel;
            Assumes.Present(componentModel);
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
            var service = ProjectConverterRepository.GetService(project, PackageInstallerService, Solution);
            service.Execute();
        }
    }
}
