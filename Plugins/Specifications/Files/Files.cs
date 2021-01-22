using System.Xml.Serialization;

namespace FenixAlliance.ABP.Hub.Plugins.Specifications.Files
{
    [XmlRoot(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "files")]
    public class Files
    {
        [XmlAttribute(AttributeName = "include")]
        public string Include { get; set; }
        [XmlAttribute(AttributeName = "exclude")]
        public string Exclude { get; set; }
        [XmlAttribute(AttributeName = "buildAction")]
        public string BuildAction { get; set; }
        [XmlAttribute(AttributeName = "copyToOutput")]
        public string CopyToOutput { get; set; }
        [XmlAttribute(AttributeName = "flatten")]
        public string Flatten { get; set; }
        [XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "file")]
        public PackageFile File { get; set; }
    }
}