using System;
using System.Linq;
using System.Text;
using MibiHAL;
using NUnit.Framework;

namespace Tests
{
    class BrainTests
    {
        private Brain m_Brain;

        [SetUp]
        public void Setup()
        {
            var symbols = Parse.ToSymbols("It was the best of times, it was the worst of times.");

            m_Brain = new Brain();
            m_Brain.Train(symbols, 3);
        }

        [Test]
        public void Count()
        {
            foreach (var chain in m_Brain.Chains())
            {
                Console.WriteLine(chain);
            }
            Assert.That(m_Brain.Chains().Count(), Is.EqualTo(17));
        }

        [Test]
        public void Candidates()
        {
            var the = new Word("the");
            var space = new NonWord(" ", false);
            var best = new Word("best");
            var worst = new Word("worst");

            var candidates = m_Brain.Candidates(new Chain(the));

            Assert.That(candidates, Is.EquivalentTo( new []
            {
                new Chain(the, space, best),
                new Chain(the, space, worst)
            }));
        }

        [Test]
        public void CandidatesTerminal()
        {
            var times = new Word("times");
            var comma_space = new NonWord(", ", false);
            var it = new Word("it");
            var fullstop = new NonWord(".", true);

            var candidates = m_Brain.Candidates(new Chain(times));

            Assert.That(candidates, Is.EquivalentTo( new []
            {
                new Chain(times, comma_space, it),
                new Chain(times, fullstop)
            }));
        }
    }
}