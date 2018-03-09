using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.String;

namespace DacDeployer.Helpers
{
    public static class Logger
    {

        private static readonly string LogFilePath;
        private static readonly StringBuilder LogBuffer;
        private static readonly Mode ExecutionMode ;
        public static bool IsQuiet;
        private static string DividerLine = "- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -";

        static Logger () 
        {
            LogFilePath = ConsoleAppArgsParser.GetParamValue("LogFile");

            if (!IsNullOrWhiteSpace(LogFilePath))
                LogBuffer = new StringBuilder();

            ExecutionMode = ModeDefiner.DefineMode();

            IsQuiet = ConsoleAppArgsParser.ParamExists("Quiet");
        }

        public static void Append (string message) 
        {
            LogBuffer?.Append(message);

            if (!IsQuiet)
                Console.Write(message);
        }

        public static void AppendLine(string message)
        {
            LogBuffer?.AppendLine(message);

            if (!IsQuiet)
                Console.WriteLine(message);
        }

        private static void AppendValueLine(StringBuilder messageSb, string name, string value, int nameAndGapLength)
        {
            messageSb.Append(name).Append(":").Append(new string(' ', nameAndGapLength - name.Length));
            
            if (value == null)
                messageSb.AppendLine("null");
            else if (value == Empty)
                messageSb.AppendLine("\"\"");
            else if (value.Equals("True", StringComparison.InvariantCultureIgnoreCase) || value.Equals("False", StringComparison.InvariantCultureIgnoreCase))
                messageSb.AppendLine(value);
            else
                messageSb.Append("\"").Append(value).AppendLine("\"");
        }

        public static void AppendEmptyLine()
        {
            AppendLine(" ");
        }

        public static void LogConsoleArguments()
        {
            var messageSb = new StringBuilder();

            messageSb.AppendLine("Console arguments passed");
            messageSb.AppendLine(DividerLine);

            var nameAndGapLength = "BeforeDeploymentScript".Length + 2;

            AppendValueLine(messageSb, "DacPacFile", ConsoleAppArgsParser.GetParamValue("DacPacFile"), nameAndGapLength);
            AppendValueLine(messageSb, "PublishProfileFolder", ConsoleAppArgsParser.GetParamValue("PublishProfileFolder"), nameAndGapLength);
            AppendValueLine(messageSb, "PublishProfileFile", ConsoleAppArgsParser.GetParamValue("PublishProfileFile"), nameAndGapLength);
            AppendValueLine(messageSb, "BuildConfiguration", ConsoleAppArgsParser.GetParamValue("BuildConfiguration"), nameAndGapLength);
            AppendValueLine(messageSb, "BeforeDeploymentScript", ConsoleAppArgsParser.GetParamValue("BeforeDeploymentScript"), nameAndGapLength);
            AppendValueLine(messageSb, "SqlCmdVariablesScript", ConsoleAppArgsParser.GetParamValue("SqlCmdVariablesScript"), nameAndGapLength);
            AppendValueLine(messageSb, "CompilationFolder", ConsoleAppArgsParser.GetParamValue("CompilationFolder"), nameAndGapLength);
            AppendValueLine(messageSb, "Compile", ConsoleAppArgsParser.ParamExists("Compile") ? "True" : "False", nameAndGapLength);
            AppendValueLine(messageSb, "Deploy", ConsoleAppArgsParser.ParamExists("Deploy") ? "True" : "False", nameAndGapLength);
            AppendValueLine(messageSb, "Quiet", ConsoleAppArgsParser.ParamExists("Quiet") ? "True" : "False", nameAndGapLength);
            AppendValueLine(messageSb, "LogFile", ConsoleAppArgsParser.GetParamValue("LogFile"), nameAndGapLength);

            messageSb.AppendLine(" ").AppendLine(" ");

            Logger.Append(messageSb.ToString());
        }

