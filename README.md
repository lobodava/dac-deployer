# DacDeployer

DacDeployer is a console application designed to deploy Data Tier Applications (Dac) with and without Visual Studio.

DacDeployer may work in three execution mode: Help, Compile and Deploy

## "Compile" mode

In the Compile mode DacDeployer copies the files, required for deployment without Visual Studio, to a compilation folder.

As result of the execution, three folders and batch files will be created in the compilation folder:

DacDeployer folder, which will contain this console application with all the required dependencies.
DacPac folder, which will contain the DacPac file with its dependencies.
PublishProfiles folder, which will contain all found "*.publish.xml" files.
The "*.bat" files will be added to the compilation folder, one bat file to each found "*.publish.xml" file.

The Compile mode is activated with a /Compile parameter

The following are the parameters in the Compile mode:
```
/CompilationFolder - required parameter - is an absolute path to a folder where all the compiled files should be copied to.
/DacPacFile - required parameter - is an absolute path to a source file with extention ".dacpac".
/PublishProfileFolder - required parameter - is an absolute path to a folder where the source "*.publish.xml" files are located.
/BeforeDeploymentScript - optional parameter - is an RELATIVE (from ".dacpac" location) path to a file with sql script which has to be executed before the schema comparison.
```

The example of DacDeployer call in the Compile mode:
```
"C:\DacDeployer.exe" /Compile /CompilationFolder:"C:\DacDeploy" /DacPacFile:"C:\Project\bin\DB.dacpac" /PublishProfileFolder:"C:\Project\PublishProfiles" /BeforeDeploymentScript:".\BeforeDeployment\_BeforeDeployment.script.sql"
```

## "Deploy" mode

The Deploy mode is activated with a /Deploy parameter

The following are the parameters in the Deploy mode:
```
/DacPacFile - required parameter - is an absolute path to a source file with extention ".dacpac".
/PublishProfileFolder - optional parameter - is an absolute path to a folder where the source "*.publish.xml" files are located.
/PublishProfileFile - optional parameter - is an absolute path to a publis profile file "*.publish.xml" which will be used for deployment.
/BuildConfiguration - optional parameter - build configuration ( Debug, Release, etc.). this parameter is used with /PublishProfileFolder.
/BeforeDeploymentScript - optional parameter - is an RELATIVE (from ".dacpac" location) path to a file with sql script which has to be executed before the schema comparison.
```

If /PublishProfileFile parameter is provided then it will be used for deployment
If /PublishProfileFile parameter is not provided and /PublishProfileFolder parameter provided instead (with or without /BuildConfiguration parameter),
then the PublishProfileFile will be searched in the PublishProfileFolder with the following name format priorities:
```
{MachineName}.{BuildConfiguration}.publish.xml
{MachineName}.publish.xml
{BuildConfiguration}.publish.xml
default.publish.xml
```
If no publish profile of such format is found the error occurs.

The example of DacDeployer call in the Compile mode:
```
"C:\DacDeployer.exe" /Deploy /DacPacFile:"C:\Project\bin\DB.dacpac" /PublishProfileFolder:"C:\Project\PublishProfiles" /BuildConfiguration:"Debug" /BeforeDeploymentScript:".\BeforeDeployment\_BeforeDeployment.script.sql"
```

## Visual Studio MSBuild project configuration

The following is example of the xml configuration in *.sqlproj to compile and deploy with DacDeployer:
```xml
<PropertyGroup>
  <DacDeployerExe>"$(SolutionDir)DacDeployer\bin\Debug\DacDeployer.exe"</DacDeployerExe>
  <CompilationFolder>/CompilationFolder:"$(MSBuildProjectDirectory)\bin\DacDeploy"</CompilationFolder>
  <DacPacFile>/DacPacFile:"$(MSBuildProjectDirectory)\$(OutputPath)$(MSBuildProjectName).dacpac"</DacPacFile>
  <PublishProfileFolder>/PublishProfileFolder:"$(MSBuildProjectDirectory)\PublishProfiles"</PublishProfileFolder>
  <BuildConfiguration>/BuildConfiguration:"$(Configuration)"</BuildConfiguration>
  <BeforeDeploymentScript>/BeforeDeploymentScript:".\0. Before Deployment\_BeforeDeployment.script.sql"</BeforeDeploymentScript>
</PropertyGroup>
<Target Name="AfterBuild">
  <Message Importance="High" Text="." />
  <Message Importance="High" Text="Execution of DacDeployer.exe in COMPILE mode on AfterBuild event:" />
  <Exec Command="$(DacDeployerExe) /compile $(CompilationFolder) $(DacPacFile) $(PublishProfileFolder) $(BeforeDeploymentScript)"></Exec>
  <Message Importance="High" Text="." />
</Target>
<Target Name="AfterRebuild">
  <Message Importance="High" Text="." />
  <Message Importance="High" Text="Execution of DacDeployer.exe in DEPLOY mode on AfterRebuild event:" />
  <Exec Command="$(DacDeployerExe) /deploy $(DacPacFile) $(PublishProfileFolder) $(BuildConfiguration) $(BeforeDeploymentScript)"></Exec>
  <Message Importance="High" Text="." />
</Target>
```
