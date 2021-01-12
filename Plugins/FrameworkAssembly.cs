using System.Xml.Serialization;

namespace FenixAlliance.ABP.Hub.Plugins
{
    [XmlRoot(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "frameworkAssembly")]
    public class FrameworkAssembly
    {
        [XmlAttribute(AttributeName = "assemblyName")]
        public string AssemblyName { get; set; }
        [XmlAttribute(AttributeName = "targetFramework")]
        public string TargetFramework { get; set; }
    }
}