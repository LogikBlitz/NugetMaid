using System;
using System.Xml;

namespace LogikBlitz.NugetMaid.Models
{
    internal static class NugetConfigFile
    {
        private static readonly XmlDocument _nugetConfig;
        private const string NugetConfigXmlString = @"<configuration>
  <config>
    <add key=""repositoryPath"" value=""packages"" />
  </config>
  <solution>
    <add key=""disableSourceControlIntegration"" value=""true"" />
  </solution>
</configuration>";

        static NugetConfigFile()
        {
            _nugetConfig = new XmlDocument();
            _nugetConfig.LoadXml(NugetConfigXmlString);
            _nugetConfig.PreserveWhitespace = true;
        }


        public static XmlDocument GetNugetConfig()
        {
            return _nugetConfig;
        }

        public static void SaveNugetConfigAtPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path", "nuget.config path to save at cannot be null or empty.");
            _nugetConfig.Save(path);
        }
    }
}