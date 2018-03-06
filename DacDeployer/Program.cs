using System;
using DacDeployer.Helpers;

namespace DacDeployer
{
    class Program
    {
        static void Main()
        {
            var mode = ModeDefiner.DefineMode();

            Console.WriteLine();
            Console.WriteLine($"Dac Deployer started in {mode.ToString().ToUpper()} mode!"); //Console.Title = {Console.Title}
            Console.WriteLine();

            try 
            {
                (var dacPacFolderPath, var dacPacFilePath) = PathResolver.GetDacPacPaths();

                (var publishProfileFolderPath, var publishProfileFilePath) = PathResolver.GetPublishProfilePaths();

                if (mode == Mode.Compile)
                {
                    var compilationFolderPath = PathResolver.GetCompilationFolderPath();

                    (var beforeDeploymentScriptPath, var sqlCmdVariablesScriptPath) = PathResolver.GetBeforeDeploymentPaths();

                    CompilationSteps.CreateOrClearCompilationFolder(compilationFolderPath);

                    CompilationSteps.CopyDacDeployer(compilationFolderPath);

                    CompilationSteps.CopyDacPac(compilationFolderPath, dacPacFolderPath);

                    CompilationSteps.CopyPublishProfiles(compilationFolderPath, publishProfileFolderPath);

                    CompilationSteps.CreateBatFiles(compilationFolderPath, dacPacFilePath, beforeDeploymentScriptPath, sqlCmdVariablesScriptPath);
                }
                else if (mode == Mode.Deploy)
                {
                    var publishProfile = new PublishProfile(publishProfileFilePath);

                    (var beforeDeploymentScriptPath, var sqlCmdVariablesScriptPath) = PathResolver.GetBeforeDeploymentPaths(dacPacFolderPath);

                    DeploymentSteps.AppendSqlCmdVariables(sqlCmdVariablesScriptPath, publishProfile);

                    if (DeploymentSteps.ExecuteBeforeDeployment(beforeDeploymentScriptPath, publishProfile))
                        DeploymentSteps.DeployDacPac(dacPacFilePath, publishProfile);
                }

                if (RunsDirectly()) Console.ReadKey();

            }
            catch(Exception ex) 
            {
                ExitWithError(ex.Message);
            }
        }


        private static void ExitWithError(string message) {
            Console.WriteLine();
            Console.WriteLine("DacDeployer.exe exited with the following error:");
            Console.WriteLine(message);
            Console.WriteLine();

            if (RunsDirectly())
            {
                Console.WriteLine("Press any key...");
                Console.ReadKey();
            }
            Environment.Exit(0);
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
