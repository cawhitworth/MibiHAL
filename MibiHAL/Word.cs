namespace MibiHAL
{
    public class Word : ISymbol
    {
        private readonly bool m_Starter;
        private readonly int m_HashCode;

        public Word(string word, bool starter = false)
        {
            m_Starter = starter;
            Value = word.ToLowerInvariant();
            m_HashCode = Value.GetHashCode();
        }

        public string Value { get; private set; }
        public bool Terminal { get { return false; }}
        public bool Starter { get { return m_Starter; } }

        public override string ToString()
        {
            return Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is Word)
                return Equals(obj as Word);

            return base.Equals(obj);
        }

        public bool Equals(Word word)
        {
            return word.Value == Value;
        }

        public override int GetHashCode()
        {
            return m_HashCode;
        }
    }
}