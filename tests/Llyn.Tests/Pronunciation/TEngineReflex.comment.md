# TEngineReflex.cs

## `public sealed class TEngineReflex`

Covers the engine's reflex fill over a fixture pack with three rules behind stubbed pages.
The rules are read from the pack in written order, with their template, every flag and busy text.
A find fetches each rule's page for the character and reads one row per match.
The format fills its slots from the named groups, and a double-bracketed piece is dropped when its group is empty.
A `main` group that captured marks the row, and `every` keeps every match of a rule.
A rule with `first` marks its first row when no match captured `main`, and a captured `main` wins over it.
Rows of one character that all carry one text are all marked, whichever of them the page marked.
The Classical Chinese Korean pattern reads a 훈 of several words whole, the last word alone being the 음.
A rule's rewrites run over the formatted text, so slashed readings are stored bracketed.
Each row's respelling is filled through its own language pack's groups whatever the switch says, the text and note untouched.
A phonemic borrower pack has the brackets of its respelling turned into slashes.
A headword of two characters has its rows of one language and kind folded into one.
A start on an entry with nothing stored fetches, stores the rows and raises the reflex bulletin.
A start on an entry with rows stored fetches nothing.
An entry every page was not found for is asked once per session and stores nothing.
A held draft of the entry with no rows takes the stored rows and stays unchanged against the entry.
The epithet is derived when the rows are stored and kept on the entry.
It reads back blank once the rows are dropped.
The setting hides it from the read without touching what is stored.
A workspace rebuilt from an older schema has its blank epithets and respellings derived once on open.
