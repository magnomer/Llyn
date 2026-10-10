# LLiverySheet.cs
Hash: `8668bec83c78091f`

## `public sealed class LLiverySheet : LLivery`

The adapter behind the livery port, writing the style note, the entry bodies and the reconstruction bodies.
The style note comes from `LLiveryStyle.LLiveryStyleFormat` over the theme it is handed.
The entry bodies come from `LLiveryPage` as Markdown, so a note looks like the entry in view mode.

## `private const string LLiveryImageHead = "<img src=\"data:";`

The start of every embedded picture the sheet writes, which the body swaps for a resource link.

## `private const long LLiveryImageCeiling = 24L * 1024L * 1024L;`

The same size limit the sheet puts on a picture file, so an embedded address cannot exceed it.

## `internal const string LLiveryAudioHead = "<audio controls class=\"llyn-audio\" src=\"";`

The start of every player `LLiverySound` writes, which the body swaps for a Markdown resource link.
Both files read this one constant, so the writer and the swap never drift apart.

## `private const long LLiveryAudioCeiling = 32L * 1024L * 1024L;`

The largest recording file sent to Joplin, beside the picture limit.

## `internal const string LLiveryVideoHead = "<video controls class=\"llyn-video\" src=\"";`

The start of every video player `LLiveryCard` writes, which the body swaps for a Markdown resource link.
Both files read this one constant, so the writer and the swap never drift apart.

## `private const long LLiveryVideoCeiling = 128L * 1024L * 1024L;`

The largest video file sent to Joplin, beside the recording limit.

## `public LLiverySheet(LTheme theme)`

Takes the theme once, so no push re-reads the embedded resource.

## `public string LLiveryRead()`

The warning line, a blank line, and the CSS of `LLiveryStyle.LLiveryStyleFormat` inside a `css` fence.
That CSS reads the theme handed to the constructor.
Every rule sits under `.llyn`, since Joplin applies the style to its whole viewer page.
Backticks are stripped from the CSS, so theme text can never close the fence early.
No rule uses a backtick, so only stray theme text is touched.

## `public LLiveryNote LLiveryFormat(LLiveryPage page, string style, Func<long, string> note, Func<string, string, string, string> link, Func<string, string> lookup)`

The style import from `LLiveryMarkFormat` comes first, since Joplin's import plugin only sees an opening import.
Writing it through that method keeps the body and the trash check on one line of text.
The `.llyn` wrapper follows with a blank line after it, so Joplin reads the Markdown inside.
`LLiveryHeader.LLiveryHeaderAppend` writes the headword line first.
`LLiverySound.LLiverySoundAppend` writes the readings and sound rows next.
It takes the sheet's theme, since its contour chart images carry their own colours.
`LLiveryHeader.LLiveryChipAppend` writes the speech and frequency chips next.
`LLiveryRime.LLiveryRimeAppend` writes the paradigm box and the rime card after the chips.
It alone takes `link`, since only the rime card's chips point at reconstruction notes.
`LLiveryScript.LLiveryScriptAppend` writes the script card after the rime card.
`LLiveryCard.LLiveryCardAppend` writes the meanings, collocations and incoming usages next.
`LLiveryEtymology.LLiveryEtymologyAppend` writes the etymology, the note and the stamps last, as view mode orders them.
Every fixed word comes through `lookup`, and every other word is stored page data.
`note` maps an entry id to its Joplin note id.
`LLiveryCardAppend` and `LLiveryEtymologyAppend` read it for entry links.
Embedded pictures become resource links through `LLiveryImageApply`.
Stored recordings then become resource links through `LLiveryMediaApply` with `LLiveryAudioLoad`.
Stored videos follow through `LLiveryMediaApply` with `LLiveryVideoLoad`.
All passes fill one parcel map, so a repeated payload uploads once.
Blank lines stay, since the Markdown needs them.

## `public LLiveryNote LLiveryFormat(LLiveryStem stem, string style, Func<long, string> note, Func<string, string> lookup)`

Writes a series note through `LLiverySheetBuild` and `LLiveryXiesheng.LLiveryXieshengAppend`.
It checks its arguments first, so a null part throws before any text is written.

