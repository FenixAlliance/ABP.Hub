using System.Xml.Serialization;

namespace FenixAlliance.ABP.Hub.Plugins
{
    [XmlRoot(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "frameworkReference")]
    public class FrameworkReference
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }
}