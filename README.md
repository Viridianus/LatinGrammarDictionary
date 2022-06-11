# LatinGrammarDictionary

This is an open-source C# utility for automatic "inflection" (in traditional sense - of course, there is no actual boundary between "inflection" and "derivation") of Latin.

In particular, it presumes the following "morphological parts of speech":

1. Generalized noun - anything that inflects for case and number (with additional support for *singularia tantum* and *pluralia tantum*) but not gender;
2. Generalized adjective - anything that inflects for case, number, and gender; internally, it joins three generalized nouns and an adverb.
3. Verb - inflecting for tense, aspect, mood (including infinitive), person, number and, potentially, voice (with support for intransitive, deponent, and semideponent verbs).
4. Indeclinables.

Participles and gerundivum are generated for a verb as generalized adjectives, gerundium - as a generalized noun, two supins - as indeclinable forms (akin to adverb with generalized adjective).
