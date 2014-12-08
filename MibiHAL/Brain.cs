using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

namespace MibiHAL
{
    public class Brain
    {
        private readonly Dictionary<Chain, int> m_Chains = new Dictionary<Chain, int>(); 

        public Brain()
        {
        }

        public Dictionary<int, int> Scores
        {
            get
            {
                var result = new Dictionary<int, int>();
                foreach (var chain in m_Chains)
                {
                    var score = chain.Value;
                    if (!result.ContainsKey(score))
                    {
                        result[score] = 0;
                    }
                    result[score]++;
                }
                return result;
            }
        }

        public IEnumerable<ISymbol> Symbols
        {
            get
            {
                var hashSet = new HashSet<ISymbol>();
                foreach (var chain in m_Chains.Keys)
                {
                    foreach (var symbol in chain.Symbols)
                    {
                        hashSet.Add(symbol);
                    }
                }
                return hashSet;
            }
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
            return m_Chains[chain];
        }

        #region Save/Load
        [Flags]
        private enum Flags
        {
            Word = 0x01,
            Starter = 0x02,
            Terminal = 0x04,
        };

        public void Save(Stream output)
        {
            using (var compressionStream = new DeflateStream(output, CompressionMode.Compress, true))
            {

                // Build a dictionary of symbols
                var arr = new List<ISymbol>();
                var dict = new Dictionary<ISymbol, int>();

                foreach (var chain in m_Chains.Keys)
                {
                    foreach (var symbol in chain.Symbols)
                    {
                        if (!dict.ContainsKey(symbol))
                        {
                            arr.Add(symbol);
                            dict[symbol] = arr.Count;

                        }
                    }
                }

                // Write out symbols
                foreach (var symbol in arr)
                {
                    int index = dict[symbol];
                    WriteInt(compressionStream, index);
                    var bytes = Encoding.UTF8.GetBytes(symbol.Value);
                    compressionStream.Write(bytes, 0, bytes.Length);
                    compressionStream.WriteByte(0);
                    var flags = (symbol is Word ? Flags.Word : 0) |
                                (symbol.Starter ? Flags.Starter : 0) |
                                (symbol.Terminal ? Flags.Terminal : 0);
                    compressionStream.WriteByte((byte) flags);
                }

                WriteInt(compressionStream, 0);

                // Write out chains
                foreach (var chain in m_Chains)
                {
                    foreach (var s in chain.Key.Symbols)
                    {
                        WriteInt(compressionStream, dict[s]);
                    }
                    WriteInt(compressionStream, 0);
                    WriteInt(compressionStream, chain.Value);
                }

                WriteInt(compressionStream, 0);
            }
        }

        private static void WriteInt(Stream s, int i)
        {
            s.WriteByte((byte)(i & 0xff));
            s.WriteByte((byte)((i >> 8) & 0xff));
            s.WriteByte((byte)((i >> 16) & 0xff));
            s.WriteByte((byte)((i >> 24) & 0xff));
        }

        private static int ReadInt(Stream s)
        {
            var index = 0;
            index |= s.ReadByte();
            index |= (s.ReadByte() << 8);
            index |= (s.ReadByte() << 16);
            index |= (s.ReadByte() << 24);
            return index;
        }

        public static Brain Load(Stream input)
        {
            var brain = new Brain();

            using (var decompressionStream = new DeflateStream(input, CompressionMode.Decompress, true))
            {
                var symbolDict = new Dictionary<int, ISymbol>();
                var decoder = new UTF8Encoding();

                var bytes = new byte[1024];
                do
                {
                    var index = ReadInt(decompressionStream);
                    if (index == 0) break;

                    var b = decompressionStream.ReadByte();
                    var idx = 0;
                    while (b != 0)
                    {
                        if (b != 0) bytes[idx++] = (byte) b;
                        b = decompressionStream.ReadByte();
                    }
                    var value = decoder.GetString(bytes, 0, idx);

                    var flags = (Flags) decompressionStream.ReadByte();
                    ISymbol symbol;
                    if (flags.HasFlag(Flags.Word))
                    {
                        symbol = new Word(value, flags.HasFlag(Flags.Starter));
                    }
                    else
                    {
                        symbol = new NonWord(value, flags.HasFlag(Flags.Terminal));
                    }
                    symbolDict[index] = symbol;

                } while (true);


                do
                {
                    var start = ReadInt(decompressionStream);
                    if (start == 0) break;

                    var symbols = new List<ISymbol> {symbolDict[start]};

                    do
                    {
                        var symbol = ReadInt(decompressionStream);
                        if (symbol == 0) break;
                        symbols.Add(symbolDict[symbol]);
                    } while (true);

                    var score = ReadInt(decompressionStream);

                    var chain = new Chain(symbols);
                    brain.m_Chains[chain] = score;

                } while (true);
            }

            return brain;

        }
        #endregion
    }
}