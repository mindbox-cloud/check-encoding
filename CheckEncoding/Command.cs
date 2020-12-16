using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using Ude;

#pragma warning disable CS8618

namespace CheckEncoding
{
	internal abstract class Command
	{
		[Option('e', "target-encodings", Default = "utf-8,ASCII", Separator = ',')]
		public IEnumerable<string> TargetEncodings { get; set; }
	    
		[Option("extensions", Default = "txt,xml,json,config,cs,csproj,sln", Separator = ',')]
		public IEnumerable<string> FileExtensions { get; set; }

		[Option('d', "directory", Required = true)]
		public string TargetDirectory { get; set; }
	    
		public abstract CommandResult Execute();

		protected (string path, bool isValid, string encoding)[] GetFilesWithWrongEncoding()
		{
			var extensions = new HashSet<string>(
				FileExtensions.Select(extensionWithoutDot => "." + extensionWithoutDot), 
				StringComparer.OrdinalIgnoreCase);
		    
			var filesToCheck = Directory
				.EnumerateFiles(TargetDirectory, "*", SearchOption.AllDirectories)
				.Where(filePath => extensions.Contains(Path.GetExtension(filePath)));

			return filesToCheck
				.Select(file =>
				{
					var detector = new CharsetDetector();
					using var fileStream = File.Open(file, FileMode.Open);
					
					detector.Feed(fileStream);
					detector.DataEnd();

					if (detector.Charset == null)
					{
						throw new InvalidOperationException(
							$"Failed to fill charset detector with content from {file}. Check file content");
					}

					var isValidEncoding = TargetEncodings.Any(encoding =>
						detector.Charset.Equals(encoding, StringComparison.InvariantCultureIgnoreCase));
					
					return (path: file, isValid: isValidEncoding, encoding: detector.Charset);
				})
				.Where(file => !file.isValid)
				.ToArray();
		}
	}
}