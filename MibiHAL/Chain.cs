using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MibiHAL
{
    public class Chain
    {
        private readonly IEnumerable<ISymbol> m_Symbols;

        public Chain(IEnumerable<ISymbol> symbols)
        {
            m_Symbols = symbols;
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
            return new Chain(Symbols.TakeWhile((s, i) => i < count));
        }

        public Chain Last(int count)
        {
            return new Chain(Symbols.Reverse().TakeWhile((s, i) => i < count).Reverse());
        }

        public bool StartsWith(Chain chain)
        {
            var symbols = chain.Symbols;
            var enumerable = symbols as ISymbol[] ?? symbols.ToArray();
            return First(enumerable.Count()).Symbols.Zip(enumerable, (s1, s2) => s1.Equals(s2)).All(eq => eq);
        }

        public bool EndsWith(Chain chain)
        {
            var symbols = chain.Symbols;
            var enumerable = symbols as ISymbol[] ?? symbols.ToArray();
            return Last(enumerable.Count()).Symbols.Zip(enumerable, (s1, s2) => s1.Equals(s2)).All(eq => eq);
        }

        public int Score { get; set; }

        public IEnumerable<ISymbol> Symbols
        {
            get { return m_Symbols; }
        }

        public Chain Slice(int start, int count)
        {
            return new Chain(Last(Length - start).Symbols.TakeWhile((s, i) => i < count));
        }

        public override int GetHashCode()
        {
            var hash = Symbols.Aggregate(0, (current, symbol) => current ^ 397*symbol.GetHashCode());
            return hash;
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

            return chain.Symbols.Zip(Symbols, (s1, s2) => s1.Equals(s2)).All(eq => eq);
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