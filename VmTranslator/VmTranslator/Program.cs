using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VmTranslator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Count() < 2)
            {
                throw new ArgumentException("Must provide at least two command line arguments");
            }

            var inputFiles = args.Take(args.Length - 1);
            string outputFileName = args.Last();
            List<string> assembly = new List<string>();

            var writer = new CodeWriter();
            assembly.AddRange(writer.WriteInit());

            foreach (var file in inputFiles)
            {
                string[] commands = File.ReadAllLines(file);

                var parser = new Parser(commands);
                writer.CurrentVmFile = file.Split('\\').Last().Split('.')[0];
                var translator = new AssemblyWriter(parser, writer);

                assembly.AddRange(translator.GetAssembly());
            }

            File.WriteAllLines(outputFileName, assembly);
            Console.WriteLine($"Successfully created assembly file {outputFileName}");
            Console.ReadKey();
        }
    }
}
