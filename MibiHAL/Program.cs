using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MibiHAL
{
    class Program
    {
        static void Main(string[] args)
        {
            var b = new Brain();
            var r = new Random();

            var chainBuilder = new ChainBuilder(b);

            var set = Train(b, "train.txt");


            var words = set.ToArray();

            Console.WriteLine("Trained!");

            Console.WriteLine();

            var starterWord = words[r.Next(words.Length)];
            
            Console.WriteLine("Seed word: {0}", starterWord);
            Console.WriteLine();

            for (int i = 0; i < 10; i++)
            {
                var starter = new Chain(starterWord);

                var output = chainBuilder.BuildChain(starter);

                Console.WriteLine(Prettifier.Prettify(string.Join("", output.Symbols)));
                Console.WriteLine();
            }
        }

        private static IEnumerable<ISymbol> Train(Brain b, string trainFile)
        {
            var set = new HashSet<ISymbol>();
            var file = new StreamReader(trainFile);
            string line;
            while ((line = file.ReadLine()) != null)
            {
                if (line.StartsWith("#")) continue;

                var symbols = Parse.ToSymbols(line).ToArray();

                foreach (var symbol in symbols.Where(s => s is Word))
                {
                    set.Add(symbol);
                }

                b.Train(symbols, 3);
                b.Train(symbols, 5);
            }
            return set;
        }
    }
}
