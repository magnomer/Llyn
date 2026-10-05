# LLivery.cs
Hash: `92185ee26454dece`

## `public interface LLivery`

The port for how an entry looks in Joplin, so Joplin shows an entry as Llyn's export does.
Its adapter owns the theme, the rules, the page markup and the hashing.
The engine pushes the bodies and never learns how they are written.

## `string LLiveryRead();`

The whole body of the style note, as Markdown.
A line warns that Llyn writes the note and overwrites edits, so nobody tunes it by hand in vain.
The CSS follows in a fenced `css` block, which Joplin applies to the notes that import it.
The CSS never holds three backticks, so the fence cannot close early.

## `LLiveryNote LLiveryFormat(LPortraitPage page, string style);`

One entry's note body, written from the same page likeness the HTML export prints.
The body imports the style note by id, so a theme change touches one note instead of every entry.
Pictures leave the body as parcels, since Joplin shows images only as its own resources.

## `string LLiveryIdFormat(string seed);`

A fixed Joplin id from `seed`, the first 32 hex characters of the SHA-256 of its UTF-8 bytes.
The same seed always yields the same id, so each push overwrites the same note.
The realm in an entry's seed separates independently created workspaces.
A folder copied outside Llyn keeps it and pushes onto the same notes.

## `string LLiveryDigestFormat(LOutpostNote note, IReadOnlyList<string> tags, IReadOnlyList<LParcel> parcels);`

Hashes everything a note carries into Joplin, so any change to it pushes the note again.
The answer is the whole SHA-256 in lowercase hex, which the manifest keeps per note id.
Tags are sorted here, so their order never forces a push.
