using EnvDTE;

namespace PclToNetStandard.Extensions
{
    internal static class DteExtension
    {
        /// <summary>
        /// Get weather solution is on building or not.
        /// </summary>
        /// <param name="dte"></param>
        /// <returns></returns>
        public static bool OnBuilding(this DTE dte)
        {
            Solution solution = dte.Solution;
            SolutionBuild solutionBuild = solution.SolutionBuild;
            vsBuildState buildState = solutionBuild.BuildState;
            return buildState == vsBuildState.vsBuildStateInProgress;
        }

        /// <summary>
        /// Get weather solution is on debugging or not.
        /// </summary>
        /// <param name="dte"></param>
        /// <returns></returns>
        public static bool OnDebugging(this DTE dte)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            Debugger debugger = dte.Debugger;
            return debugger.CurrentMode != dbgDebugMode.dbgDesignMode;
        }
    }
}
