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

            var next = starter;

            var output = starter.First(1);

            do
            {
                var forwardCandidates = m_Brain.ForwardCandidates(next);

                var chain = PickChain(forwardCandidates);

                if (chain == null) break;

                output = output.Join(chain.Last(chain.Length - 1));

                next = chain.Last(1);
            
            } while (!next.Symbols.Last().Terminal);

            next = starter;
            do
            {
                var backwardCandidates = m_Brain.BackwardsCandidates(next);

                var chain = PickChain(backwardCandidates);
                
                if (chain == null) break;

                output = chain.First(chain.Length - 1).Join(output);

                next= chain.First(1);
            
            } while (!next.Symbols.First().Starter);

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

            return chain ?? chains.LastOrDefault();
        }
    }
}