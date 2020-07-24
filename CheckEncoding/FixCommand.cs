using System;
using System.IO;
using System.Linq;
using System.Text;
using CommandLine;

namespace CheckEncoding
{
	[Verb("fix")]
	internal class FixCommand : Command
	{
		public override CommandResult Execute()
		{
			var filesWithWrongEncoding = GetFilesWithWrongEncoding();

			var targetEncodings = TargetEncodings.Select(Encoding.GetEncoding).ToArray();
			
			foreach (var (path, _, encoding) in filesWithWrongEncoding)
			{
				Console.WriteLine($"Fixing file: {path}");
				
				var sourceEncoding = Encoding.GetEncoding(encoding);
				string data;
				using (var fileStream = File.OpenRead(path))
				using (var fileReader = new StreamReader(fileStream, sourceEncoding, true))
					data = fileReader.ReadToEnd();

				using (var fileStream = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None))
				using (var fileWriter = new StreamWriter(fileStream, targetEncodings.First()))
					fileWriter.Write(data);
			}
			
			return CommandResult.Success;
		}
	}
}