using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeployDatabase.Helpers
{
    public static class PathResolver
    {
        public static (string dacPacFolder, string dacPacFile) GetDacPacPaths() 
        {
            string dacPacFile = ConsoleAppArgsParser.GetParamValue("DacPacFile", true);

            if (!File.Exists(dacPacFile))
                throw new FileNotFoundException($"The DacPac file is not found on the path provided with \"DacPacFile\" console application argument: \"{DacPacFile}\".");

            string dacPacFolder = Path.GetDirectoryName(dacPacFile);

            return (dacPacFolder, dacPacFile);
        }

        public static (string publishProfileFolder, string publishProfileFile) GetPublishProfilePaths()
        {
            string publishProfileFolder = ConsoleAppArgsParser.GetParamValue("PublishProfileFolder");

            if (!String.IsNullOrWhiteSpace(publishProfileFolder))            
            {
                if (!Directory.Exists(publishProfileFolder))
                    throw new FileNotFoundException($"The Publish Profile Folder is not found on the path provided with \"PublishProfileFolder\" console application argument: \"{publishProfileFolder}\".");
            }

            string publishProfileFile = ConsoleAppArgsParser.GetParamValue("PublishProfileFile");
            string buildConfiguration = ConsoleAppArgsParser.GetParamValue("BuildConfiguration");

            publishProfileFile = PublishProfileFileSelector.GetPublishProfileFile(publishProfileFolder, publishProfileFile, Environment.MachineName, buildConfiguration);

            //Console.WriteLine($"PublishProfileFolder = \"{PublishProfileFolder ?? "null"}\"");
            //Console.WriteLine($"PublishProfileFile = \"{PublishProfileFile ?? "null"}\"");
            //Console.WriteLine($"DacPacFolder = \"{DacPacFolder ?? "null"}\"");
            //Console.WriteLine($"BuildConfiguration = \"{BuildConfiguration ?? "null"}\"");  

            return (publishProfileFolder, publishProfileFile);
        }

    }

    }
}
