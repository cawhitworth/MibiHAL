using System.Collections.Generic;
using System.Linq;
using MibiHAL;
using NUnit.Framework;

namespace Tests
{
    class ParseTests
    {
        private IEnumerable<ISymbol> m_Symbols; 

        [SetUp]
        public void Setup()
        {
            m_Symbols = Parse.ToSymbols("It was the best of times.");
        }

        [Test]
        public void StartWord()
        {
            Assert.That(m_Symbols.First(), Is.EqualTo(new Word("It", true)));
        }

        [Test]
        public void TerminalNonWord()
        {
            Assert.That(m_Symbols.Last(), Is.EqualTo(new NonWord(".", true)));
        }
    }
}