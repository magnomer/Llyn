# Language

This folder holds the language pack relays and fixtures.
They cover flags, typography, facts, a throwaway pack and the reflex pack.

## `TInterfaceEnsign`

The relay for the shared flag-fill ordering rule, so a test drives it over a fake settings port.
The relay forwards to `CCatalog.LCatalogEnsignLoad` and is transparent.

## TInterfaceFact

The relays for the facts the engine records answer with, plus the localization, theme, usher and flag cache relays.
Most relays hand one production operation through unchanged.
The theme read, the flag path adds and the epoch table read do a little work of their own.
The static constructor lists the embedded catalog languages once, as the bootstrap does.
So the localization relays normalise against the real list.
The state value relay normalizes a store value.
So a Conduct test can read the cleared uncertainty without a Core call.
The paradigm slot relay lets a test pin each status without the wording.
The fanqie sort relay lets a test pin the order the fanqie clerk reads and groups rows in.
The script image builder with an epoch makes a stored image with no gloss and no data.
The epoch and the stored id are the keys a sort test varies.

## `TInterfaceFont`

The relays for a language pack's typography, from the engine font to the one Conduct font rule.
Each relay is transparent.
The engine font builders let a fake settings port answer, so a test never constructs a Core record.
The empty engine font stands for no family and no size, as a blank language answers.
The Conduct font builder makes the expected font, so a test never constructs a Conduct record itself.
The role relay casts an engine role to the Conduct role of the same name.
`TFont` pins that the two enums stay in step.

## `TLanguageFixture`

A throwaway language pack written beside the shipped ones, under a name no other test uses.
The engine loads it by that name exactly as it loads a shipped pack.
Disposing removes the folder again.
The load relay writes the pack, loads it through the relay and removes it again.
A test that only reads the loaded pack needs no handle on the folder.
The save writes a second file beside the pack's source.json, for a pack that names a file of its own.

## `TReflexFixture`

The reflex pack, the pages behind it and the readers the reflex engine tests share.
The pack declares three rules, one each for Korean, Japanese and Mandarin, over the stubbed pages.
It declares the same languages as its row order, with Go-on before Kan-on.
The Wu pack declares one rule with a recast and superscript tones.
The settle waits until the engine reports no fetch running for the entry, or fails after the patience.
The row read reduces a row to its language, kind, text, romanization and main mark, so a test compares tuples.
The fetch read saves an entry of the headword and starts its reflex fetch.
It waits for the fetch to settle and reads what it stored.
The draft builder makes a draft with one bare card, enough for the entry to be saved.

## Stamp

Commit: `f3a687d70b47`
- `TInterfaceEnsign.cs` `9cba746c999a7ec7`
- `TInterfaceFact.cs` `fc800f4e92340903`
- `TInterfaceFont.cs` `d97fc8eec98e7420`
- `TLanguageFixture.cs` `2eae066608973693`
- `TReflexFixture.cs` `3f1ba34dce04007a`
