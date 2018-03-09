using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using static System.String;

namespace DacDeployer.Helpers
{
    public static class HelpInfo
    {
        

        public static void Help() 
        {

            Logger.AppendLine("DacDeployer is a console application designed to deploy Data Tier Applications (Dac) with and without Visual Studio.");
            Logger.AppendEmptyLine();

            Logger.AppendLine("DacDeployer may work in three execution mode: Help (cuuernt mode), Compile and Deploy");
            Logger.AppendEmptyLine();

            Logger.AppendLine("In the Compile mode DacDeployer copies the files, required for deployment without Visual Studio, to a compilation folder.");

            Logger.AppendLine("As result of the execution, three folder will be created in the compilation folder:");
            Logger.AppendLine("DacDeployer folder, which will contain this console application with all the required dependencies.");
            Logger.AppendLine("DacPac folder, which will contain the DacPac file with its dependencies.");
            Logger.AppendLine("PublishProfiles folder, which will contain all found \"*.publish.xml\" files.");
            Logger.AppendLine("The \"*.bat\" files will be added to the compilation folder, one bat file to each found \"*.publish.xml\" file.");


            Logger.AppendLine("The Compile mode is activated with a /Compile parameter");

            Logger.AppendLine("The following are the parameters in the Compile mode:");

            Logger.AppendLine("/CompilationFolder — required parameter - is an absolute path to a folder where all the compiled files should be copied to.");

            Logger.AppendLine("/DacPacFile — required parameter - is an absolute path to a source file with extention \".dacpac\".");

            Logger.AppendLine("/PublishProfileFolder — required parameter - is an absolute path to a folder where the source \"*.publish.xml\" files are located.");

            Logger.AppendLine("/BeforeDeploymentScript — optional parameter - is an RELATIVE (from \".dacpac\" location) path to a file with sql script which has to be executed before the schema comparison.");

            Logger.AppendLine("/SqlCmdVariablesScript — optional parameter - is an RELATIVE (from \".dacpac\" location) path to a file where the SqlCmd Variables will automatically be written to from a publish profile.");


            Logger.AppendEmptyLine();


            Logger.AppendLine("The example of DacDeployer call in the Compile mode:");

            Logger.AppendLine(@"""C:\DacDeployer.exe"" /compile /CompilationFolder:""C:\DacDeploy"" /DacPacFile:""C:\Project\bin\DB.dacpac"" /PublishProfileFolder:""C:\Project\PublishProfiles"" /BeforeDeploymentScript:"".\BeforeDeployment\_BeforeDeployment.script.sql"" /SqlCmdVariablesScript:"".\BeforeDeployment\_SqlCmdVariables.script.sql""");







            //$(CompilationFolder) $(DacPacFile) $(PublishProfileFolder) $(BeforeDeploymentScript) $(SqlCmdVariablesScript)


            //BeforeDeploymentScriptPath
            //SqlCmdVariablesScriptPath
            //PublishProfileFolderPath
            //PublishProfileFilePath
            //DacPacFilePath


            //BeforeDeploymentScript
            //SqlCmdVariablesScript
            //PublishProfileFolder
            //PublishProfileFile
            //DacPacFile




            //Logger.AppendLine($"Bat files in quantity of {batFileCount} created in \"{compilationFolderPath}\"");

        }




    }
}
