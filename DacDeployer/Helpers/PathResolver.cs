using System;
using System.IO;
using static System.String;

namespace DeployDatabase.Helpers
{
    public static class PathResolver
    {
        public static (string dacPacFolder, string dacPacFile) GetDacPacPaths() 
        {
            var dacPacFile = ConsoleAppArgsParser.GetParamValue("DacPacFile", true);

            if (!File.Exists(dacPacFile))
                throw new FileNotFoundException($"The DacPac file is not found on the path provided with \"DacPacFile\" console application argument: \"{dacPacFile}\".");

            string dacPacFolder = Path.GetDirectoryName(dacPacFile);

            return (dacPacFolder, dacPacFile);
        }

        public static string GetPublishProfilePath()
        {
            var publishProfileFolder = ConsoleAppArgsParser.GetParamValue("PublishProfileFolder");

            if (!IsNullOrWhiteSpace(publishProfileFolder))            
            {
                if (!Directory.Exists(publishProfileFolder))
                    throw new FileNotFoundException($"The Publish Profile Folder is not found on the path provided with \"PublishProfileFolder\" console application argument: \"{publishProfileFolder}\".");
            }

            var publishProfileFile = ConsoleAppArgsParser.GetParamValue("PublishProfileFile");
            var buildConfiguration = ConsoleAppArgsParser.GetParamValue("BuildConfiguration");

            publishProfileFile = PublishProfileFileSelector.GetPublishProfileFile(publishProfileFolder, publishProfileFile, Environment.MachineName, buildConfiguration);

            return publishProfileFile;
        }

        public static (string beforeDeploymentScript, string sqlCmdVariablesScript) GetBeforeDeploymentPaths(string dacPacFolder)
        {
            var beforeDeploymentScript =  ConsoleAppArgsParser.GetParamValue("BeforeDeploymentScript");

            if (!IsNullOrWhiteSpace(beforeDeploymentScript))
            {
                beforeDeploymentScript = Path.Combine(dacPacFolder, beforeDeploymentScript);

                if (!File.Exists(beforeDeploymentScript))
                    throw new FileNotFoundException($"The BeforeDeploymentScript file is not found on the path provided with \"BeforeDeploymentScript\" console application argument: \"{beforeDeploymentScript}\".");
            }

            var sqlCmdVariablesScript = ConsoleAppArgsParser.GetParamValue("SqlCmdVariablesScript");

            if (!IsNullOrWhiteSpace(sqlCmdVariablesScript))
            {
                sqlCmdVariablesScript = Path.Combine(dacPacFolder, sqlCmdVariablesScript);

                if (!File.Exists(sqlCmdVariablesScript))
                    throw new FileNotFoundException($"The SqlCmdVariablesScript file is not found on the path provided with \"SqlCmdVariablesScript\" console application argument: \"{sqlCmdVariablesScript}\".");
            }

            Console.WriteLine($"beforeDeploymentScript...{          (beforeDeploymentScript == null ? "null" : $"\"{beforeDeploymentScript}\"")}");
            Console.WriteLine($"sqlCmdVariablesScript....{          (sqlCmdVariablesScript == null ? "null" : $"\"{sqlCmdVariablesScript}\"")}");

            return (beforeDeploymentScript, sqlCmdVariablesScript);
        }





    }

}
