﻿using System;
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

            var dacPacFolder = Path.GetDirectoryName(dacPacFile);

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

            var chosenPublishProfileFile = PublishProfileFileSelector.GetPublishProfileFile(publishProfileFolder, publishProfileFile, Environment.MachineName, buildConfiguration);

            return (publishProfileFolder, chosenPublishProfileFile);
        }

        public static string  GetBeforeDeploymentPath(string dacPacFolder = null)
        {
            var beforeDeploymentScript =  ConsoleAppArgsParser.GetParamValue("BeforeDeploymentScript");

            if (!IsNullOrWhiteSpace(beforeDeploymentScript) && !IsNullOrWhiteSpace(dacPacFolder))
            {
                beforeDeploymentScript = Path.Combine(dacPacFolder, beforeDeploymentScript);

                if (!File.Exists(beforeDeploymentScript))
                    throw new FileNotFoundException($"The BeforeDeploymentScript file is not found on the path provided with \"BeforeDeploymentScript\" console application argument: \"{beforeDeploymentScript}\".");
            }

            return beforeDeploymentScript;
        }

        public static string GetCompilationFolderPath()
        {
            return ConsoleAppArgsParser.GetParamValue("CompilationFolder", true);
        }
    }

}
