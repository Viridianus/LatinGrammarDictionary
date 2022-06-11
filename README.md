# LatinGrammarDictionary

This is an open-source C# utility for automatic "inflection" (in traditional sense - of course, there is no actual boundary between "inflection" and "derivation") of Latin.

In particular, it presumes the following "morphological parts of speech":

1. Generalized noun - anything that inflects for case (with number but with additional support for *singularia tantum* and *pluralia tantum*) but not gender;
2. Generalized adjective - anything that inflects for case and gender (number is supported as above); internally, it joins three generalized nouns and an adverb.
3. Verb - inflecting for tense, aspect, mood (including infinitive), person, number and, potentially, voice (with support for intransitive, deponent, and semideponent verbs).
4. Indeclinables.

Participles and gerundivum are generated for a verb as generalized adjectives, gerundium - as a generalized noun, two supins - as indeclinable forms (akin to adverb with generalized adjective).

Locative for nouns is supported as a separate case when it is distinct from ablative (iLocative - domi, ruri, Romae, etc.). The three third declensions are consistently distinguished, and support for Greek influences on Latin nominal (but not verbal) inflection is, where possible, added.

For verbs, it is crucial to manually list them as intransitive; by default, the forms of non-deponent verbs are built as if they are transitive - although, of course, if there is no supin stem, neither perfect participle - and thus passive in perfect system, only leaving present, imperfective, and first future passive - nor future passive infinitive are built.

The string "folder" near the end of the code should be manually replaced if you expect to store your files at the same place at all times; after that, re-compile.
