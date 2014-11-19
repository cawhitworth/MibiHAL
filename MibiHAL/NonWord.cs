namespace MibiHAL
{
    public class NonWord : ISymbol
    {
        public NonWord(string nonword, bool terminal)
        {
            Value = nonword;
            Terminal = terminal;
        }

        public string Value { get; private set; }
        public bool Terminal { get; private set; }

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
            return Value.GetHashCode() ^ (397 * Terminal.GetHashCode());
        }
    }
}