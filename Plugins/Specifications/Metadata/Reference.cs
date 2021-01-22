using System.Xml.Serialization;

namespace FenixAlliance.ABP.Hub.Plugins
{
    [XmlRoot(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "reference")]
    public class Reference
    {
        [XmlAttribute(AttributeName = "file")]
        public string File { get; set; }
    }
}