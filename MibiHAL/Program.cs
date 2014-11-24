using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NDesk.Options;

namespace MibiHAL
{
    class Program
    {
        static void Main(string[] args)
        {
            var r = new Random();

            Brain b = null;
            ISymbol[] words;
            string brainFilename = "brain.brn";
            string trainFilename = String.Empty;
            string outputBrain = String.Empty;

            var o = new OptionSet()
            {
                {"b|brain=", v => brainFilename = v},
                {"t|train=", v => trainFilename = v},
                {"s|save=", v => outputBrain = v}
            };

            o.Parse(args);

            try
            {
                using (var brainFile = new FileStream(brainFilename, FileMode.Open))
                {
                    b = Brain.Load(brainFile);
                }
                Console.WriteLine("Brain loaded!");
            }
            catch (FileNotFoundException)
            {
                b = new Brain();
                Console.WriteLine("Empty brain created");
            }


            if (!String.IsNullOrEmpty(trainFilename))
            {
                var set = Train(b, trainFilename);
                Console.WriteLine("Trained!");
            }

            words = b.Symbols.ToArray();
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

            if (String.IsNullOrEmpty(outputBrain)) outputBrain = brainFilename;

            using (var saveBrain = new FileStream(outputBrain, FileMode.Create))
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

            try
            {
                b.Train(symbols, 3);
                b.Train(symbols, 5);
            }
            catch
            {
            }
        }
    }
}
