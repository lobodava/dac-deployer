using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static System.String;

namespace DacDeployer.Helpers
{
    public static class CompilationSteps
    {
        private const string DacDeployerFolderName = "DacDeployer";
        private const string DacPacFolderName = "DacPac";
        private const string PublishProfilesFolderName = "PublishProfiles";

        public static void CreateOrClearCompilationFolder(string compilationFolderPath)
        {
            if (!Directory.Exists(compilationFolderPath))
            {
                Directory.CreateDirectory(compilationFolderPath);

                Logger.AppendLine($"CompilationFolder created on \"{compilationFolderPath}\"");
            }
            else
            {
                var di = new DirectoryInfo(compilationFolderPath);

                foreach (FileInfo file in di.GetFiles())
                    file.Delete();

                foreach (DirectoryInfo dir in di.GetDirectories())
                    dir.Delete(true);

                Logger.AppendLine($"CompilationFolder cleared on \"{compilationFolderPath}\"");
            }
        }
        
        public static void CopyDacDeployer(string compilationFolderPath)
        {
            var targetDacDeployerFolder = Path.Combine(compilationFolderPath, DacDeployerFolderName);
            Directory.CreateDirectory(targetDacDeployerFolder);

            var sourceDacDeployerFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            foreach (var filePath in Directory.GetFiles(sourceDacDeployerFolder, "*.*", SearchOption.TopDirectoryOnly)
                                              .Where(s => s.EndsWith(".exe") || s.EndsWith(".dll") || s.EndsWith(".xml")))
                File.Copy(filePath, filePath.Replace(sourceDacDeployerFolder, targetDacDeployerFolder));

            Logger.AppendLine($"DacDeployer copied to \"{targetDacDeployerFolder}\"");
        }

        public static void CopyDacPac(string compilationFolderPath, string dacPacFolderPath)
        {
            var targetDacPacFolder = Path.Combine(compilationFolderPath, DacPacFolderName);
            Directory.CreateDirectory(targetDacPacFolder);

            foreach (var dirPath in Directory.GetDirectories(dacPacFolderPath, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(dacPacFolderPath, targetDacPacFolder));

            foreach (var filePath in Directory.GetFiles(dacPacFolderPath, "*.*", SearchOption.AllDirectories))
                File.Copy(filePath, filePath.Replace(dacPacFolderPath, targetDacPacFolder), true);

            Logger.AppendLine($"DacPac copied to \"{targetDacPacFolder}\"");
        }

        public static void CopyPublishProfiles(string compilationFolderPath, string publishProfileFolderPath)
        {
            var targetPublishProfileFolder = Path.Combine(compilationFolderPath, PublishProfilesFolderName);
            Directory.CreateDirectory(targetPublishProfileFolder);

            foreach (var filePath in Directory.GetFiles(publishProfileFolderPath, "*publish.xml"))
                File.Copy(filePath, filePath.Replace(publishProfileFolderPath, targetPublishProfileFolder));

            Logger.AppendLine($"Publish Profiles copied to \"{targetPublishProfileFolder}\"");
        }


        public static void CreateBatFiles(string compilationFolderPath, string dacPacFilePath, string beforeDeploymentScriptPath) 
        {
            var publishProfileFolder = Path.Combine(compilationFolderPath, PublishProfilesFolderName);
            var dacPacFileName = Path.GetFileName(dacPacFilePath);

            var batFileCount = 0;

            foreach (var filePath in Directory.GetFiles(publishProfileFolder, "*publish.xml"))
            {
                var publishProfileFileName = Path.GetFileName(filePath);

                var sb = new StringBuilder($"\"%~dp0{DacDeployerFolderName}\\DacDeployer.exe\" ")
                    .Append("/deploy ")
                    .Append($"/DacPacFile:\"%~dp0{DacPacFolderName}\\{dacPacFileName}\" ")
                    .Append($"/PublishProfileFile:\"%~dp0{PublishProfilesFolderName}\\{publishProfileFileName}\" ");

                if (!IsNullOrWhiteSpace(beforeDeploymentScriptPath))
                    sb.Append($"/BeforeDeploymentScript:\"%~dp0{DacPacFolderName}\\{beforeDeploymentScriptPath}\" ");

                var logFileName = Regex.Replace(publishProfileFileName, "[.]publish[.]xml", ".log.txt", RegexOptions.IgnoreCase);
                var logFilePath = Path.Combine(compilationFolderPath, logFileName);
                
                sb.AppendLine().AppendLine($"rem /LogFile:\"{logFilePath}\" ");

                var batFileName = Regex.Replace(publishProfileFileName, "[.]publish[.]xml", ".publish.bat", RegexOptions.IgnoreCase);
                var batFilePath = Path.Combine(compilationFolderPath, batFileName);

                File.WriteAllText(batFilePath, sb.ToString());

                batFileCount++;
            }

            Logger.AppendLine($"Bat files in quantity of {batFileCount} created in \"{compilationFolderPath}\"");

        }

    }
}
