using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PclToNetStandard
{
    /// <summary>
    /// Constant values class
    /// </summary>
    internal class Constants
    {
        /// <summary>
        /// Portable Class Library projet Kind
        /// </summary>
        public static readonly string PclProjectTypeGuids = "{786C830F-07A1-408B-BD7F-6EE04809D6DB};{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";

        /// <summary>
        /// Package config file name.
        /// </summary>
        public static readonly string PackageConfigFileName = "packages.config";

        /// <summary>
        /// Properties folder name.
        /// </summary>
        public static readonly string PropertiesFolderName = "Properties";

        /// <summary>
        /// AssemblyInfo.cs file name.
        /// </summary>
        public static readonly string AssemblyInfoCsFileName = "AssemblyInfo.cs";

        /// <summary>
        /// Uint value of root item in Visual Studio
        /// </summary>
        public static readonly uint VSITEMID_ROOT = 0xFFFFFFFE;

    }
}
