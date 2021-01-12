using System.Xml.Serialization;

namespace FenixAlliance.ABP.Hub.Plugins
{
    [XmlRoot(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "package")]
    public class Package
    {
        [XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "metadata")]
        public Metadata Metadata { get; set; }
        [XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "files")]
        public Files Files { get; set; }
    }
}