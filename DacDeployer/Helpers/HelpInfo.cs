namespace DacDeployer.Helpers
{
    public static class HelpInfo
    {
        

        public static void Help() 
        {

            Logger.AppendLine("DacDeployer is a console application designed to deploy Data Tier Applications (Dac) with and without Visual Studio.");
            Logger.AppendEmptyLine();

            Logger.AppendLine("DacDeployer may work in three execution mode: Help (currnet mode), Compile and Deploy");
            Logger.AppendEmptyLine();

            Logger.AppendLine("In the Compile mode DacDeployer copies the files, required for deployment without Visual Studio, to a compilation folder.");
            Logger.AppendEmptyLine();

            Logger.AppendLine("As result of the execution, three folders and batch files will be created in the compilation folder:");
            Logger.AppendDividerLine();
            Logger.AppendLine("DacDeployer folder, which will contain this console application with all the required dependencies.");
            Logger.AppendLine("DacPac folder, which will contain the DacPac file with its dependencies.");
            Logger.AppendLine("PublishProfiles folder, which will contain all found \"*.publish.xml\" files.");
            Logger.AppendLine("The \"*.bat\" files will be added to the compilation folder, one bat file to each found \"*.publish.xml\" file.");
            Logger.AppendEmptyLine();

            Logger.AppendLine("The Compile mode is activated with a /Compile parameter");
            Logger.AppendEmptyLine();

            Logger.AppendLine("The following are the parameters in the Compile mode:");
            Logger.AppendDividerLine();
            Logger.AppendLine("/CompilationFolder — required parameter - is an absolute path to a folder where all the compiled files should be copied to.");
            Logger.AppendLine("/DacPacFile — required parameter - is an absolute path to a source file with extention \".dacpac\".");
            Logger.AppendLine("/PublishProfileFolder — required parameter - is an absolute path to a folder where the source \"*.publish.xml\" files are located.");
            Logger.AppendLine("/BeforeDeploymentScript — optional parameter - is an RELATIVE (from \".dacpac\" location) path to a file with sql script which has to be executed before the schema comparison.");
            Logger.AppendEmptyLine();

            Logger.AppendLine("The example of DacDeployer call in the Compile mode:");
            Logger.AppendLine(@"""C:\DacDeployer.exe"" /Compile /CompilationFolder:""C:\DacDeploy"" /DacPacFile:""C:\Project\bin\DB.dacpac"" /PublishProfileFolder:""C:\Project\PublishProfiles"" /BeforeDeploymentScript:"".\BeforeDeployment\_BeforeDeployment.script.sql""");
            Logger.AppendEmptyLine();

            Logger.AppendLine("The Deploy mode is activated with a /Deploy parameter");
            Logger.AppendEmptyLine();

            Logger.AppendLine("The following are the parameters in the Deploy mode:");
            Logger.AppendDividerLine();
            Logger.AppendLine("/DacPacFile — required parameter - is an absolute path to a source file with extention \".dacpac\".");
            Logger.AppendLine("/PublishProfileFolder — optional parameter - is an absolute path to a folder where the source \"*.publish.xml\" files are located.");
            Logger.AppendLine("/PublishProfileFile — optional parameter - is an absolute path to a publis profile file \"*.publish.xml\" which will be used for deployment.");
            Logger.AppendLine("/BuildConfiguration — optional parameter - build configuration ( Debug, Release, etc.). this parameter is used with /PublishProfileFolder."); 
            Logger.AppendLine("/BeforeDeploymentScript — optional parameter - is an RELATIVE (from \".dacpac\" location) path to a file with sql script which has to be executed before the schema comparison.");
            Logger.AppendEmptyLine();
            Logger.AppendLine("If /PublishProfileFile parameter is provided then it will be used for deployment");
            Logger.AppendLine("If /PublishProfileFile parameter is not provided and /PublishProfileFolder parameter provided instead (with or without /BuildConfiguration parameter),");
            Logger.AppendLine("then the PublishProfileFile will be searched in the PublishProfileFolder with the following name format priorities:");
            Logger.AppendLine("  {MachineName}.{BuildConfiguration}.publish.xml");
            Logger.AppendLine("  {MachineName}.publish.xml");
            Logger.AppendLine("  {BuildConfiguration}.publish.xml");
            Logger.AppendLine("  default.publish.xml");
            Logger.AppendEmptyLine();
            Logger.AppendLine("If no publish profile of such format is found the error occurs.");

            Logger.AppendLine("The example of DacDeployer call in the Compile mode:");
            Logger.AppendLine(@"""C:\DacDeployer.exe"" /Deploy /DacPacFile:""C:\Project\bin\DB.dacpac"" /PublishProfileFolder:""C:\Project\PublishProfiles"" /BuildConfiguration:""Debug"" /BeforeDeploymentScript:"".\BeforeDeployment\_BeforeDeployment.script.sql""");
            Logger.AppendEmptyLine();

            Logger.AppendLine("The following is example of the xml configuration in *.sqlproj to compile and deploy with DacDeployer:");
            Logger.AppendDividerLine();

            Logger.AppendLine(@"<PropertyGroup>");
            Logger.AppendLine(@"  <DacDeployerExe>""$(SolutionDir)DacDeployer\bin\Debug\DacDeployer.exe""</DacDeployerExe>");
            Logger.AppendLine(@"  <CompilationFolder>/CompilationFolder:""$(MSBuildProjectDirectory)\bin\DacDeploy""</CompilationFolder>");
            Logger.AppendLine(@"  <DacPacFile>/DacPacFile:""$(MSBuildProjectDirectory)\$(OutputPath)$(MSBuildProjectName).dacpac""</DacPacFile>");
            Logger.AppendLine(@"  <PublishProfileFolder>/PublishProfileFolder:""$(MSBuildProjectDirectory)\PublishProfiles""</PublishProfileFolder>");
            Logger.AppendLine(@"  <BuildConfiguration>/BuildConfiguration:""$(Configuration)""</BuildConfiguration>");
            Logger.AppendLine(@"  <BeforeDeploymentScript>/BeforeDeploymentScript:"".\0. Before Deployment\_BeforeDeployment.script.sql""</BeforeDeploymentScript>");
            Logger.AppendLine(@"</PropertyGroup>");
            Logger.AppendLine(@"<Target Name=""AfterBuild"">");
            Logger.AppendLine(@"  <Message Importance=""High"" Text=""."" />");
            Logger.AppendLine(@"  <Message Importance=""High"" Text=""Execution of DacDeployer.exe in COMPILE mode on AfterBuild event:"" />");
            Logger.AppendLine(@"  <Exec Command=""$(DacDeployerExe) /compile $(CompilationFolder) $(DacPacFile) $(PublishProfileFolder) $(BeforeDeploymentScript)""></Exec>");
            Logger.AppendLine(@"  <Message Importance=""High"" Text=""."" />");
            Logger.AppendLine(@"</Target>");
            Logger.AppendLine(@"<Target Name=""AfterRebuild"">");
            Logger.AppendLine(@"  <Message Importance=""High"" Text=""."" />");
            Logger.AppendLine(@"  <Message Importance=""High"" Text=""Execution of DacDeployer.exe in DEPLOY mode on AfterRebuild event:"" />");
            Logger.AppendLine(@"  <Exec Command=""$(DacDeployerExe) /deploy $(DacPacFile) $(PublishProfileFolder) $(BuildConfiguration) $(BeforeDeploymentScript)""></Exec>");
            Logger.AppendLine(@"  <Message Importance=""High"" Text=""."" />");
            Logger.AppendLine(@"</Target>");
        }
    }
}
