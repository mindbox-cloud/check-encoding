using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
				Console.WriteLine($"The encoding of following files is different from target {TargetEncoding}");
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
	
	[Verb("fix")]
	internal class FixCommand : Command
	{
		public override CommandResult Execute()
		{
			var filesWithWrongEncoding = GetFilesWithWrongEncoding();

			var targetEncoding = Encoding.GetEncoding(TargetEncoding);
			
			foreach (var (path, _, encoding) in filesWithWrongEncoding)
			{
				Console.WriteLine($"Fixing file: {path}");
				
				var sourceEncoding = Encoding.GetEncoding(encoding);
				string data;
				using (var fileStream = File.OpenRead(path))
				using (var fileReader = new StreamReader(fileStream, sourceEncoding, true))
					data = fileReader.ReadToEnd();

				using (var fileStream = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None))
				using (var fileWriter = new StreamWriter(fileStream, targetEncoding))
					fileWriter.Write(data);
			}
			
			return CommandResult.Success;
		}
	}
}