using System;
using System.Text;
using CommandLine;

namespace CheckEncoding
{
    class Program
    {
	    static Program()
	    {
		    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
	    }
	    
        static void Main(string[] args)
        {
	        var commands = new[]
	        {
		        typeof(CheckCommand),
		        typeof(FixCommand)
	        };

	        CommandResult result;

	        var argsParsingResult = Parser.Default.ParseArguments(args, commands);
	        if (argsParsingResult is Parsed<object> parsed && parsed.Value is Command command)
	        {
		        result = command.Execute();
	        }
	        else
	        {
		        result = CommandResult.Error;
	        }

	        Environment.Exit((int)result);
        }
    }
}
