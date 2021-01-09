using System.Xml.Serialization;

namespace FenixAlliance.ABP.Hub.Plugins
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

	[XmlRoot(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "packageType")]
	public class PackageType
	{
		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }
		[XmlAttribute(AttributeName = "version")]
		public string Version { get; set; }
	}

	[XmlRoot(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "packageTypes")]
	public class PackageTypes
	{
		[XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "packageType")]
		public PackageType PackageType { get; set; }
	}

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

	[XmlRoot(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "group")]
	public class Group
	{
		[XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "dependency")]
		public Dependency Dependency { get; set; }
		[XmlAttribute(AttributeName = "targetFramework")]
		public string TargetFramework { get; set; }
		[XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "frameworkReference")]
		public FrameworkReference FrameworkReference { get; set; }
		[XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "reference")]
		public Reference Reference { get; set; }
	}

	[XmlRoot(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "dependencies")]
	public class Dependencies
	{
		[XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "dependency")]
		public Dependency Dependency { get; set; }
		[XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "group")]
		public Group Group { get; set; }
	}

	[XmlRoot(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "frameworkAssembly")]
	public class FrameworkAssembly
	{
		[XmlAttribute(AttributeName = "assemblyName")]
		public string AssemblyName { get; set; }
		[XmlAttribute(AttributeName = "targetFramework")]
		public string TargetFramework { get; set; }
	}

	[XmlRoot(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "frameworkAssemblies")]
	public class FrameworkAssemblies
	{
		[XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "frameworkAssembly")]
		public FrameworkAssembly FrameworkAssembly { get; set; }
	}

	[XmlRoot(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "frameworkReference")]
	public class FrameworkReference
	{
		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }
	}

	[XmlRoot(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "frameworkReferences")]
	public class FrameworkReferences
	{
		[XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "group")]
		public Group Group { get; set; }
	}

	[XmlRoot(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "reference")]
	public class Reference
	{
		[XmlAttribute(AttributeName = "file")]
		public string File { get; set; }
	}

	[XmlRoot(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "references")]
	public class References
	{
		[XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "reference")]
		public Reference Reference { get; set; }
		[XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "group")]
		public Group Group { get; set; }
	}

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

	[XmlRoot(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "contentFiles")]
	public class ContentFiles
	{
		[XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "files")]
		public Files Files { get; set; }
	}

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

	[XmlRoot(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "package")]
	public class Package
	{
		[XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "metadata")]
		public Metadata Metadata { get; set; }
		[XmlElement(Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd", ElementName = "files")]
		public Files Files { get; set; }
	}

}


