# vocabulary.json
Hash: `58acdd0eae49c1f5`

## file

Display vocabulary for Portuguese.
The file format is documented in `languages/French/vocabulary.comment.md`.
Every id is an integer that never changes once published.
Feature codes 3 to 8 and value codes 5 to 21 are retired and must not be reissued.
They once listed adjective gender and number and verb mood, tense, person and number.
Those are inflection dimensions, not facts a dictionary entry states, so the features were replaced.

## `registers`

Literary covers the second-person plural and the mesoclitic future, which speech has dropped.
Slang covers gíria such as cara and fixe.

## `parts`

It lists the parts of speech, each a bare role.
Valency and other subtypes belong to the dependence field and the morphology, not here.
Determiner stands beside article, for meu and este before a noun.
No partitive article is listed, since Portuguese has none.

## `retired`

It lists the parts once published and since removed, so no id is ever reused.
The import applies every row on each start, and a row whose part is already gone does nothing.
`into` names the part that takes over the removed part's links.
`unit` names the lexical unit set on a linking entry that has none yet.
A row with neither drops the links.

## `features`

A noun takes gender and number.
Number marks a plurale tantum such as óculos.
An adjective takes form, the four gender and number spellings.
A verb takes form and conjugation.
Form lists the infinitive, the gerund and the past participle.
Conjugation is first, second or third, the ar, er and ir verbs.
No auxiliary feature is listed, since every compound tense takes ter.

## `paradigms`

The list stands empty until the regular rules are written.
