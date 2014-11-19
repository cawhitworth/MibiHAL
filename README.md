# MibiHAL

### Mucking around with Markov chains

This is the start of a reimplementation of some the ideas in the the [MegaHAL](http://megahal.alioth.debian.org/)
conversation simulator.

It uses n-gram statistical modeling of language, based on a corpus (lifted from MegaHAL) and then generates random plausible
sentences based on that model. It treats words and non-words as elements in the n-grams, so a 3-gram will typically be
word-nonword-word, or nonword-word-nonword.

By default, it builds 3-grams and 5-grams from the corpus, and only searches for potential n-grams starting with a single
word, but there's no reason this couldn't be extended.
