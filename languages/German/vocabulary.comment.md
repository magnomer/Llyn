# vocabulary.json

## file

Display vocabulary for German.
The file format is documented in `languages/French/vocabulary.comment.md`.
Every id is an integer that never changes once published.

## `parts`

The verb presets add modal and separable, which German marks in the dictionary.
The pronoun presets add indefinite, for man and jemand.
Postposition stands beside preposition, for entlang and zufolge.
Particle stands with a modal child, for doch, mal and ja.
No partitive article is listed, since German has none.

## `features`

A noun takes gender and number.
An adjective takes form, the degree of comparison.
A verb takes form, class and auxiliary.
Class is weak, strong or mixed.
A preposition takes case, with two-way for one governing accusative or dative.

## `paradigms`

The noun paradigm predicts the plural.
A headword ending in in doubles the n and takes en.
One ending in ung, heit, keit, schaft, tät, ion, ie or ei takes en.
One ending in e takes n.
One ending in el, er, en, chen or lein stays.
One ending in a vowel other than e takes s.
Any other takes e.
An umlaut plural such as Häuser is so irregular and carries a note.
The adjective paradigm predicts the comparative and the attributive superlative stem.
The comparative adds er, or r after a final e.
The superlative adds st, or est after d, t, s, ß, x, z or sch.
An umlaut such as älter is irregular.
The verb paradigm predicts the third-person singular present, the preterite and the past participle.
The present adds t to the stem, or et after d, t or a consonant plus m or n.
The preterite adds te or ete the same way.
The participle wraps the stem in ge and t, or et after d or t.
A stem ending in ier takes no ge.
Neither does one opening with be, emp, ent, er, ge, miss, ver or zer.
An eln or ern infinitive loses only its n.
A strong verb such as gehen is so irregular in every form and carries a note.
