# Source

This folder holds the tests of the pack-defined pronunciation sources.
They cover the generic runner, morphology, real packs and Spanish.

## `TSourceGeneric`

Covers the generic source reading several varieties from one fetched body.
Two readings on one attempt both come back tagged.
A later attempt fills only the varieties an earlier one missed, first wins per variety.
A skip count passes over earlier matches of the same pattern, and still moves which match is the first.
A pattern matching several times yields the first match alone, so a page's inflections are left out.
A reading marked every yields every match once, so a page with two etymologies shows both readings.
A flat attempt marked every is one untagged reading yielding every match.
A comma or slash inside one matched text separates alternatives, and each is a reading of its own.
Spelling rules on a spec recast the headword before the token fills, so a macron headword matches the bare file.
A `{word}` token in a pattern is the headword, escaped so a regex character in it matches literally.
A link reading answers the fetched address itself, with `{headword}` filled as typed before any spelling rule.
A decoded attempt matches characters the page writes as entities, which a plain attempt misses.
A missed guard is a blank answer and a server failure is a lost one.
A page that only points at another headword is followed there, and the reading comes from the page pointed at.
A page with its own readings that also points elsewhere keeps its own and adds the pointed page's.
Two pages pointing at each other stop at the hop limit with a blank reached answer.
The headword an attempt followed to is the one a later attempt asks for.

## `TSourceMorphology`

Covers the generic source reading inflected forms from one fetched body, as a pack's `morphology` list declares them.
Three regex readings keyed on morphology value codes come back tagged `"5"`, `"6"` and `"2"` with their spelling untouched.
A missed confirm guard is a blank answer that was reached.
A server failure is a lost answer.
The fixture body has three form-of blocks in the shape the Wiktionary REST HTML writes them.
The class names the kind of form and the anchor text is the form itself.

## `TSourcePack`

Covers real language packs reading IPA from Wiktionary's REST rendering.
The reader reads only the first page that answers, and the REST rendering answers first.
So a pack pattern that misses on that rendering never reaches the article page.
The REST rendering writes `data-mw-wikitext` before `id` on each section heading.
A heading anchor written as `<h2 id=` therefore missed every REST page.
The Korean fixture holds an empty category span inside the IPA span, which the span reader skips.
The Spanish fixture lists the phonemic reading first and the phonetic one in brackets after it.
It also covers the Mandarin and Cantonese `glyph` source on recorded wikitext answers of the parse API.
A traditional page with no `{{zh-see}}` pointer answers its own title, as the reading view shows it.
A simplified page answers the form its pointer names, and only that form.
A missing page answers a blank that was reached.

## `TSourceSpanish`

Covers the shipped Spanish `morphology` source reading one recibir page.
The readings come from the pack itself, so the test reads the patterns the app fetches with.
Each reading must hit exactly one cell of the Spanish table and nothing in the decoy.
The page answers 64 readings, one per verb cell, in the paradigm's cell order.
Each variety is the cell key, so the fetch can match it to its slot.
A 2sg cell carrying tú and vos answers the tú form.
The vos form of the present subjunctive appears nowhere.
Each negative imperative cell answers the form after `no`, never `no` itself.
The Galician decoy table comes first and reuses the imperative titles.
The answer still holds the Spanish 2pl imperatives and no Galician form.
The embedded fixture exercises language anchoring, alternate forms, empty cells and nested header markup.
The affirmative decoy keeps an empty first cell, exposing incorrect second-plural reads without language anchoring.
The Spanish table keeps the eleven finite rows and drops the nonfinite rows and person headers.
The future subjunctive header keeps its footnote `<sup>`.
Expected forms follow paradigm cell order, independently checking each extracted form.
The fake client answers the body for any address, so the live URL is never called.

## Stamp

Commit: `aae5349dbb94`
- `TSourceGeneric.cs` `095a67904a6f352c`
- `TSourceMorphology.cs` `dad89363daf96f29`
- `TSourcePack.cs` `2ee512e52a57fe5f`
- `TSourceSpanish.cs` `19854b0c39cfbb71`
