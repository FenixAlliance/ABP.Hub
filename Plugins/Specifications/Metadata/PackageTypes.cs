using System.Xml.Serialization;

namespace FenixAlliance.ABP.Hub.Plugins.Specifications.Metadata
{
    [XmlRoot(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "packageTypes")]
    public class PackageTypes
    {
        [XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "packageType")]
        public PackageType PackageType { get; set; }
    }
}