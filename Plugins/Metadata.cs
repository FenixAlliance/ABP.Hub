using System.Xml.Serialization;

namespace FenixAlliance.ABP.Hub.Plugins
{
    [XmlRoot(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "metadata")]
    public class Metadata
    {
        [XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "id")]
        public string Id { get; set; }
        [XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "version")]
        public string Version { get; set; }
        [XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "title")]
        public string Title { get; set; }
        [XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "authors")]
        public string Authors { get; set; }
        [XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "owners")]
        public string Owners { get; set; }
        [XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "licenseUrl")]
        public string LicenseUrl { get; set; }
        [XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "projectUrl")]
        public string ProjectUrl { get; set; }
        [XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "iconUrl")]
        public string IconUrl { get; set; }
        [XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "requireLicenseAcceptance")]
        public string RequireLicenseAcceptance { get; set; }
        [XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "developmentDependency")]
        public string DevelopmentDependency { get; set; }
        [XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "description")]
        public string Description { get; set; }
        [XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "summary")]
        public string Summary { get; set; }
        [XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "releaseNotes")]
        public string ReleaseNotes { get; set; }
        [XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "copyright")]
        public string Copyright { get; set; }
        [XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "language")]
        public string Language { get; set; }
        [XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "tags")]
        public string Tags { get; set; }
        [XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "serviceable")]
        public string Serviceable { get; set; }
        [XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "icon")]
        public string Icon { get; set; }
        [XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "repository")]
        public Repository Repository { get; set; }
        [XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "license")]
        public License License { get; set; }
        [XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "packageTypes")]
        public PackageTypes PackageTypes { get; set; }
        [XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "dependencies")]
        public Dependencies Dependencies { get; set; }
        [XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "frameworkAssemblies")]
        public FrameworkAssemblies FrameworkAssemblies { get; set; }
        [XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "frameworkReferences")]
        public FrameworkReferences FrameworkReferences { get; set; }
        [XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "references")]
        public References References { get; set; }
        [XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "contentFiles")]
        public ContentFiles ContentFiles { get; set; }
        [XmlAttribute(AttributeName = "minClientVersion")]
        public string MinClientVersion { get; set; }
    }
}