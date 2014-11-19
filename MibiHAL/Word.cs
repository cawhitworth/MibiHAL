namespace MibiHAL
{
    public class Word : ISymbol
    {
        public Word(string word)
        {
            Value = word.ToLowerInvariant();
            Terminal = false;
        }

        public string Value { get; private set; }
        public bool Terminal { get; private set; }

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
            return Value.GetHashCode();
        }
    }
}