## `public LLiveryNote LLiveryFormat(LLiveryDiwei diwei, string style, Func<long, string> note, Func<string, string> lookup)`

Writes a rime-table category note through `LLiverySheetBuild` and `LLiveryYunjing.LLiveryYunjingAppend`.
It checks its arguments first, so a null part throws before any text is written.

## `public string LLiveryMarkFormat(string style)`

The style import line that opens every note body, with no line break.
It checks `style` first, so a bad id throws `ArgumentException` here as in the body.
The entry `LLiveryFormat` and `LLiverySheetBuild` write the first line through it, so the mark never drifts.

## `public string LLiveryIdFormat(string seed)`

The first 32 hex characters of the SHA-256 over the UTF-8 bytes of `seed`.
These are the same bytes the courier hashed before, so no pushed note changes its id.

## `public string LLiveryDigestFormat(LOutpostNote note, IReadOnlyList<string> tags, IReadOnlyList<LParcel> parcels)`

Joins the title, folder and body, then the sorted tags, then the parcel ids, and hashes the UTF-8 bytes.
A null character ends each field and `\u0001` ends each tag or parcel id.
No text holds those separators, so one field's end never passes for another's start.
The bytes match the courier's former digest, so the manifest's digests stay valid.

## `private LLiveryNote LLiverySheetBuild(string style, Action<StringBuilder> append)`

Wraps what `append` writes in the same style mark and `.llyn` div an entry body has.
The mark comes from `LLiveryMarkFormat`, so a bad style id throws here too.
A reconstruction note holds no pictures or recordings, so it answers no parcels.

## `private static void LLiveryStyleCheck(string style)`

Rejects any id that is not 32 lowercase hex characters, the only form Joplin resolves.
A bad id would otherwise reach the import and silently leave the entry unstyled.

## `private static string LLiveryImageApply(string body, Dictionary<string, LParcel> parcels)`

Swaps each embedded picture for a link to a Joplin resource holding the same bytes.
The id comes from a hash of the bytes, so a repeated picture uploads once.
The same data-address parser the sheet uses is reused here for the media type and suffix.
The address is decoded from its HTML escaping first, so the parser sees the real media type.
A non-picture media type or an oversized payload becomes an empty `llyn-blank` span instead.
`LLiveryStyle.LLiveryStyleFormat` draws that span as an empty picture box.
Joplin would otherwise store a non-picture as a resource or refuse an oversized upload.
An address that fails to parse stays as written, which Joplin then shows as a broken picture.

## `private static string LLiveryMediaApply(string body, Dictionary<string, LParcel> parcels, string head, string tail, Func<string, LParcel?> load)`

Swaps each player that opens with `head` for a Markdown link to a Joplin resource.
The player ends at the first `tail` after its path.
Joplin resolves `:/` resource addresses only in `<img>` tags and Markdown links.
An `<audio>` or `<video>` source stays unresolved and plays nothing.
A Markdown link to an audio or video resource gets Joplin's own player.
The path is decoded from its HTML escaping first, so the file read sees the real path.
A path `load` cannot read loses its whole player and the space before it.
So a missing recording leaves the accent row as if no recording were stored.
A missing video file leaves its card as if no video were stored.

## `private static LParcel? LLiveryAudioLoad(string path)`

Reads one stored recording through `LLiveryFileLoad` under `LLiveryAudioCeiling`.
The media type comes from the extension, one of mp3, wav, ogg, m4a, flac or opus.
The title is `audio` with the extension.

## `private static LParcel? LLiveryVideoLoad(string path)`

Reads one stored video file through `LLiveryFileLoad` under `LLiveryVideoCeiling`.
The media type comes from the extension, one of mp4, m4v, webm, ogv or mov.
The title is `video` with the extension.

## `private static LParcel? LLiveryFileLoad(string path, string media, string title, long ceiling)`

Reads one stored file into a parcel of type `media` and title `title`, or answers null.
An empty media type, a relative path, a missing file or a file above `ceiling` answers null.
An IO or access failure while reading also answers null, so a locked file only drops its player.
The id is a hash of the bytes, as for pictures.
