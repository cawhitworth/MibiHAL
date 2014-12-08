using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MibiHAL
{
    public class Chain
    {
        private readonly ISymbol[] m_Symbols;
        private int m_HashCode = -1;

        public Chain(IEnumerable<ISymbol> symbols)
        {
            m_Symbols = symbols as ISymbol[] ?? symbols.ToArray();
        }

        public Chain(params ISymbol[] symbols)
        {
            m_Symbols = symbols;
        }

        public int Length
        {
            get { return Symbols.Count(); }
        }

        public Chain Join(Chain other)
        {
            var a = Symbols.ToList();
            a.AddRange(other.Symbols);
            return new Chain(a);
        }

        public Chain First(int count)
        {
            return new Chain(Symbols.Take(count));
        }

        public Chain Last(int count)
        {
            return new Chain(Symbols.Skip(Length - count).Take(count));
        }

        public bool StartsWith(Chain chain)
        {
            var symbols = chain.Symbols;
            var enumerable = symbols as ISymbol[] ?? symbols.ToArray();

            var mine = Symbols.Take(enumerable.Length);

            return mine.Zip(enumerable, (s1, s2) => s1.Equals(s2)).All(eq => eq);
        }

        public bool EndsWith(Chain chain)
        {
            var symbols = chain.Symbols;

            var enumerable = symbols as ISymbol[] ?? symbols.ToArray();

            var mine = Symbols.Skip(Symbols.Count() - enumerable.Length);

            return mine.Zip(enumerable, (s1, s2) => s1.Equals(s2)).All(eq => eq);
        }

        public IEnumerable<ISymbol> Symbols
        {
            get { return m_Symbols; }
        }

        public Chain Slice(int start, int count)
        {
            return new Chain(Symbols.Skip(start).Take(count));
        }

        public override int GetHashCode()
        {
            // Memoize the hashcode
            if (m_HashCode == -1)
                m_HashCode = Symbols.Aggregate(0, (current, symbol) => current ^ 397*symbol.GetHashCode());
            return m_HashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj is Chain)
                return Equals(obj as Chain);
            return base.Equals(obj);
        }

        public bool Equals(Chain chain)
        {
            if (chain.Length != Length) return false;

            var itr1 = Symbols.GetEnumerator();
            var itr2 = chain.Symbols.GetEnumerator();
            do
            {
                if (itr1.MoveNext() || itr2.MoveNext())
                {
                    break;
                }
                if (!itr1.Current.Equals(itr2.Current))
                    return false;
            } while (true);

            return true;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append("Chain [");

            sb.Append(string.Join(", ", Symbols.Select(s => "<" + s.ToString() + ">")));

            sb.Append("]");

            return sb.ToString();
        }
    }
}