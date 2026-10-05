# LLivery.cs
Hash: `9bc6ccb47722539a`

## `public interface LLivery`

The port for how an entry looks in Joplin, so Joplin shows an entry as Llyn's export does.
`LLiverySheet` in Infrastructure is its adapter, which owns the theme, the rules and the page markup.
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
