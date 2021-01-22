using System.Xml.Serialization;

namespace FenixAlliance.ABP.Hub.Plugins.Specifications.Metadata
{
    [XmlRoot(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "contentFiles")]
    public class ContentFiles
    {
        [XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "files")]
        public Files.Files Files { get; set; }
    }
}