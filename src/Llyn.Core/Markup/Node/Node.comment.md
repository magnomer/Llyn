# Node

This folder holds the markup tree, the entry-level reader and writer, and the read results.
It stands apart from `Card`, which holds the card-level records.
Core sees a file only as `LMarkupNode` values and never the XML.
`LMarkupFile` in Infrastructure parses the text into the tree and formats the tree back.

## `LMarkup`

The `.llx` reader and writer, the one door between the markup tree and markup records.
The format is real XML with a `llyn` root and one `entry` element per entry.
It carries no id of any kind.
A file written by the program, a person or an AI reads the same.
Element names are an external contract and stay plain lowercase English.
The element tree mirrors `LMarkupEntry` one to one.

## `LMarkupState`

The one attribute the format knows, marking a value as unknown.
Its one value is `unknown`.
The writer emits only that attribute set.

## `LMarkupDepthCeiling`

The deepest element a file may nest, counting the root as one.
The adapter enforces it while streaming the text, before any tree is built.

## `LMarkupOmissionCeiling`

The most omissions one read reports before it stops counting.
The same ceiling bounds the whole import report, read and import rows together.

## `LMarkupParse`

Reads the tree under the root into entries in file order and reports what was skipped.
A root that is not `llyn` refuses with `LRefusal.LRefusalMarkup`.
Malformed XML and nesting past the depth ceiling are refused by the adapter before the tree exists.
An unknown element is skipped with its whole subtree and named in the omissions with its line.
Any attribute other than `state="unknown"` is skipped and named the same way, so an `id` never enters.

## `LMarkupFormat`

Writes entries as a tree under one `llyn` root.
An unspecified value writes nothing.
An unknown value writes an empty element marked unknown.

## `LMarkupOmissionClamp`

Cuts a finished report to the ceiling, so rows added after the read cannot pass it.
A report within the ceiling comes back as it is.
A longer one keeps its first rows and ends with exactly one `...` row.
That row takes the line of the first row it hides, as the reader's own `...` row does.
A reader `...` row always stands at the ceiling index, so it is replaced and never doubled.

## `LMarkupValueParse`

No element or a blank one is unspecified.
The state `unknown` reads as unknown, and text reads as specified.

## `LMarkupValueFormat`

Unspecified writes nothing.
Unknown writes an empty element with the state attribute.
Specified writes its text.

## `LMarkupNumberParse`

Reads an integer field, nothing when the text is not a number.
Each caller picks the value that marks the field as unreadable.
A bad offset is therefore dropped and not moved to zero.

## Text helpers in `LMarkup`

A plain text field reads as empty when the element is absent.
A plain text field writes nothing at all when the text is empty.

## `LMarkupNode`

One element of a markup file as a value, free of the parser that read it.
The reader walks a tree of these and the writer builds one.
The line is zero for a node the writer built.
`LMarkupNodeCreate` builds attribute-free leaves or branches with no mixed text.
The reader can also carry attributes on a node, including the unknown-state mark.

## `LMarkupOmission`

One thing a markup read or import left behind, named so the reader can find it in the file.
The parser reports unknown elements and attributes here, and the import later adds what it could not resolve.
The line is zero when unknown.

## `LMarkupCargo`

What one read of a markup file holds, the parsed entries and what the read skipped.
The shell shows the entries, asks how each enters, and hands the same cargo back to the import.
The file is read once, so what the user saw is what is imported.
A file swapped on disk between the two steps changes nothing.

## `LMarkupReader`

Turns the entry-level elements of a markup document into records.
Every element parser walks its children in file order and hands each name it knows to a field.
A name it does not know becomes an omission and its subtree is never entered.
Card-level elements are read by `LMarkupCardReader`.
Sound elements are read by `LMarkupPronunciationReader`.

## `LMarkupAttributeScan`

Walks an element and everything under it once and reports every attribute except `state="unknown"`.
An `id` attribute is the expected case, since the format never carries ids.
A `state` with any other value is reported too.
The reader would otherwise read the text as specified in silence.

## `LMarkupOmissionAdd`

Adds one omission while the list is under `LMarkup.LMarkupOmissionCeiling`.
The first one past it is written as `...` and the rest are dropped.
A file of a million unknown attributes would otherwise be reported one row at a time.

## `LMarkupEntryParse`

Reads one `entry` element with all of its repeated children in file order.
The line the element opens on travels with the record, so the import can place its own omissions.
A `unit` element is read through `LUnitKeyParse`, so an unknown value leaves the unit unchosen.
A `freedom` element is read through `LFreedomKeyParse` the same way.
A unit or freedom word the parser does not know is also reported, so the user sees it was dropped.
The report lives here, not in the parsers, because the speech loader also calls `LUnitKeyParse`.

## `LMarkupTranscriptionAdd`

Keeps one `transcription` element unless its scheme is blank or already read in this entry.
Such a row is reported and skipped, so the rest of the file still imports.
The scheme compare is the save's, trimmed and ordinal.
The save refuses those rows, and that refusal guards the editor, so the file is cleaned here.

## `LMarkupEtymologyParse`

Reads text with the spans that name an entry.
A span carries no sense, so a `sense` child is an omission.

## `LMarkupPronunciationReader`

Turns the sound elements of one entry into drafts.
It reads `pronunciation`, `transcription` and `reflex` elements and the syllables under a pronunciation.
Each parser walks its children in file order like `LMarkupReader`, and an unknown name becomes an omission.

## `LMarkupPronunciationParse`

Audio and source are kept verbatim.
A `respelling` child fills the draft's respelling, and a file without one leaves it blank.
Syllables read at their position under a pronunciation with no id.

## `LMarkupReflexParse`

Reads a language, kind, text, respelling, romanization, meaning, note and region.
An empty `owned` element marks a user-entered meaning.
An empty `main` element marks the common reading.

## `LMarkupWriter`

Turns entry-level records into markup elements.
Children are written in the order the format table lists them, repeated rows in list order.
An empty text field writes nothing, so a minimal entry stays short.
Card-level records are written by `LMarkupCardWriter`.

## `LMarkupEntryFormat`

Writes one `entry` element with its repeated rows.
An etymology with empty text is left out, even when it holds mentions.
The `unit` element is left out when no unit is chosen.
The `freedom` element follows it, left out the same way.

## `LMarkupLocalFormat`

Writes a `local` element whenever the spelling is stated, even as empty text.
A stored empty spelling would otherwise come back as none, and the next import would count that as a change.

## `LMarkupPronunciationFormat`

Audio and source are written verbatim and never copied.
The `respelling` element follows `ipa` and is skipped when empty, like every other plain text child.

## `LMarkupSyllableFormat`

The syllable tone number is written in invariant digits.

## `LMarkupReflexFormat`

Writes empty `owned` and `main` elements when their flags are marked.

## Stamp

Commit: `c2f335c331fe`
- `LMarkup.cs` `f1a4662e9be0ea86`
- `LMarkupCargo.cs` `814d8aeee9c7370b`
- `LMarkupNode.cs` `0772124e410ccc3b`
- `LMarkupOmission.cs` `40e39dad503436c1`
- `LMarkupPronunciationReader.cs` `f10ade4a00de35ba`
- `LMarkupReader.cs` `3965a6bf1dc54305`
- `LMarkupWriter.cs` `fd2234f30a75c478`
