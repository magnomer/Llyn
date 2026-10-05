# LLiverySheet.cs
Hash: `ab1d2a703589d9a8`

## `public sealed class LLiverySheet : LLivery`

The adapter behind the livery port, writing the style note and the entry bodies from the sheet's own rules.
It is built over the theme the exported page wears.
So an entry in Joplin and an exported page look alike.

## `private const string LLiveryImageHead = "<img src=\"data:";`

The start of every embedded picture the sheet writes, which the body swaps for a resource link.

## `private const long LLiveryImageCeiling = 24L * 1024L * 1024L;`

The same size limit the sheet puts on a picture file, so an embedded address cannot exceed it.

## `public LLiverySheet(LTheme theme)`

Takes the theme once, so no push re-reads the embedded resource.

## `public string LLiveryRead()`

The warning line, a blank line, and the sheet scoped under `.llyn` inside a `css` fence.
Every rule sits under `.llyn`, since Joplin applies the style to its whole viewer page.
Backticks are stripped from the CSS, so theme text can never close the fence early.
No rule uses a backtick, so only stray theme text is touched.

## `public LLiveryNote LLiveryFormat(LPortraitPage page, string style)`

The style import comes first, because Joplin's import plugin only sees an import that opens its block.
The sheet's own body writer follows inside a `.llyn` wrapper, so the export and the note never drift apart.
Embedded pictures become resource links, and blank lines are folded away last.

## `private static void LLiveryStyleCheck(string style)`

Rejects any id that is not 32 lowercase hex characters, the only form Joplin resolves.
A bad id would otherwise reach the import and silently leave the entry unstyled.

## `private static string LLiveryImageApply(string body, Dictionary<string, LParcel> parcels)`

Swaps each embedded picture for a link to a Joplin resource holding the same bytes.
The id comes from a hash of the bytes, so a repeated picture uploads once.
The same data-address parser the sheet uses is reused here for the media type and suffix.
The address is decoded from its HTML escaping first, so the parser sees the real media type.
A non-picture media type or an oversized payload becomes the sheet's blank placeholder instead.
Joplin would otherwise store a non-picture as a resource or refuse an oversized upload.
An address that fails to parse stays as written, which Joplin then shows as a broken picture.

## `private static string LLiveryLineApply(string body)`

Joplin ends an HTML block at the first blank line and reads what follows as Markdown.
Carriage returns become line feeds first, since Joplin counts a lone one as a line break too.
Each blank line is folded into the line before it as an encoded line break.
So text inside a code block keeps its breaks while the whole wrapper stays one HTML block.
