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
            string line;
            var set = new HashSet<ISymbol>();
            var b = new Brain();
            var file = new StreamReader("train.txt");
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

            var r = new Random();

            var words = set.ToArray();

            Console.WriteLine("Trained!");

            var starterWord = words[r.Next(words.Length)];
            for (int i = 0; i < 10; i++)
            {
                var starter = new Chain(starterWord);

                var output = starter.First(1);

                do
                {
                    var candidates = b.Candidates(starter);

                    var chains = candidates as Chain[] ?? candidates.ToArray();

                    var scores = chains.Select(b.Score).ToArray();
                    var total = scores.Aggregate(0, (i1, i2) => i1 + i2);

                    var c = r.Next(total);

                    var s = 0;
                    Chain chain = null;
                    for (int j = 0; j < scores.Count(); j++)
                    {
                        s += scores[j];
                        if (s > c)
                        {
                            chain = chains[j];
                            break;
                        }
                    }
                    if (chain == null) chain = chains.Last();


                    output = output.Join(chain.Last(chain.Length - 1));

                    starter = chain.Last(1);

                } while (!starter.Symbols.Last().Terminal);



                Console.WriteLine(string.Join("", output.Symbols));


            }
        }
    }
}
