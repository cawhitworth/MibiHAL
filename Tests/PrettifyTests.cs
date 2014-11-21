using MibiHAL;
using NUnit.Framework;

namespace Tests
{
    class PrettifyTests
    {
        [Test]
        public void CapitaliseStart()
        {
            var s = "hello world.";

            Assert.That(Prettifier.Prettify(s), Is.EqualTo("Hello world."));
        }

        [Test]
        public void CapitaliseAfterFullStop()
        {
            var s = "hello world. this is great.";

            Assert.That(Prettifier.Prettify(s), Is.EqualTo("Hello world. This is great."));
        }

        [Test]
        public void CapitaliseAfterFullStop_WithSeveralSpaces()
        {
            var s = "hello world.    this is great.";

            Assert.That(Prettifier.Prettify(s), Is.EqualTo("Hello world.    This is great."));
        }

        [Test]
        public void CapitaliseAfterPunctuation()
        {
            var s = "hello! who are you? are you my friend?";

            Assert.That(Prettifier.Prettify(s), Is.EqualTo("Hello! Who are you? Are you my friend?"));
        }
    }
}