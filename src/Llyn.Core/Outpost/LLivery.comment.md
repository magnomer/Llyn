# LLivery.cs
Hash: `791b17789a18977b`

## `public interface LLivery`

The port for how an entry looks in Joplin, so Joplin shows an entry as view mode does.
Its adapter owns the theme, the rules, the page markup and the hashing.
The engine pushes the bodies and never learns how they are written.

## `string LLiveryRead();`

The whole body of the style note, as Markdown.
A line warns that Llyn writes the note and overwrites edits, so nobody tunes it by hand in vain.
The CSS follows in a fenced `css` block, which Joplin applies to the notes that import it.
The CSS never holds three backticks, so the fence cannot close early.

## `LLiveryNote LLiveryFormat(LLiveryPage page, string style, Func<long, string> note, Func<string, string, string, string> link, Func<string, string> lookup);`

One entry's note body, written as Markdown from `page`, the stored data view mode shows.
`note` maps an entry id to its Joplin note id.
`link` maps a language, a kind and a key to the id of a reconstruction note.
It answers empty for a note this push does not write, so that chip stays plain.
`lookup` maps a localization key to the text view mode shows.
The body imports the style note by id, so a theme change touches one note instead of every entry.
Pictures leave the body as parcels, since Joplin shows images only as its own resources.

## `LLiveryNote LLiveryFormat(LLiveryStem stem, string style, Func<long, string> note, Func<string, string> lookup);`

One series note body, written as Markdown from `stem`.
It imports the same style note and opens with the same mark as an entry body.
So the trash check proves a series note as Llyn's own too.
`note` and `lookup` mean what they mean for an entry body.

## `LLiveryNote LLiveryFormat(LLiveryDiwei diwei, string style, Func<long, string> note, Func<string, string> lookup);`

One rime-table category note body, written as Markdown from `diwei`.
It shares the style import, the mark and the meaning of `note` and `lookup` with an entry body.

## `string LLiveryMarkFormat(string style);`

The first line of every note body Llyn writes, the import of the style note `style`.
A note in Llyn's notebook whose body starts with it is proven to be Llyn's own.
So only such a note may ever be trashed.

## `string LLiveryIdFormat(string seed);`

A fixed Joplin id from `seed`, the first 32 hex characters of the SHA-256 of its UTF-8 bytes.
The same seed always yields the same id, so each push overwrites the same note.
The realm in an entry's or reconstruction note's seed separates independently created workspaces.
A folder copied outside Llyn keeps it and pushes onto the same notes.

## `string LLiveryDigestFormat(LOutpostNote note, IReadOnlyList<string> tags, IReadOnlyList<LParcel> parcels);`

Hashes everything a note carries into Joplin, so any change to it pushes the note again.
The answer is the whole SHA-256 in lowercase hex, which the manifest keeps per note id.
Tags are sorted here, so their order never forces a push.
