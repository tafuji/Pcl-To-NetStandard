using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace PclToNetStandard.Templates
{
    internal class PackageConfigParser
    {

        public string ConfigFilePath { get; set; }

        public PackageConfigParser(string packageConfigPath)
        {
            ConfigFilePath = packageConfigPath;
        }

        public PackageConfigRoot Parse()
        {
            var packages = new XmlSerializer(typeof(PackageConfigRoot)).Deserialize(new XmlTextReader(ConfigFilePath)) as PackageConfigRoot;
            return packages;
        }
    }

    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(ElementName = "packages", Namespace = "", IsNullable = false)]
    public partial class PackageConfigRoot
    {
        [XmlElement("package")]
        public PackageElement[] Packages { get; set; }
    }

    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType = true)]
    public partial class PackageElement
    {

        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("version")]
        public string Version { get; set; }

        [XmlAttribute("targetFramework")]
        public string TargetFramework { get; set; }

        [XmlAttribute("developmentDependency")]
        public bool DevelopmentDependency { get; set; }

        [XmlAttribute("allowedVersions")]
        public string AllowedVersions { get; set; }
    }
}
