using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
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
        public void ForwardCandidates()
        {
            var the = new Word("the");
            var space = new NonWord(" ", false);
            var best = new Word("best");
            var worst = new Word("worst");

            var candidates = m_Brain.ForwardCandidates(new Chain(the));

            Assert.That(candidates, Is.EquivalentTo( new []
            {
                new Chain(the, space, best),
                new Chain(the, space, worst)
            }));
        }

        [Test]
        public void ForwardCandidatesTerminal()
        {
            var times = new Word("times");
            var comma_space = new NonWord(", ", false);
            var it = new Word("it");
            var fullstop = new NonWord(".", true);

            var candidates = m_Brain.ForwardCandidates(new Chain(times));

            Assert.That(candidates, Is.EquivalentTo( new []
            {
                new Chain(times, comma_space, it),
                new Chain(times, fullstop)
            }));
        }

        [Test]
        public void BackwardsCandidates()
        {
            var of = new Word("of");
            var space = new NonWord(" ", false);

            var best = new Word("best");
            var worst = new Word("worst");

            var candidates = m_Brain.BackwardsCandidates(new Chain(of));

            Assert.That(candidates, Is.EquivalentTo( new[]
            {
                new Chain(best, space, of),
                new Chain(worst, space, of),

            }));
        }

        [Test]
        public void LoadSave()
        {
            var s = new MemoryStream();

            m_Brain.Save(s);

            s.Seek(0, SeekOrigin.Begin);

            var b = Brain.Load(s);

            Assert.That(m_Brain.Chains(), Is.EquivalentTo(b.Chains()));
        }
    }
}