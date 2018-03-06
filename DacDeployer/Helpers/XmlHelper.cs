using System;
using System.Xml;
using static System.String;

namespace DacDeployer.Helpers
{
	public static class PublishProfileXmlHelper
	{
		public const string MsBuildXmlNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";

		public static (XmlDocument , XmlNamespaceManager ) LoadPublishProfile (string PublishProfileFile)
		{
			var doc = new XmlDocument();
			var ns = GetNamespaceManager(doc, MsBuildXmlNamespace);
			doc.Load(PublishProfileFile);
			return (doc, ns);
		}


		public static string GetRequiredXmlNodeValue(XmlDocument doc, XmlNamespaceManager ns, string pathToXmlNode)
		{
			var nodeToFind = doc.SelectSingleNode(pathToXmlNode, ns);

			if (nodeToFind == null)
				throw new ArgumentNullException($"Node {pathToXmlNode.Replace("ns:", "")} not found in {doc.Name}");
			
			if (IsNullOrWhiteSpace(nodeToFind.InnerText))
				throw new ArgumentNullException($"Node {pathToXmlNode.Replace("ns:", "")} has no value in {doc.Name}");

			return nodeToFind.InnerText;
		}

		public static string GetOptionalXmlNodeValue(XmlDocument doc, XmlNamespaceManager ns, string pathToXmlNode)
		{
			var nodeToFind = doc.SelectSingleNode(pathToXmlNode, ns);

			return IsNullOrWhiteSpace(nodeToFind?.InnerText) ? null : nodeToFind.InnerText;
		}

		public static bool? GetBooleanOptionValue(XmlDocument doc, XmlNamespaceManager ns, string optionName, bool? defaultValue = false)
		{
			
			string pathToXmlNode = $"//ns:Project/ns:PropertyGroup/ns:{optionName}";

			var nodeToFind = doc.SelectSingleNode(pathToXmlNode, ns);

			if (IsNullOrWhiteSpace(nodeToFind?.InnerText))
				return defaultValue;

			if (nodeToFind.InnerText.Equals("true", StringComparison.InvariantCultureIgnoreCase))
				return true;
			
			if  (nodeToFind.InnerText.Equals("false", StringComparison.InvariantCultureIgnoreCase))
				return true;

			return defaultValue;
		}

		public static XmlNamespaceManager GetNamespaceManager(XmlDocument doc, String nameSpace) 
		{
			var ns = new XmlNamespaceManager(doc.NameTable);
			ns.AddNamespace("ns", nameSpace);
			return ns;
		}
	}
}
