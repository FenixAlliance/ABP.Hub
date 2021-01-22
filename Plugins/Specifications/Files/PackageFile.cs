using System.Xml.Serialization;

namespace FenixAlliance.ABP.Hub.Plugins.Specifications.Files
{
    [XmlRoot(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "file")]
    public class PackageFile
    {
        [XmlAttribute(AttributeName = "src")]
        public string Src { get; set; }
        [XmlAttribute(AttributeName = "target")]
        public string Target { get; set; }
        [XmlAttribute(AttributeName = "exclude")]
        public string Exclude { get; set; }
    }
}