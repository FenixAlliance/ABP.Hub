using System.Xml.Serialization;

namespace FenixAlliance.ABP.Hub.Plugins.Specifications.Metadata
{
    [XmlRoot(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "frameworkReferences")]
    public class FrameworkReferences
    {
        [XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "group")]
        public Group Group { get; set; }
    }
}