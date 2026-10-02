# TEngineReflex.cs
Hash: `f4fe937a804c4fd9`

## `public sealed class TEngineReflex`

Covers the engine's reflex find over a fixture pack with three rules behind stubbed pages.
The rules are read from the pack in written order, with their template, every flag and busy text.
The guise of each reflex row folds its trimmed language when the entry's pack folds it.
A padded language reads the same guise as the bare one.
A blank entry language folds nothing.
A find fetches each rule's page for the character and reads one row per match.
The format fills its slots from the named groups, and a double-bracketed piece is dropped when its group is empty.
A `main` group that captured marks the row, and `every` keeps every match of a rule.
A rule with `first` marks its first row when no match captured `main`, and a captured `main` wins over it.
Rows of one character that all carry one text are all marked, whichever of them the page marked.
The Classical Chinese Korean pattern reads a 훈 of several words whole, the last word alone being the 음.
A rule's `recast` moves a leading tone digit of the romanization to its end.
The `superscript` flag then raises the digits of that romanization.
Each row's respelling is filled through its own language pack, beside the text and romanization as read.
A row whose language has no respelling stays blank.
A headword of two characters has its marked rows of one language and kind joined into one.
The fetch that stores the rows on an entry is covered in `TEngineReflexStore.cs`.
The packs, pages and row readers the two share live in `TReflexFixture.cs`.
