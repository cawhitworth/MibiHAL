using MibiHAL;
using NUnit.Framework;

namespace Tests
{
    class ChainTests
    {
        private Chain m_Chain;

        [SetUp]
        public void Setup()
        {
            var symbols = Parse.ToSymbols("It was the best of times, it was the worst of times.");

            m_Chain = new Chain(symbols);
        }

        [Test]
        public void Last()
        {
            var space = new NonWord(" ", false);
            var expected = new Chain(
                new Word("of"), space,
                new Word("times"), new NonWord(".", true));

            Assert.That(m_Chain.Last(4), Is.EqualTo(expected));
        }

        [Test]
        public void Slice()
        {
            Assert.That(m_Chain.Slice(4, 3), Is.EqualTo(new Chain(
                new Word("the"), new NonWord(" ", false),
                new Word("best")
            )));
        }

        [Test]
        public void StartsWith()
        {
            Assert.That(m_Chain.StartsWith(new Chain(
                new Word("it"), new NonWord(" ", false),
                new Word("was"), new NonWord(" ", false)
             )), Is.True);
        }

        [Test]

        public void EndsWith()
        {
            var chain = new Chain(
                new NonWord(" ", false),
                new Word("times"),
                new NonWord(".", true)
                );

            Assert.That(m_Chain.EndsWith(chain));
        }
    }
}
