using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Microsoft.SqlServer.Dac;
using static System.String;

namespace DacDeployer.Helpers
{
    public static class DeploymentSteps
    {

        public static bool ExecuteBeforeDeployment(string beforeDeploymentScript, PublishProfile publishProfile)
        {
            if (IsNullOrWhiteSpace(beforeDeploymentScript))
            {
                Logger.AppendLine("Execution of BeforeDeploymentScript is skipped because the path to the script file is not provided.");
                Logger.AppendEmptyLine();
                return true;
            }

            if (!DatabaseExists(publishProfile))
            {
                Logger.AppendLine("Execution of BeforeDeploymentScript is skipped because the target database does not exist yet.");
                Logger.AppendEmptyLine();
                return true;
            }

            var beforeDeploymentWorkingDirectory = Path.GetDirectoryName(beforeDeploymentScript);
            var beforeDeploymentScriptName = Path.GetFileName(beforeDeploymentScript);
            
            var arguments = new List<string>
            {
                $"-S {publishProfile.ServerName}",
                $"-i \"{beforeDeploymentScriptName}\""
            };
            
            if (publishProfile.SqlCmdVariables.Count > 0) {
                var variables = Join(" ", publishProfile.SqlCmdVariables.Select(kv => $"{kv.Key}=\"{kv.Value}\""));
                arguments.Add($"-v {variables}");
            }

            Logger.AppendLine("Execution of the BeforeDeploymentScript by the SqlCmd utility is started with the following command:");
            Logger.AppendLine($"sqlcmd.exe {Join(" ", arguments)}");
            Logger.AppendEmptyLine();

            var isSuccess = ProcessStarter.StartProcess(beforeDeploymentWorkingDirectory, "sqlcmd", arguments);

            if (isSuccess)
            {
                Logger.AppendLine("Execution of the BeforeDeploymentScript by the SqlCmd utility completed successfully.");
                Logger.AppendEmptyLine();
            }
            else
            {
                Logger.AppendLine("Execution of the BeforeDeploymentScript by the SqlCmd utility failed.");
                Logger.AppendLine("All the rest operations are cancelled.");
                Logger.AppendEmptyLine();
            }

            return isSuccess;
        }

        public static bool DatabaseExists(PublishProfile publishProfile)
        {
            using (SqlConnection conn = new SqlConnection(publishProfile.ConnectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = $"select case when db_id('{publishProfile.DatabaseName}') is null then 0 else 1 end";

                conn.Open();

                return (int)cmd.ExecuteScalar() == 1;
            }
        }


        public static void DeployDacPac(string dacPacFile, PublishProfile publishProfile)
        {
            // https://blogs.msmvps.com/deborahk/deploying-a-dacpac-with-dacfx-api/
            // https://stackoverflow.com/questions/10438258/using-microsoft-build-evaluation-to-publish-a-database-project-sqlproj

            string getMessage(DacMessage m)
            {
                if (m.Number == 0)
                    return m.Message;

                return $"{m.Number}: {m.Message}";
            }

            var dacService = new DacServices(publishProfile.ConnectionString);
            dacService.Message += (s, e) => Logger.AppendLine(getMessage(e.Message));
            //dacService.ProgressChanged += (s,e) => Logger.AppendLine($"{e.Status}: {e.Message}");

            try
            {
                using (DacPackage dacpac = DacPackage.Load(dacPacFile))
                {
                    Logger.AppendLine("Deployment of DacPac with DacFx API is started:");
                    Logger.AppendEmptyLine();

                    dacService.Deploy(dacpac, publishProfile.DatabaseName, upgradeExisting: true, options: publishProfile.DacDeployOptions);
                }

                Logger.AppendEmptyLine();
                Logger.AppendLine("Deployment of DacPac with DacFx API completed successfully.");

            }
            catch (Exception ex)
            {
                Logger.AppendEmptyLine();
                Logger.AppendLine("Deployment of DacPac with DacFx API failed.");
                Logger.AppendLine("The exception message is:");
                Logger.AppendLine(ex.Message);
            }
        }
    
    }
}
