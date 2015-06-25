using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace cs2ts
{
    class Program
    {
        public static void Main(params string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("missing file argument");
                return;
            }

            var visitor = new Transpiler(File.ReadAllText(args[0]));

            var output = visitor.Output();
            var outputFileName = args.Length > 1 ? args[1] : Path.ChangeExtension(args[0], "ts");

            File.WriteAllText(outputFileName, output);
        }
    }
}
