using System;
using System.Reflection;
using Microsoft.PlayerFramework.Design.Common;
using Microsoft.Windows.Design.Metadata;

[assembly: ProvideMetadata(typeof(Microsoft.PlayerFramework.Design.MetadataRegistration))]

namespace Microsoft.PlayerFramework.Design
{
	public class MetadataRegistration : MetadataRegistrationBase, IProvideAttributeTable
	{
		public MetadataRegistration() : base()
		{
			// Note:
			// The default constructor sets value of AssemblyFullName and 
			// XmlResourceName used by MetadataRegistrationBase.AddDescriptions().
			// The convention here is that the <RootNamespace> in .design.csproj
			// (or Default namespace in Project -> Properties -> Application tab)
			// must be the same as runtime assembly's main namespace (t.Namespace)
			// plus .Design.
            AssemblyFullName = "Microsoft.PlayerFramework";
            XmlResourceName = "Microsoft.PlayerFramework.xml";
		}

		#region IProvideAttributeTable Members

		/// <summary>
		/// Gets the AttributeTable for design time metadata.
		/// </summary>
		public AttributeTable AttributeTable
		{
			get
			{
				return BuildAttributeTable();
			}
		}

		#endregion
	}
}