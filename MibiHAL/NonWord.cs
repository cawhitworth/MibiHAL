using System.Dynamic;

namespace MibiHAL
{
    public class NonWord : ISymbol
    {
        private readonly int m_HashCode;
        private readonly bool m_Terminal;

        public NonWord(string nonword, bool terminal)
        {
            Value = nonword;
            m_Terminal = terminal;
            m_HashCode = Value.GetHashCode() ^ (397 * Terminal.GetHashCode());
        }

        public string Value { get; private set; }

        public bool Terminal
        {
            get { return m_Terminal; }
        }

        public bool Starter { get { return false; } }

        public override string ToString()
        {
            return Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is NonWord)
                return Equals(obj as NonWord);

            return base.Equals(obj);
        }

        public bool Equals(NonWord nonWord)
        {
            return nonWord.Value == Value && nonWord.Terminal == Terminal;
        }

        public override int GetHashCode()
        {
            return m_HashCode;
        }
    }
}