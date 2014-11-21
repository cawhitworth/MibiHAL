using System;
using System.Collections.Generic;
using System.Linq;

namespace MibiHAL
{
    class ChainBuilder
    {
        private readonly Brain m_Brain;
        private readonly Random m_Random = new Random();

        public ChainBuilder(Brain brain)
        {
            m_Brain = brain;
        }

        public Chain BuildChain(Chain starter)
        {
            var output = starter.First(1);

            do
            {
                var candidates = m_Brain.ForwardCandidates(starter);

                var chain = PickChain(candidates);

                output = output.Join(chain.Last(chain.Length - 1));

                starter = chain.Last(1);
            
            } while (!starter.Symbols.Last().Terminal);

            return output;
        }

        private Chain PickChain(IEnumerable<Chain> candidates)
        {
            var chains = candidates as Chain[] ?? candidates.ToArray();
            var scores = chains.Select(m_Brain.Score).ToArray();
            var total = scores.Aggregate(0, (i1, i2) => i1 + i2);

            var c = m_Random.Next(total);

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
            return chain;
        }
    }
}