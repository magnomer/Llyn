# Reflex

This folder holds the tests of reflex readings.
They cover the engine, source and store, anatomy, tone descent, anchors to fanqie rows and the tally.

## `TAnatomy`

Covers the anatomy the Classical Chinese pack cuts from a reflex reading, over the shipped `anatomy.json`.
A Korean reading falls into compatibility jamo with its bracketed variant dropped.
A Japanese reading is cut by mora, so `りむ` keeps `mu` as its coda.
A Sinitic reading keeps its tie bar and nasal mark with the onset and its length mark with the vowel.
The tone comes out as digits, a sandhi pair hyphenated, and an unreleased stop stays whole in the coda.
A language no rule names leaves every part blank.
A save stores the anatomy, a text request cuts it, and a language change recuts it.

## `TAnchor`

Covers the anchors tying a reflex row to the fanqie rows it answers.
A refetch keeps the fanqie ids on the natural key, so an anchor survives it.
A placement the refetch drops takes its anchors by cascade.
An unknown fanqie id is not stored.
The row scan leaves out an unstored row and marks a stored one held and estimated.
The anchor text of a headword longer than one glyph is empty, though its row is anchored.
The anchor request toggles one pair on one row alone, dirties the draft and commits with the entry.
The two reflex rows read back by text, not as saved, so the anchored row reads first.

## `TDescent`

Covers the tone correspondence rows the Classical Chinese pack declares, over the shipped `anatomy_tone.json`.
A pack without the key lists none.
Cantonese 35 and 34 both name class 3, and 13 names 4S and 4 together.
Gan, Jin and Xiang contours name every matching class, including checked classes that share a contour.
An unlisted contour, a blank one and a language without a row all name nothing.
The rows also drive the anchor scan, where a reflex tone marks the fanqie rows of its class as estimated.

## `TEngineReflexClip`

A rule whose `clip` does not compile loads with no clip, and a valid sibling keeps its own.

## `TEngineReflex`

Covers the engine's reflex find over a fixture pack with three rules behind stubbed pages.
The rules are read from the pack in written order, with their template, every flag and busy text.
The guise of each reflex row folds its trimmed language when the entry's pack folds it.
A padded language reads the same guise as the bare one, and a blank entry language folds nothing.
A guise flags a language phonemic only when its pack says so, so its respelling takes slashes.
With the respelling switch on, a guise shows the respelling only where the pack respells and one is filled.
A find fetches each rule's page for the character and reads one row per match.
Found rows list by the pack's declared order rule, the one a stored read follows.
The format fills its slots from the named groups, and a double-bracketed piece is dropped when its group is empty.
A `main` group that captured marks the row, and `every` keeps every match of a rule.
A rule with `first` marks its first row when no match captured `main`, and a captured `main` wins over it.
Rows of one character that all carry one text are all marked, whichever of them the page marked.
The Classical Chinese Korean pattern reads a 훈 of several words whole, the last word alone being the 음.
A rule's `recast` moves a leading tone digit of the romanization to its end.
The `superscript` flag then raises the digits of that romanization.
Each row's respelling is filled through its own language pack, and a language with none stays blank.
A headword of two characters has its marked rows of one language and kind joined into one.
Storing rows on an entry is covered in `TEngineReflexStore`.
The packs, pages and row readers the two share live in `TReflexFixture.cs`.

## `TEngineReflexSource`

Covers the page reading of one reflex rule over stubbed pages, split, gloss and dedupe included.
A rule with `split` pairs each reading with its romanization into rows.
Its gloss is looked up under the match through the gloss pattern, stopping where the `until` pattern matches.
The gloss pattern parts a source note from a parenthesized meaning and leaves a bare gloss's meaning empty.
A page listing one reading twice yields one row.
A match capturing a `note` group hands that text as the note without any gloss pattern.
A historical spelling equal to the current one folds into the marked row.
A different historical or ancient spelling is its own unmarked row labelled by the note.
A `main` group nested in the note marks the new-style row over the first, and that row is listed first.
A lone unlabelled row is still marked as the first.
A pack pattern that times out answers no rows and not reached, so the miss is not remembered.

## `TEngineReflexStore`

Covers the engine filling and keeping the reflex rows of a stored entry.
A start on an entry with nothing stored fetches, stores the rows and raises the reflex bulletin.
A start on an entry with rows stored fetches nothing.
A rebuild drops the stored rows and fetches them again.
A user-entered meaning is restored to a fetched row with the same language, region, kind and text.
A scraped meaning is not restored, so a corrected source replaces it.
An entry every page was not found for is asked once per session and stores nothing.
A held draft of the entry with no rows takes the stored rows and stays unchanged against the entry.
Rows list in the pack's declared order, not the order the rules ran in.
Two unmarked rows of one language and kind follow by text, never by arrival.
The epithet is derived when the rows are stored and kept on the entry.
Its pieces follow the pack's declared language order, then reading text, never rule or storage order.
It reads back blank once the rows are dropped.
The setting hides it from an incoming usage without touching what is stored.
A workspace rebuilt from an older schema has its blank epithets and respellings derived once on open.

## `TReflexObserver`

An observer that completes once on the bulletin a reflex test waits for.
Made without a draft it waits for the reflex bulletin of any entry.
Made with a draft it waits for the draft bulletin carrying that id instead.

## `TTally`

Covers the tally the engine builds for a Diwei page from anchored reflex rows and their anatomy.
A reflex row nobody anchored counts nothing, the page lists no placement, and its entry count is nought.
A row anchored to one of two placements of its character counts on that placement's page alone.
An initial's page counts onsets per division, one count per distinct character, both sets carried.
A character with two Mandarin readings counts once under each onset they take, and each mark names its characters.
A mark names its characters in code point order, not in the order they were placed.
Languages follow the Classical Chinese pack's declared order and marks sort by count.
Kinds of one language follow the declared kind order, whatever order their rows were stored in.
A language spelled with other letter case than the declared one ranks as undeclared, as in the reflex list.
So the tally and the entry's reflex rows order one pack alike.
A character nobody anchored is not listed, and a division of such characters is not listed either.
A rime's page counts vowel and coda joined, and Japanese Go-on and Kan-on stand on separate lines.
A rime page sections its tallies by the articulatory place of the initial, so each section counts its own characters.

## Stamp

Commit: `c37ca1837ded`
- `TAnatomy.cs` `1e0a9c9e99f93f07`
- `TAnchor.cs` `b0ad444f9ea39ae7`
- `TDescent.cs` `870bf17fcb401a80`
- `TEngineReflex.cs` `a2e632174bd7534c`
- `TEngineReflexClip.cs` `74eae8d42102ef05`
- `TEngineReflexSource.cs` `657b16091b41c7c8`
- `TEngineReflexStore.cs` `a71d60a9b25ed165`
- `TReflexObserver.cs` `e009b72135411401`
- `TTally.cs` `a2a1ba18f1db07b4`
