using System;
using System.Collections.Generic;
using System.Linq;

namespace MibiHAL
{
    public class Brain
    {
        private readonly Dictionary<Chain, int> m_Chains = new Dictionary<Chain, int>(); 

        public Brain()
        {
        }

        private void Add(Chain c)
        {
            if (!m_Chains.ContainsKey(c))
            {
                m_Chains[c] = 0;
            }

            m_Chains[c]++;
        }

        private readonly Dictionary<Chain, IEnumerable<Chain>> m_StartsWith = new Dictionary<Chain, IEnumerable<Chain>>(); 
        private readonly Dictionary<Chain, IEnumerable<Chain>> m_EndsWith = new Dictionary<Chain, IEnumerable<Chain>>(); 

        public IEnumerable<Chain> ForwardCandidates(Chain start)
        {
            if (!m_StartsWith.ContainsKey(start))
            {
                m_StartsWith[start] = Chains().Where(c => c.StartsWith(start));
            }

            return m_StartsWith[start];
        }

        public IEnumerable<Chain> BackwardsCandidates(Chain end)
        {
            if (!m_EndsWith.ContainsKey(end))
            {
                m_EndsWith[end] = Chains().Where(c => c.EndsWith(end));
            }

            return m_EndsWith[end];
        }

        public IEnumerable<Chain> Chains()
        {
            return m_Chains.Keys;
        }

        public void Train(IEnumerable<ISymbol> symbols, int order = 3)
        {
            var chain = new Chain(symbols);

            if(chain.Length < order)
                throw new ArgumentException("order");

            for (var offset = 0; offset < chain.Length - 1; offset++)
            {
                var sliceSize = order < chain.Length - offset ? order : chain.Length - offset;
                var slice = chain.Slice(offset, sliceSize);
                Add(slice);

                // Warm the candidates cache
                foreach (var count in Enumerable.Range(0, order - 1))
                {
                    ForwardCandidates(slice.First(count));
                    BackwardsCandidates(slice.Last(count));
                }
            }
        }

        public int Score(Chain chain)
        {
            if (!m_Chains.ContainsKey(chain))
                throw new KeyNotFoundException();

            return m_Chains[chain];
        }

    }
}