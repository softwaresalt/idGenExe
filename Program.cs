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
			NamedArgument<int> count = new NamedArgument<int>("gen-count", "gc", "Gencount", "The count of IDs to generate; default=100.", 100, null);
			NamedArgument<string> o = new NamedArgument<string>("output", "o", "Ouput", "The file location to output the results of the ID generator.", default, null);
			FlagArgument<bool> help = new FlagArgument<bool>("help", "h", "Help", "Output parameter options to app.");
			(parser.NamedArguments as List<Argument>).Add(count);
			(parser.NamedArguments as List<Argument>).Add(o);
			(parser.FlagArguments as List<Argument>).Add(help);
			List<string> parms = new List<string>();
			string exe = Environment.GetCommandLineArgs()[0];
			FileInfo info = new FileInfo(exe);
			parms.Add(info.Name.Replace(info.Extension, ".exe")); //Parser expects first argument to be the name of the executable.
			if (parms.Count == 0 || (!(args.Any(x => x.Contains("-gc")) || args.Any(x => x.Contains("--gen-count"))) && !(args.Contains("-h") || args.Contains("--help"))))
			{
				parms.Add($"-{count.Alias}");
			}
			parms.AddRange(args.ToList());
			ParsingResults parsingResults = parser.Parse(parms.ToArray());
			if (parsingResults.ParsedValues.ContainsKey(help.Destination) && (bool)parsingResults.ParsedValues[help.Destination])
			{
				writeHelpInfo(parser);
				return;
			}

			int gc = parsingResults.GetParsedValue<int>(count.Destination);

			//IdGenerator initializer can be improved to include custom IdGeneratorOptions.
			IdGenerator idGen = new IdGenerator(0);
			IEnumerable<long> ids = idGen.Take(gc);

			if (parsingResults.ParsedValues.ContainsKey(o.Destination) && parsingResults.ParsedValues[o.Destination] != null)
			{
				writeToFile(ids, parsingResults.GetParsedValue<string>(o.Destination));
			}
			else
			{
				foreach (long id in ids) { Console.WriteLine(id); }
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
				Console.WriteLine(@"Example: idGenExe.exe -gc=100 -o=C:\temp\ids.txt");
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

		private static void writeToFile(IEnumerable<long> ids, string path)
		{
			FileInfo info = new FileInfo(path);
			if (info.Directory.Exists)
			{
				if (!info.Exists) { File.AppendAllText(path, String.Empty); }
				using (StreamWriter writer = info.AppendText())
				{
					foreach (long id in ids)
					{
						writer.WriteLine(id);
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