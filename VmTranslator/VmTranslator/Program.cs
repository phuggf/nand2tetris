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
            if (args.Count() != 2)
            {
                throw new ArgumentException("Must provide two command line arguments");
            }

            string inputFileName = args[0];
            string outputFileName = args[1];

            string[] commands = File.ReadAllLines(inputFileName);

            var parser = new Parser(commands);
            var writer = new CodeWriter();
            var translator = new AssemblyWriter(parser, writer);

            File.WriteAllLines(args[1], translator.GetAssembly());
            Console.WriteLine($"Successfully created assembly file {outputFileName}");
            Console.ReadKey();
        }
    }
}
