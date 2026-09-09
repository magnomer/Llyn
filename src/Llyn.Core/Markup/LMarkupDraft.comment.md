# LMarkupDraft.cs

## `public static class LMarkupDraft`

Writes a whole Llyn Markup document back out.
It is the inverse of `LMarkup`, and the two must agree tag for tag.
A field one writes and the other does not read is the defect this pairing exists to prevent.
Markup is the only export that keeps everything, including what the display never shows.

The writer takes a document rather than an entry.
One file holds any number of entries and one catalog they share.
Shaping the writer around a single entry would make sharing impossible to write down.

## `public static string LMarkupDraftFormat(LMarkup.LMarkupDocument document, IReadOnlyDictionary<string, string> keys)`

Writes `<llyn>`, then the catalog, then every entry in the order the caller gave them.

The keys map every stored row id to the key the document declares it under.
The drafts carry stored ids, and a file may name nothing outside itself.
So a citation is written by looking its row up here.
A row that is not in the map is not in this document, and the citation naming it is dropped.
That is how a translation to an entry outside the export leaves no dangling key behind.

The layer that reads the workspace builds the map, because only it knows what a row is called.
Deriving keys here would need the row content this class is only handed in part.

**Parameters**

- `document` — The catalog to declare and the entries to write.
- `keys` — Every stored row id, and the document key it is written under.

## `private static void LMarkupDraftAppend(StringBuilder text, LMarkup.LMarkupCatalog catalog)`

Writes the `<catalog>` block, or nothing at all when the document shares no row.
Section 2 of the format spec lets a document with no shared rows omit it.

Rows are written by kind, and within a kind in key order.
Section 8 makes the catalog the one place order carries nothing.
Writing them in the order the walk happened to find them would reshuffle a file on every export.
A diff of two exports of one workspace would then show every row as changed.
Ordering by key is stable because a key is derived from the row's own content.

An author, an image and a source are written whole here rather than delegated.
A row that is a single tag is not worth a class, and the catalog is where its shape is decided.
A source is delegated, because a source has children and credits authors.
An example is delegated for the same reason.

## `private static void LMarkupDraftAppend(StringBuilder text, LMarkup.LMarkupEntry entry, IReadOnlyDictionary<string, string> keys)`

Writes one `<entry>`, its own fields first and then its cards.

The entry declares its key so that a translation in another entry can name it.
Section 4 of the format spec calls the key needed only when something points at the entry.
Writing it always costs one attribute and saves the writer from predicting who will point.

`<headword>`, `<lang>` and `<note>` are plain text, and an empty one is written as no tag at all.
Section 6 keeps an absent tag and an empty one apart, and a plain field has only the one state.
Writing an empty tag would claim the field is unreadable, which a plain field cannot be.

Forms, parts of speech, inflections and the pronunciation are written between the note and the cards.
Each is written in the entry's own order, which section 8 makes meaningful.

## `private static void LMarkupDraftAppend(StringBuilder text, LForm form)`

Writes one `<form>`, its role and its localized label as attributes.
A form with no text is not a form and writes nothing.

## `private static void LMarkupDraftAppend(StringBuilder text, LSpeechDraft speech)`

Writes one `<pos>`, as an id when a language pack names it and as text when the user typed it.
The two forms are what keep a lookup and a custom name apart on the way back in.
Writing a language-pack value as its display name would import it as a custom name.

## `private static void LMarkupDraftAppend(StringBuilder text, LInflection inflection)`

Writes one `<inflection>`, its `<text>` first and then one `<feature>` per feature.
The features keep the order the inflection holds them in.
An inflection with no form writes nothing, because the form is the whole of it.

## `private static void LMarkupDraftAppend(StringBuilder text, LPronunciationDraft? pronunciation)`

Writes the `<pronunciation>` block, or nothing when the entry records none.
The level is an attribute of the block and the reading, syllables, representations and audio are its children.
The recording's `source` is written as an attribute of `<audio>`, as section 4 requires.
The audio's added time is workspace bookkeeping and is never written.

## `private static void LMarkupDraftAppend(StringBuilder text, LSyllable syllable)`

Writes one `<syllable>` as attributes alone, since a syllable is a set of segments and no text.
A syllable with no nucleus is one the reader would refuse, so it is not written.

## `private static void LMarkupDraftAppend(StringBuilder text, LRepresentation representation)`

Writes one `<representation>`, its system and role as attributes and the word as its text.
A representation with no text says nothing and is not written.

## `private static List<KeyValuePair<string, TRow>> LMarkupDraftSort<TRow>(IReadOnlyDictionary<string, TRow> rows)`

Puts one kind of catalog row into key order.
A dictionary has no order of its own, and an export must be the same file twice running.
