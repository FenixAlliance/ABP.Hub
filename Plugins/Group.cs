using System.Xml.Serialization;

namespace FenixAlliance.ABP.Hub.Plugins
{
    [XmlRoot(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "group")]
    public class Group
    {
        [XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "dependency")]
        public Dependency Dependency { get; set; }
        [XmlAttribute(AttributeName = "targetFramework")]
        public string TargetFramework { get; set; }
        [XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "frameworkReference")]
        public FrameworkReference FrameworkReference { get; set; }
        [XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "reference")]
        public Reference Reference { get; set; }
    }
}