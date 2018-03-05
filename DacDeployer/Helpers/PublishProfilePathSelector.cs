using System;
using System.IO;
using static System.String;

namespace DeployDatabase.Helpers
{
	public static class PublishProfileFileSelector
	{

		private static bool IsEmpty (string param) => IsNullOrWhiteSpace(param);
		private static bool IsNotEmpty (string param) => !IsNullOrWhiteSpace(param);


		public static string GetPublishProfileFile(string PublishProfileFolder, string PublishProfileFile, string machineName, string buildConfiguration)
		{
			if (IsEmpty(PublishProfileFolder) && IsEmpty(PublishProfileFile)) 
				throw new ArgumentException("Both of the concole application parameters \"PublishProfileFolder\" and \"PublishProfileFile\" have no value. One of them has to be provided.");

			if (IsNotEmpty(PublishProfileFile)) {
				if (File.Exists(PublishProfileFile))
					return PublishProfileFile;

				//if (IsEmpty(PublishProfileFolder))
					throw new FileNotFoundException($"The publish profile file is not found on the path provided with \"PublishProfileFile\" console application argument: \"{PublishProfileFile}\".");
			}

			string tryPublishProfileFile;

			if (IsNotEmpty(machineName) && IsNotEmpty(buildConfiguration))
			{
				tryPublishProfileFile = Path.Combine(PublishProfileFolder, $"{machineName}.{buildConfiguration}.publish.xml");
				
				if (File.Exists(tryPublishProfileFile))
					return tryPublishProfileFile;
			}

			if (IsNotEmpty(machineName))
			{
				tryPublishProfileFile = Path.Combine(PublishProfileFolder, $"{machineName}.publish.xml");
				
				if (File.Exists(tryPublishProfileFile))
					return tryPublishProfileFile;
			}


			if (IsNotEmpty(buildConfiguration))
			{
				tryPublishProfileFile = Path.Combine(PublishProfileFolder, $"{buildConfiguration}.publish.xml");
				
				if (File.Exists(tryPublishProfileFile))
					return tryPublishProfileFile;
			}

			tryPublishProfileFile =  Path.Combine(PublishProfileFolder, "default.publish.xml"); ;

			if (File.Exists(tryPublishProfileFile))
				return tryPublishProfileFile;


			throw new FileNotFoundException($"The publish profile file is not found in the directory provided with \"PublishProfileFolder\" console application argument: \"{PublishProfileFile}\"." 
				+ "Either provide absolute path to a certain publish profile with \"PublishProfileFile\" concole app argrument or rename the target publish profile file as: "
				+ "\"{MachineName}.{BuildConfiguration}.publish.xml\", \"{MachineName}.publish.xml\", \"{BuildConfiguration}.publish.xml\" or just \"default.publish.xml\"." );
		}

	}
}