        public static void LogResolvedPaths(
            string dacPacFolderPath = null,
            string dacPacFilePath = null,
            string publishProfileFolderPath = null,
            string publishProfileFilePath = null,
            string beforeDeploymentScriptPath = null,
            string sqlCmdVariablesScriptPath = null
        )
        {
            string dacDeployerExeFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            var messageSb = new StringBuilder();

            messageSb.AppendLine("Resolved and confirmed paths");
            messageSb.AppendLine(DividerLine);

            var nameAndGapLength = "BeforeDeploymentScriptPath".Length + 2;
            
            AppendValueLine(messageSb, "DeployerExeFolder", dacDeployerExeFolder, nameAndGapLength);
            AppendValueLine(messageSb, "DacPacFolderPath", dacPacFolderPath, nameAndGapLength);
            AppendValueLine(messageSb, "DacPacFilePath", dacPacFilePath, nameAndGapLength);
            AppendValueLine(messageSb, "PublishProfileFolderPath", publishProfileFolderPath, nameAndGapLength);

            if (ExecutionMode == Mode.Deploy)
            {
                AppendValueLine(messageSb, "PublishProfileFilePath", publishProfileFilePath, nameAndGapLength);
                AppendValueLine(messageSb, "BeforeDeploymentScriptPath", beforeDeploymentScriptPath, nameAndGapLength);
                AppendValueLine(messageSb, "sqlCmdVariablesScriptPath", sqlCmdVariablesScriptPath, nameAndGapLength);
            }

            messageSb.AppendLine(" ").AppendLine(" ");

            Logger.Append(messageSb.ToString());
        }

        public static void LogParsedPublishProfile(PublishProfile publishProfile)
        {
            var messageSb = new StringBuilder();

            messageSb.AppendLine("PublishProfile parsed properties");
            messageSb.AppendLine(DividerLine);
            
            var nameAndGapLength = "IntegratedSecurity".Length + 2;

            AppendValueLine(messageSb, "ConnectionString", publishProfile.ConnectionString, nameAndGapLength);
            AppendValueLine(messageSb, "DatabaseName", publishProfile.DatabaseName, nameAndGapLength);
            AppendValueLine(messageSb, "ServerName", publishProfile.ServerName, nameAndGapLength);
            AppendValueLine(messageSb, "IntegratedSecurity", publishProfile.IntegratedSecurity ? "True" : "False", nameAndGapLength);
            AppendValueLine(messageSb, "UserID", publishProfile.UserID, nameAndGapLength);
            AppendValueLine(messageSb, "Password", publishProfile.Password, nameAndGapLength);

            messageSb.AppendLine(" ").AppendLine(" ");

            Logger.Append(messageSb.ToString());
        }

        public static void OutputLogToFile()
        {
            if (LogFilePath == null)
                return;

            var logFolderPath = Path.GetDirectoryName(LogFilePath);
            var logFileName = Path.GetFileNameWithoutExtension(LogFilePath);
            var logFileExtension = Path.GetExtension(LogFilePath);

            var tempLogFileName = logFileName;
            var newLogFilePath = Path.Combine(logFolderPath, $"{tempLogFileName}{logFileExtension}");
            var counter = 2;

            while (File.Exists(newLogFilePath))
            {
                tempLogFileName = $"{logFileName}({counter})";
                newLogFilePath = Path.Combine(logFolderPath, $"{tempLogFileName}{logFileExtension}");
                counter++;
            }

            File.WriteAllText(newLogFilePath, LogBuffer.ToString());
        }
    }
}

        //public static void LogPublishProfilePaths (string publishProfileFolder, string publishProfileFile, string buildConfiguration, string chosenPublishProfileFile) 
        //{
        //    //var messageSb = new StringBuilder();

        //    //messageSb.AppendLine("PublishProfile paths");
        //    //messageSb.AppendLine(DeviderLine);

        //    //messageSb.AppendLine($"PublishProfileFolder:       \"{publishProfileFolder}\"");
           
        //    //if (ExecutionMode == Mode.Deploy)
        //    //{
        //    //    messageSb.AppendLine($"PublishProfileFile:         {(publishProfileFile == null ? "null" : $"\"{publishProfileFile}\"")}");
        //    //    messageSb.AppendLine($"BuildConfiguration:         {(buildConfiguration == null ? "null" : $"\"{buildConfiguration}\"")}");

        //    //    messageSb.AppendLine($"Chosen PublishProfileFile:  \"{chosenPublishProfileFile}\"");
        //    //}

        //    //messageSb.AppendLine(" ").AppendLine(" ");

        //    //LogMessage(messageSb.ToString());
        //}

        //public static void LogBeforeDeploymentPaths(string beforeDeploymentScript, string sqlCmdVariablesScript)
        //{
        //    //var messageSb = new StringBuilder();

        //    //messageSb.AppendLine("BeforeDeployment paths");
        //    //messageSb.AppendLine(DeviderLine);

        //    //messageSb.AppendLine($"BeforeDeploymentScript:  {(beforeDeploymentScript == null ? "null" : $"\"{beforeDeploymentScript}\"")}");
        //    //messageSb.AppendLine($"SqlCmdVariablesScript:   {(sqlCmdVariablesScript == null ? "null" : $"\"{sqlCmdVariablesScript}\"")}");

        //    //messageSb.AppendLine(" ").AppendLine(" ");

        //    //LogMessage(messageSb.ToString());
        //}