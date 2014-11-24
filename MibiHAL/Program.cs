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
            var r = new Random();

            Brain b;
            ISymbol[] words;

            try
            {
                using (var brainFile = new FileStream("brain.brn", FileMode.Open))
                {
                    b = Brain.Load(brainFile);
                    words = b.Symbols.ToArray();
                }
                Console.WriteLine("Brain loaded!");
            }
            catch (FileNotFoundException)
            {
                b = new Brain();
                var set = Train(b, "train.txt");
                words = set.ToArray();
                Console.WriteLine("Trained!");
            }

            var chainBuilder = new ChainBuilder(b);


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

            using (var saveBrain = new FileStream("brain.brn", FileMode.Create))
            {
                b.Save(saveBrain);
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

                LearnLine(b, line, set);
            }
            return set;
        }

        private static void LearnLine(Brain b, string line, ISet<ISymbol> words)
        {
            var symbols = Parse.ToSymbols(line).ToArray();

            foreach (var symbol in symbols.Where(s => s is Word))
            {
                words.Add(symbol);
            }

            b.Train(symbols, 3);
            b.Train(symbols, 5);
        }
    }
}
