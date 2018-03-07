using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.SqlServer.Dac;
using static System.String;


namespace DacDeployer.Helpers
{
	public class PublishProfile
	{
		public readonly string FilePath;
		
		public string ConnectionString { get; private set; }
		public string DatabaseName { get; private set; }
		public string ServerName { get; private set; }
        public bool IntegratedSecurity { get; private set; }
        public string UserID { get; private set; }
		public string Password { get; private set; }

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

            Logger.LogParsedPublishProfile(this);
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

            IntegratedSecurity = builder.IntegratedSecurity;

            if (!IntegratedSecurity)
			{
				UserID = builder.UserID;
				if (IsNullOrWhiteSpace(ServerName))
					throw new ArgumentException($"No UserID provided although the Integrated Security property of TargetConnectionString in publish profile \"{targetPublishProfileFile}\" set to \"false\"");

				Password = builder.Password;
				if (IsNullOrWhiteSpace(ServerName))
					throw new ArgumentException($"No Password provided although the Integrated Security property of TargetConnectionString in publish profile \"{targetPublishProfileFile}\" set to \"false\"");
			}

			DacDeployOptions = DacDeployOptionsCreator.CreateDacDeployOptions(doc, ns);
		}

	}
}
