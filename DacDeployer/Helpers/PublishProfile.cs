using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.SqlServer.Dac;
using static System.String;


namespace DeployDatabase.Helpers
{
	public class PublishProfile
	{
		public readonly string FilePath;
		
		public string ConnectionString { get; private set; }
		public string DatabaseName { get; private set; }
		public string ServerName { get; private set; }
		public string UserID { get; private set; }
		public string Password { get; private set; }
		
		//public string BeforeDeploymentAbsoluteDirectory { get; set; }
		//public string BeforeDeploymentRelativeDirectory { get; private set; }
		//public string BeforeDeploymentScriptName { get; private set; }
		//public string SqlCmdVariablesScriptName { get; private set; }

		public DacDeployOptions DacDeployOptions { get; private set; }

		private readonly Dictionary<string, string> sqlCmdVariables = new Dictionary<string, string>();

		public Dictionary<string, string> SqlCmdVariables
		{
			get 
			{
				if (DacDeployOptions == null)
					return sqlCmdVariables;

				return sqlCmdVariables.Union(DacDeployOptions.SqlCommandVariableValues).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
			}
		}
			
			

		public PublishProfile (string PublishProfileFile) {

			FilePath = PublishProfileFile;

			ReadPublishProfile(PublishProfileFile);

		}

		private void ReadPublishProfile(string targetPublishProfileFile) 
		{
			(var doc, var ns) = PublishProfileXmlHelper.LoadPublishProfile(targetPublishProfileFile);
			
			DatabaseName = PublishProfileXmlHelper.GetRequiredXmlNodeValue(doc, ns, "//ns:Project/ns:PropertyGroup/ns:TargetDatabaseName");

			if (IsNullOrWhiteSpace(DatabaseName)) 
				throw new ArgumentException($"Database Name not found in publish profile \"{targetPublishProfileFile}\". The node \"//Project/PropertyGroup/TargetDatabaseName\" is empty or missed.");

			sqlCmdVariables.Add("DatabaseName", DatabaseName);


			ConnectionString = PublishProfileXmlHelper.GetRequiredXmlNodeValue(doc, ns, "//ns:Project/ns:PropertyGroup/ns:TargetConnectionString");
			
			var builder = new SqlConnectionStringBuilder(ConnectionString);

			ServerName = builder.DataSource;
			if (IsNullOrWhiteSpace(ServerName))
				throw new ArgumentException($"No Server name (DataSource) provided in TargetConnectionString property of publish profile {targetPublishProfileFile}. The node \"//Project/PropertyGroup/TargetConnectionString\" is empty or missed.");

			if (!builder.IntegratedSecurity)
			{
				UserID = builder.UserID;
				if (IsNullOrWhiteSpace(ServerName))
					throw new ArgumentException($"No UserID provided although the Integrated Security property of TargetConnectionString in publish profile \"{targetPublishProfileFile}\" set to \"false\"");

				Password = builder.Password;
				if (IsNullOrWhiteSpace(ServerName))
					throw new ArgumentException($"No Password provided although the Integrated Security property of TargetConnectionString in publish profile \"{targetPublishProfileFile}\" set to \"false\"");
			}

			//BeforeDeploymentRelativeDirectory = PublishProfileXmlHelper.GetOptionalXmlNodeValue(doc, ns, "//ns:Project/ns:ItemGroup/ns:SqlCmdVariable[@Include='BeforeDeploymentRelativeDirectory']/ns:Value");
			//BeforeDeploymentScriptName = PublishProfileXmlHelper.GetOptionalXmlNodeValue(doc, ns, "//ns:Project/ns:ItemGroup/ns:SqlCmdVariable[@Include='BeforeDeploymentScriptName']/ns:Value"); 
			//SqlCmdVariablesScriptName = PublishProfileXmlHelper.GetOptionalXmlNodeValue(doc, ns, "//ns:Project/ns:ItemGroup/ns:SqlCmdVariable[@Include='SqlCmdVariablesScriptName']/ns:Value"); 

			DacDeployOptions = DacDeployOptionsCreator.CreateDacDeployOptions(doc, ns);
		}

	}
}
