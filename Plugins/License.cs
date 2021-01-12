using System.Xml.Serialization;

namespace FenixAlliance.ABP.Hub.Plugins
{
    [XmlRoot(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "license")]
    public class License
    {
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
        [XmlText]
        public string Text { get; set; }
    }
}