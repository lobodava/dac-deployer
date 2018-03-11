using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using static System.String;

namespace DacDeployer.Helpers
{
	public static class ProcessStarter
	{
		// --http://csharptest.net/532/using-processstart-to-capture-console-output/
		// --http://social.msdn.microsoft.com/Forums/vstudio/en-US/8652fae3-819a-4cde-8cc2-70d45c15087c/vs2005-addin-write-asynchronous-output-from-batch-to-outputwindowpane?forum=vsx

		public static bool StartProcess(string workingDirectory, string fileName, IEnumerable<string> arguments)
		{
            var p = new Process
			{
				StartInfo = new ProcessStartInfo() 
				{
					UseShellExecute = false,
					RedirectStandardError = true,
					RedirectStandardOutput = true,
					RedirectStandardInput = true,
					StandardOutputEncoding = Encoding.GetEncoding(866),
					WindowStyle = ProcessWindowStyle.Hidden,
					CreateNoWindow = true,
					ErrorDialog = false,
					WorkingDirectory = workingDirectory,
					FileName = fileName,
					Arguments = Join(" ", arguments) //  EscapeArguments(arguments) 
				}
			};

			p.OutputDataReceived += (s, a) => Logger.AppendLine(a.Data);// , workingDirectory);
			p.ErrorDataReceived += (s, a) => Logger.AppendLine(a.Data);// , workingDirectory);

			p.Start();

			p.BeginOutputReadLine();
			p.BeginErrorReadLine();
			p.StandardInput.Close();

			p.WaitForExit();

			var exitCode = p.ExitCode;

			p.Close();

            return exitCode == 0;

		}

	}
}
