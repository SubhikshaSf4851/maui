using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using Microsoft.Maui.Controls.Xaml;

namespace Microsoft.Maui.Controls.SourceGen;

static class XmlTypeExtensions
{
	public static string GetTypeName(XmlType xmlType, SourceGenContext context, bool globalAlias = true)
		=> GetTypeName(xmlType, context.Compilation, context.XmlnsCache, context.TypeCache, globalAlias);

	public static ITypeSymbol? ResolveTypeSymbol(this XmlType xmlType, SourceGenContext context)
	{
		if (TryResolveTypeSymbol(xmlType, context, out var symbol))
			return symbol!;
		//FIXME reportDiagnostic
		// throw new BuildException(BuildExceptionCode.TypeResolution, xmlInfo, null, $"{xmlType.NamespaceUri}:{xmlType.Name}");
		return null;
	}

	public static bool TryResolveTypeSymbol(this XmlType xmlType, SourceGenContext context, out ITypeSymbol? symbol)
	{
		var xmlnsDefinitions = context.XmlnsCache.XmlnsDefinitions;
		symbol = xmlType.GetTypeReference(
			xmlnsDefinitions,
			context.Compilation.AssemblyName!, 
			typeInfo =>
			{
				var t = context.Compilation.GetTypeByMetadataName($"{typeInfo.clrNamespace}.{typeInfo.typeName}");
				if (t is not null && t.IsPublic())
					return t;
				return null;
			}
		);
		return symbol is not null;		
	}

	//FIXME should return a ITypeSymbol, and properly construct it for generics. globalalias param should go away
    public static string GetTypeName(this XmlType xmlType, Compilation compilation, AssemblyCaches xmlnsCache, IDictionary<XmlType, string> typeCache, bool globalAlias = true)
	{
		if (typeCache.TryGetValue(xmlType, out string returnType))
		{
			if (globalAlias)
				returnType = $"global::{returnType}";
			return returnType;
		}

		var ns = GetClrNamespace(xmlType.NamespaceUri);
		if (ns != null)
			returnType = $"{ns}.{xmlType.Name}";
		else
			// It's an external, non-built-in namespace URL.
			returnType = GetTypeNameFromCustomNamespace(xmlType, compilation, xmlnsCache);

		if (xmlType.TypeArguments != null)
			returnType = $"{returnType}<{string.Join(", ", xmlType.TypeArguments.Select(typeArg => GetTypeName(typeArg, compilation, xmlnsCache, typeCache)))}>";

		typeCache[xmlType] = returnType;
		if (globalAlias)
			returnType = $"global::{returnType}";
		return returnType;
	}

	static string? GetClrNamespace(string namespaceuri)
	{
		if (namespaceuri == XamlParser.X2009Uri)
			return "System";

		if (namespaceuri != XamlParser.X2006Uri &&
			!namespaceuri.StartsWith("clr-namespace", StringComparison.InvariantCulture) &&
			!namespaceuri.StartsWith("using:", StringComparison.InvariantCulture))
			return null;

		return XmlnsHelper.ParseNamespaceFromXmlns(namespaceuri);
	}

	static string GetTypeNameFromCustomNamespace(XmlType xmlType, Compilation compilation, AssemblyCaches xmlnsCache)
	{
#nullable disable
		string typeName = xmlType.GetTypeReference<string>(xmlnsCache.XmlnsDefinitions, null,
			(typeInfo) =>
			{
				string typeName = typeInfo.typeName.Replace('+', '/'); //Nested types
				string fullName = $"{typeInfo.clrNamespace}.{typeInfo.typeName}";
				IList<INamedTypeSymbol> types = compilation.GetTypesByMetadataName(fullName);

				if (types.Count == 0)
				{
					return null;
				}

				foreach (INamedTypeSymbol type in types)
				{
					// skip over types that are not in the correct assemblies
					if (type.ContainingAssembly.Identity.Name != typeInfo.assemblyName)
					{
						continue;
					}

					if (!type.IsPublicOrVisibleInternal(xmlnsCache.InternalsVisible))
					{
						continue;
					}

					int i = fullName.IndexOf('`');
					if (i > 0)
					{
						fullName = fullName.Substring(0, i);
					}
					return fullName;
				}

				return null;
			});

		return typeName;
#nullable enable
	}
}