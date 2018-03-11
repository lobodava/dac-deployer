using System;
using DacDeployer.Helpers;

namespace DacDeployer
{
    class Program
    {
        static void Main()
        {
            var mode = ModeDefiner.DefineMode();

            if (!RunsDirectly())
                Logger.AppendEmptyLine();

            Logger.AppendLine($"DacDeployer is started in {mode.ToString().ToUpper()} mode!"); //Console.Title = {Console.Title}
            Logger.AppendEmptyLine();
            
            if (mode == Mode.Help) {
                HelpInfo.Help();
                PressAnyKey();
                Environment.Exit(0);
            }

            Logger.LogConsoleArguments();

            try 
            {
                (var dacPacFolderPath, var dacPacFilePath) = PathResolver.GetDacPacPaths();

                (var publishProfileFolderPath, var publishProfileFilePath) = PathResolver.GetPublishProfilePaths();

                if (mode == Mode.Compile)
                {
                    var compilationFolderPath = PathResolver.GetCompilationFolderPath();

                    var beforeDeploymentScriptPath = PathResolver.GetBeforeDeploymentPath();

                    Logger.LogResolvedPaths(dacPacFolderPath, dacPacFilePath, publishProfileFolderPath, publishProfileFilePath, beforeDeploymentScriptPath);

                    Logger.AppendLine("DacDeployer has started compilation:");
                    Logger.AppendDividerLine();

                    CompilationSteps.CreateOrClearCompilationFolder(compilationFolderPath);

                    CompilationSteps.CopyDacDeployer(compilationFolderPath);

                    CompilationSteps.CopyDacPac(compilationFolderPath, dacPacFolderPath);

                    CompilationSteps.CopyPublishProfiles(compilationFolderPath, publishProfileFolderPath);

                    CompilationSteps.CreateBatFiles(compilationFolderPath, dacPacFilePath, beforeDeploymentScriptPath);

                    Logger.AppendEmptyLine();
                    Logger.AppendLine("DacDeployer has completed compilation.");
                }
                else if (mode == Mode.Deploy)
                {
                    var publishProfile = new PublishProfile(publishProfileFilePath);

                    var beforeDeploymentScriptPath = PathResolver.GetBeforeDeploymentPath(dacPacFolderPath);

                    Logger.LogResolvedPaths(dacPacFolderPath, dacPacFilePath, publishProfileFolderPath, publishProfileFilePath, beforeDeploymentScriptPath);

                    Logger.AppendLine("DacDeployer has started deployment:");
                    Logger.AppendEmptyLine();

                    if (DeploymentSteps.ExecuteBeforeDeployment(beforeDeploymentScriptPath, publishProfile))
                    {
                       DeploymentSteps.DeployDacPac(dacPacFilePath, publishProfile);

                       Logger.AppendEmptyLine();
                       Logger.AppendLine("DacDeployer has completed deployment.");
                    }
                }
            }
            catch(Exception ex) 
            {
                Logger.AppendEmptyLine();
                Logger.AppendLine("DacDeployer.exe exited with the following error:");
                Logger.AppendLine(ex.Message);
                Logger.AppendEmptyLine();
            }

            Logger.OutputLogToFile();

            PressAnyKey();
        }

        private static void PressAnyKey()
        {
            if (RunsDirectly())
            {
                Console.WriteLine();
                Console.WriteLine("Press any key...");
                Console.ReadKey();
            }
        }



        private static bool RunsDirectly()
        {
            return (Console.Title?.EndsWith("DacDeployer.exe") == true);
        }
    }
}


//Console.WriteLine("Preparing Deployment Report:");
//Console.WriteLine();
//Console.Write(dacService.GenerateDeployReport(dacpac, DatabaseName, options: dacOptions)); 
//Console.WriteLine();

//Console.WriteLine("Preparing Deployment script:");
//Console.WriteLine();
//Console.Write(dacService.GenerateDeployScript(dacpac, DatabaseName, options: dacOptions)); 
//Console.WriteLine();
