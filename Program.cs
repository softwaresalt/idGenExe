using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Arguments;
using System.IO;
using System.Linq;

namespace IDGen
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			ParserOptions options = new ParserOptions(true, "--", "-", "=");
			Parser parser = new Parser(options);
			NamedArgument<int> count = parser.CreateNamedArgument<int>("gen-count", "gc", "Gencount", "The count of IDs to generate; default=100.", 100, null);
			NamedArgument<string> o = parser.CreateNamedArgument<string>("output", "o", "Ouput", "The file location to output the results of the ID generator; will append to existing file.", default, null);
			FlagArgument<bool> overWrite = parser.CreateFlagArgument<bool>("create", "c", "Create", "Create the output file; will overwrite existing file.");
			FlagArgument<bool> help = parser.CreateFlagArgument<bool>("help", "h", "Help", "Output parameter options to app.");
			parser.AddNamedArgument(count).AddNamedArgument(o).AddFlagArgument(overWrite).AddFlagArgument(help);
			FileInfo file = new FileInfo(Environment.GetCommandLineArgs()[0]);
			List<string> parms = new List<string>();
			parms.Add(file.Name.Replace(file.Extension, ".exe")); //First command is the application itself.
			parms.AddRange(args);
			try
			{
				ParsingResults pr = parser.Parse(parms);

				if (pr.HasParsedValue(help.Destination) && pr.GetParsedValue<bool>(help.Destination))
				{
					writeHelpInfo(parser);
					return;
				}

				int gc = pr.GetParsedValue<int>(count.Destination);

				//IdGenerator initializer can be improved to include custom IdGeneratorOptions.
				IdGenerator idGen = new IdGenerator(0);
				IEnumerable<long> ids = idGen.Take(gc);

				if (pr.HasParsedValue(o.Destination))
				{
					writeToFile(ids, pr.GetParsedValue<string>(o.Destination), pr.GetParsedValue<bool>(overWrite.Destination));
				}
				else
				{
					foreach (long id in ids) { Console.WriteLine(id); }
				}
			}
			catch (Exception e)
			{
				Console.WriteLine($"{e.GetType()}: {e.Message}");
				writeHelpInfo(parser);
			}
		}

		private static void writeHelpInfo(Parser parser)
		{
			if (parser.NamedArguments.Any()) { Console.WriteLine("Arguments"); }
			foreach (Argument arg in parser.NamedArguments)
			{
				Console.WriteLine($"\t--{arg.Name} -{arg.Alias}\t\t: {arg.Help}");
			}
			if (parser.NamedArguments.Any())
			{
				Console.WriteLine();
				Console.WriteLine(@"Example: idGenExe.exe -gc=100 -o=C:\temp\ids.txt -ow");
				Console.WriteLine();
			}

			if (parser.PositionalArguments.Any()) { Console.WriteLine("PositionalArguments"); }
			foreach (Argument arg in parser.PositionalArguments)
			{
				Console.WriteLine($"\t--{arg.Name} -{arg.Alias}\t\t: {arg.Help}");
			}

			if (parser.FlagArguments.Any()) { Console.WriteLine("Flags"); }
			foreach (Argument arg in parser.FlagArguments)
			{
				Console.WriteLine($"\t--{arg.Name} -{arg.Alias}\t\t: {arg.Help}");
			}

			if (parser.Commands.Any()) { Console.WriteLine("Commands"); }
			foreach (Command cmd in parser.Commands)
			{
				Console.WriteLine($"\t--{cmd.Name} -{cmd.Alias}");
			}
		}

		private static void writeToFile(IEnumerable<long> ids, string path, bool overwrite = false)
		{
			FileInfo info = new FileInfo(path);
			if (info.Directory.Exists)
			{
				if (!info.Exists) { File.AppendAllText(path, String.Empty); }
				if (overwrite)
				{
					using (StreamWriter writer = info.CreateText())
					{
						foreach (long id in ids)
						{
							writer.WriteLine(id);
						}
					}
				}
				else
				{
					using (StreamWriter writer = info.AppendText())
					{
						foreach (long id in ids)
						{
							writer.WriteLine(id);
						}
					}
				}
			}
			else
			{
				throw new InvalidOperationException($"The file and path ({path}) do not exist.");
			}
		}
	}
}