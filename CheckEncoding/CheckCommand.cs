using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using Ude;

namespace CheckEncoding
{
	[Verb("check")]
	internal class CheckCommand : Command
	{
		public override CommandResult Execute()
		{
			var filesWithWrongEncoding = GetFilesWithWrongEncoding();

			if (filesWithWrongEncoding.Any())
			{
				Console.WriteLine($"The encoding of following files is different from target {TargetEncodings}");
				foreach (var (path, _, encoding) in filesWithWrongEncoding)
				{
					Console.Error.WriteLine($"File: {path}, encoding: {encoding}");   
				}
			    
				return CommandResult.Error;
			}
			else
			{
				return CommandResult.Success;
			}
		}
	}
}