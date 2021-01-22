using System.Xml.Serialization;

namespace FenixAlliance.ABP.Hub.Plugins.Specifications.Metadata
{
    [XmlRoot(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "dependency")]
    public class Dependency
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
        [XmlAttribute(AttributeName = "include")]
        public string Include { get; set; }
        [XmlAttribute(AttributeName = "exclude")]
        public string Exclude { get; set; }
    }
}