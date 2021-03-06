﻿using System;
using System.Collections.Generic;

namespace MibiHAL
{
    public class Parse
    {
        private enum State
        {
            Starting,
            InWord,
            InNonWord
        };

        public static IEnumerable<ISymbol> ToSymbols(string toParse)
        {

            var state = State.Starting;
            string symbol = string.Empty;

            bool firstWord = true;

            foreach (char chr in toParse)
            {
                var c = char.ToLowerInvariant(chr);
                switch (state)
                {
                    case State.Starting:
                        state = CharacterClassifier.IsWordChar(c) ? State.InWord : State.InNonWord;
                        symbol += c;
                        break;

                    case State.InWord:
                        if (CharacterClassifier.IsWordChar(c))
                        {
                            symbol += c;
                        }
                        else
                        {
                            yield return new Word(symbol, firstWord);
                            symbol = string.Empty;
                            symbol += c;
                            state = State.InNonWord;
                            firstWord = false;
                        }
                        break;
                    case State.InNonWord:
                        if (CharacterClassifier.IsNonWordChar(c))
                        {
                            symbol += c;
                        }
                        else
                        {
                            yield return new NonWord(symbol, false);
                            symbol = string.Empty;
                            symbol += c;
                            state = State.InWord;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (state == State.InWord)
            {
                yield return new Word(symbol);
            }
            else if (state == State.Starting)
            {
                yield break;
            }
            else if (state == State.InNonWord)
            {
                yield return new NonWord(symbol, true);
            }
        }
    }

    class CharacterClassifier
    {
        public static bool IsCapitalisingPunctuation(char c)
        {
            return c == '!' || c == '.' || c == '?';
        }

        public static bool IsWordChar(char c)
        {
            return Char.IsLetterOrDigit(c) || c == '\'';
        }

        public static bool IsNonWordChar(char c)
        {
            return !IsWordChar(c);
        }
    }
}