using System.Text;

namespace MibiHAL
{
    public class Prettifier
    {
        public static string Prettify(string s)
        {
            var sb = new StringBuilder();
            bool capitalise = true;
            foreach (var c in s)
            {
                if (CharacterClassifier.IsWordChar(c))
                {
                    sb.Append(capitalise ? char.ToUpper(c) : c);
                    capitalise = false;
                }
                else
                {
                    if (CharacterClassifier.IsCapitalisingPunctuation(c))
                    {
                        capitalise = true;
                    }
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }
    }
}