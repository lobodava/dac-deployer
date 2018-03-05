using System;
using System.Text;
using static System.String;

namespace DeployDatabase.Helpers
{
	public static class ConsoleAppArgsParser
	{
		public static string GetParamValue(string paramName, bool required = false)
		{
			var commandLine = Environment.CommandLine;
			
			(var index, var paramString) = GetParamIndexAndString(commandLine, paramName);
			
			if (index <= 0) 
			{
				if (required) 
					throw new ArgumentException($"Required console application argument \"{paramName}\" is not provided.");
				return null;
			}

			var quoted = false;
			var sb = new StringBuilder();

			for (var i = index + paramString.Length; i < commandLine.Length; i++)
			{
				if (i == index + paramString.Length)
				{
					if (commandLine[i] == '\"')
						quoted = true;
					else
						sb.Append(commandLine[i]);
				}
				else if (quoted && commandLine[i] == '\"' || !quoted && commandLine[i] == ' ')
					break;
				else
					sb.Append(commandLine[i]);
			}

			var value = sb.ToString();

			if (required && IsNullOrWhiteSpace(value)) 
				throw new ArgumentException($"Required console application argument \"{paramName}\" has no value.");

			return value;
		}


		public static bool ParamExists(string paramName) 
		{
			var commandLine = Environment.CommandLine;
			
			return GetParamIndexAndString(commandLine, paramName).paramIndex > -1;
		}

		private static (int paramIndex, string paramString) GetParamIndexAndString(string commandLine, string paramName) {

			int paramIndex = -1;
			string paramString = Empty;
			string[] prefixes = {" ", "/", "-", "--" };
			string[] postfixes = {" ", ":", "="};

			foreach (var prefix in prefixes)
			{
				foreach (var postfix in postfixes)
				{
					paramString = $"{prefix}{paramName}{postfix}";
					paramIndex = commandLine.IndexOf(paramString, StringComparison.InvariantCultureIgnoreCase);
					if (paramIndex < 0) 
						paramString = Empty;

					if (paramString.Length > 0)
						break;
				}

				if (paramString.Length > 0)
					break;
			}
			
			return (paramIndex, paramString);
		}

	}
}
