using System;
using System.IO;
using static System.String;

namespace DacDeployer.Helpers
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

        public static (string publishProfileFolder, string publishProfileFile) GetPublishProfilePaths()
        {
            var publishProfileFolder = ConsoleAppArgsParser.GetParamValue("PublishProfileFolder");

            if (!IsNullOrWhiteSpace(publishProfileFolder))            
            {
                if (!Directory.Exists(publishProfileFolder))
                    throw new FileNotFoundException($"The Publish Profile Folder is not found on the path provided with \"PublishProfileFolder\" console application argument: \"{publishProfileFolder}\".");
                
                if (ConsoleAppArgsParser.ParamExists("prepare"))
                    return (publishProfileFolder, null);
           }

            var publishProfileFile = ConsoleAppArgsParser.GetParamValue("PublishProfileFile");
            var buildConfiguration = ConsoleAppArgsParser.GetParamValue("BuildConfiguration");

            Console.WriteLine("PublishProfile paths:");
            Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
            Console.WriteLine($"PublishProfileFolder:         \"{publishProfileFolder}\"");
            Console.WriteLine($"PublishProfileFile:           {(publishProfileFile == null ? "null" : $"\"{publishProfileFile}\"")}");
            Console.WriteLine($"BuildConfiguration:           {(buildConfiguration == null ? "null" : $"\"{buildConfiguration}\"")}");

            publishProfileFile = PublishProfileFileSelector.GetPublishProfileFile(publishProfileFolder, publishProfileFile, Environment.MachineName, buildConfiguration);

            Console.WriteLine($"Choisen PublishProfileFile:  \"{publishProfileFile}\"");
            //Console.WriteLine("------------------------------------------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine();

            return (publishProfileFolder, publishProfileFile);
        }

        public static (string beforeDeploymentScript, string sqlCmdVariablesScript) GetBeforeDeploymentPaths(string dacPacFolder = null)
        {
            var beforeDeploymentScript =  ConsoleAppArgsParser.GetParamValue("BeforeDeploymentScript");

            if (!IsNullOrWhiteSpace(beforeDeploymentScript) && !IsNullOrWhiteSpace(dacPacFolder))
            {
                beforeDeploymentScript = Path.Combine(dacPacFolder, beforeDeploymentScript);

                if (!File.Exists(beforeDeploymentScript))
                    throw new FileNotFoundException($"The BeforeDeploymentScript file is not found on the path provided with \"BeforeDeploymentScript\" console application argument: \"{beforeDeploymentScript}\".");
            }

            var sqlCmdVariablesScript = ConsoleAppArgsParser.GetParamValue("SqlCmdVariablesScript");

            if (!IsNullOrWhiteSpace(sqlCmdVariablesScript) && !IsNullOrWhiteSpace(dacPacFolder))
            {
                sqlCmdVariablesScript = Path.Combine(dacPacFolder, sqlCmdVariablesScript);

                if (!File.Exists(sqlCmdVariablesScript))
                    throw new FileNotFoundException($"The SqlCmdVariablesScript file is not found on the path provided with \"SqlCmdVariablesScript\" console application argument: \"{sqlCmdVariablesScript}\".");
            }

            Console.WriteLine("BeforeDeployment paths:");
            Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
            Console.WriteLine($"BeforeDeploymentScript:  {(beforeDeploymentScript == null ? "null" : $"\"{beforeDeploymentScript}\"")}");
            Console.WriteLine($"SqlCmdVariablesScript:   {(sqlCmdVariablesScript == null ? "null" : $"\"{sqlCmdVariablesScript}\"")}");
            //Console.WriteLine("------------------------------------------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine();

            return (beforeDeploymentScript, sqlCmdVariablesScript);
        }

        public static string GetCompilationFolderPath()
        {
            /*var compilationFolder = ConsoleAppArgsParser.GetParamValue("CompilationFolder", true);*/
            return @"C:\Current Projects\DacDeployer\TestDb\bin\DacDeploy";
        }



    }

}
