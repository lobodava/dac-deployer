BeforeDeploymentScriptPath
SqlCmdVariablesScriptPath
PublishProfileFolderPath
PublishProfileFilePath
DacPacFilePath


BeforeDeploymentScript
SqlCmdVariablesScript
PublishProfileFolder
PublishProfileFile
DacPacFile



/compile /DacPacFile:"C:\Current Projects\DacDeployer\TestDb\bin\Debug\TestDb.dacpac" /PublishProfileFolder:"C:\Current Projects\DacDeployer\TestDb\PublishProfiles" /BeforeDeploymentScript:".\0. Before Deployment\_BeforeDeployment.script.sql" 

/deploy /DacPacFile:"C:\Current Projects\DacDeployer\TestDb\bin\Debug\TestDb.dacpac" /PublishProfileFolder:"C:\Current Projects\DacDeployer\TestDb\PublishProfiles" /BuildConfiguration:"Debug" /BeforeDeploymentScript:".\0. Before Deployment\_BeforeDeployment.script.sql" 