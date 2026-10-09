# source.json
Hash: `43989068fd6c1e68`

## `language`

The pack identifies Spanish independently of the interface language.

## `flag`

The selector uses the Spain flag identifier.

## `font`

Headwords use Segoe UI at size 40.

## `example`

Examples prefer Georgia, falling back to Segoe UI, at size 15.

## `pronunciation`

No pronunciation source is configured here.

## `audio`

No audio source is configured here.

## `inflection`

The inflection key names the file beside this one holding the verb rule book, `inflection.json`.
The book predicts each regular form and marks the letters where a stored form departs from it.
It is kept apart because its rules and layout are long and change on their own.

## `morphology[0]`

One Wiktionary REST HTML attempt declares 64 cell-keyed readings.
Normalization is disabled so conjugated spelling survives extraction.
The attempt requires `lang-es` on the page, so a page without a Spanish section gives no reading.
The Spanish conjugation sits in one `roa-inflection-table`.
Each finite row opens with a `<th>` holding `<span title="T">`, where `T` is the Spanish row name.
Six `<td>` cells follow, one per person and number, first singular to third plural.
A cell's form is the text of its first `<a>`.
Indicative and subjunctive 2sg cells carry the tú form first and the vos form second.
So the first `<a>` keeps the tú form and drops vos.
The imperative rows come last and keep an empty first `<td>` for the missing first singular.
Every negative imperative cell reads `no` and the form as two `<a>` in one span.
Its reading skips the `no` link and captures the next one.
The two imperfect subjunctive rows share one title and differ by `<br/>(ra)` or `<br/>(se)` after it.
The future subjunctive header ends in a footnote `<sup>`, so a header may carry markup before `</th>`.
Rows after the present indicative first require its title, avoiding earlier tables with reused imperative titles.
These anchors do not explicitly enforce table boundaries.
Each reading takes column k by skipping k cells, and its `variety` is the cell key.
Patterns require exact row-title spellings, including `copréterito`.
They read `presente de indicativo` and `pretérito imperfecto (copréterito)`.
They read `pretérito perfecto simple (pretérito indefinido)` and `futuro simple (futuro imperfecto)`.
They read `condicional simple (pospretérito de modo indicativo)` and `presente de subjuntivo`.
They read `pretérito imperfecto de subjuntivo` and `futuro simple de subjuntivo (futuro de subjuntivo)`.
They read `imperativo afirmativo` and `imperativo negativo`.
Embedded recibir fixtures verify alternate tú forms, negative-particle skipping, and earlier Galician decoys.
