using System.Xml.Serialization;

namespace FenixAlliance.ABP.Hub.Plugins.Specifications.Metadata
{
    [XmlRoot(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "repository")]
	public class Repository
	{
		[XmlAttribute(AttributeName = "type")]
		public string Type { get; set; }
		[XmlAttribute(AttributeName = "url")]
		public string Url { get; set; }
		[XmlAttribute(AttributeName = "branch")]
		public string Branch { get; set; }
		[XmlAttribute(AttributeName = "commit")]
		public string Commit { get; set; }
	}
}


