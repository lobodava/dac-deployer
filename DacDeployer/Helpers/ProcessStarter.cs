using System;
using System.Diagnostics;
using System.Text;
using static System.String;

namespace DacDeployer.Helpers
{
	public static class ProcessStarter
	{
		// --http://csharptest.net/532/using-processstart-to-capture-console-output/
		// --http://social.msdn.microsoft.com/Forums/vstudio/en-US/8652fae3-819a-4cde-8cc2-70d45c15087c/vs2005-addin-write-asynchronous-output-from-batch-to-outputwindowpane?forum=vsx

		public static bool StartProcess(string workingDirectory, string fileName, string[] arguments, string successMessage = null, string failMessage = null)
		{
			var p = new System.Diagnostics.Process
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

			p.OutputDataReceived += (s, a) => Console.WriteLine(a.Data);// , workingDirectory);
			//p.ErrorDataReceived += (s, a) => ErrorWriteLine(a.Data, workingDirectory);
			p.ErrorDataReceived += (s, a) => Console.WriteLine(a.Data);// , workingDirectory);

			p.Start();

			p.BeginOutputReadLine();
			p.BeginErrorReadLine();
			p.StandardInput.Close();

			p.WaitForExit();

			var exitCode = p.ExitCode;

			p.Close();

			if (exitCode == 0)
			{
				if (!String.IsNullOrWhiteSpace(successMessage))
				{
					Console.WriteLine(successMessage);
				}

				return true;
			}

			if (!String.IsNullOrWhiteSpace(failMessage))
			{
				Console.WriteLine(failMessage);
				Console.WriteLine("All the rest operations were cancelled.");
				Console.WriteLine("Database deploy failed. See Visual Studio Output Window for details.");
			}

			return false;
		}

	}
}
