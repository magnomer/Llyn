# Livery

This folder holds the livery adapter, its style note, and the bodies of the series and rime-table category notes.

## `LLiverySheet`

The adapter behind the livery port.
It writes the style note, the entry bodies and the reconstruction bodies.
The entry body is Markdown, so a note looks like the entry in view mode.
The theme is taken once, so no push re-reads the embedded resource.

### Body order

The style import from `LLiveryMarkFormat` opens the body, since Joplin's import plugin only sees an opening import.
The `.llyn` wrapper follows with a blank line after it, so Joplin reads the Markdown inside.
The writers then run in view mode's order.
  - Header, then sound, then chips, then rime, then script, then cards, then etymology, then the stored note, then the stamps.

Only the rime writer takes `link`, since only its chips point at reconstruction notes.
The sound writer takes the theme, since its contour chart images carry their own colours.
Every fixed word comes through `lookup`, and every other word is stored page data.
`note` maps an entry id to its Joplin note id.

### `LLiveryStampAppend`

The stamps close the entry body, right before the closing div.
A rule, then the created and updated stamps in one paragraph split by a backslash break.
An empty stamp is left out, and two empty stamps write no rule.
They live with the sheet, since they describe the stored page, not a section of the entry.

### Media passes

Embedded pictures become resource links through `LLiveryImageApply`.
Recordings and videos follow through `LLiveryMediaApply`.
All passes fill one parcel map, so a repeated payload uploads once.
Joplin resolves `:/` resource addresses only in `<img>` tags and Markdown links.
An `<audio>` or `<video>` source would stay unresolved and play nothing.
A Markdown link to an audio or video resource gets Joplin's own player.
The `LLiveryAudioHead` and `LLiveryVideoHead` constants are shared with their writers, so writer and swap never drift apart.

### `LLiveryRead`

Writes the warning line, a blank line, and the style CSS in a `css` fence.
Every rule sits under `.llyn`, since Joplin applies the style to its whole viewer page.
Backticks are stripped from the CSS, so theme text can never close the fence early.

### `LLiveryFormat`

The series and rime-table overloads write through `LLiverySheetBuild`.
They check their arguments first, so a null part throws before any text is written.
A reconstruction note holds no pictures or recordings, so it answers no parcels.

### `LLiveryMarkFormat` and `LLiveryStyleCheck`

The mark is the style import line that opens every body.
A style id must be 32 lowercase hex characters, the only form Joplin resolves.
A bad id would otherwise silently leave the entry unstyled.
So a bad id throws `ArgumentException` here as in the body.

### `LLiveryIdFormat`

The id is the first 32 hex characters of the SHA-256 over the UTF-8 bytes of the seed.
These are the bytes the courier hashed before, so no pushed note changes its id.

### `LLiveryDigestFormat`

It hashes the title, folder and body, then the sorted tags, then the parcel ids.
A null character ends each field and `\u0001` ends each tag or parcel id.
No text holds those separators, so one field's end never passes for another's start.
The bytes match the courier's former digest, so the manifest's digests stay valid.

### `LLiveryImageApply`

It swaps each embedded picture for a link to a Joplin resource with the same bytes.
The id is a hash of the bytes, so a repeated picture uploads once.
The address is decoded from its HTML escaping first, so the parser sees the real media type.
A non-picture media type or an oversized payload becomes an empty `llyn-blank` span.
Joplin would otherwise store a non-picture as a resource or refuse an oversized upload.
An address that fails to parse stays as written, and Joplin shows a broken picture.

### `LLiveryMediaApply`

The player ends at the first tail after its path.
The path is decoded from its HTML escaping first, so the file read sees the real path.
A path the loader cannot read loses its whole player and the space before it.
So a missing recording or video leaves its row or card as if none were stored.

### Loaders

Audio, video and picture limits are 32, 128 and 24 megabytes.
The media type comes from the extension.
Audio accepts mp3, wav, ogg, m4a, flac and opus.
Video accepts mp4, m4v, webm, ogv and mov.
`LLiveryFileLoad` answers null for an empty media type, a relative path, a missing file or an oversized file.
An IO or access failure also answers null, so a locked file only drops its player.

## `LLiveryStyle`

It writes the CSS of the style note and is public so engine tests can call it without the sheet.
Every selector starts with `.llyn`, so no rule reaches Joplin's own page.
Every colour comes from `LTheme.LThemeRead`, so no colour is a literal.
`LThemeRead` falls back to the spare palette, so a theme missing a role still yields CSS.
A role missing from both reads as black, which a literal-colour test would catch.
It styles the classes the livery writers emit, Joplin's player and icon classes, and bare tags inside `.llyn`.

### Joplin quirks handled

Recordings and videos are styled through the classes of Joplin's own player.
The resource link hides, so a recording shows only a small player.
Joplin's icon before every note or resource link is hidden, so entry links read as in Llyn.
A table whose header cells are all empty hides its header row.
The writers give every Markdown table such a header, since Markdown needs one.
Such tables also lose Joplin's borders and fills.

### Table kinds

The `reading` rule matches the seven-column readings tables, found through the `reflex` header test.
The `card` rule matches card tables other than readings, so a series member card keeps the entry's sizes.
Its `:where` adds no weight, so card rules keep the weight of `.llyn .llyn-card table`.
The `sound` rule matches the two-column sound rows outside the paradigm box.
A `llyn-tone` span is superscript, except inside a card table, where the rime card shows it as text.

### Audio and contour

The audio player sits 9 pixels below the baseline.
That centres its 32-pixel height 7 pixels above the baseline, level with the reading's middle.
The `middle` value would centre it on the smaller cell text, below the reading's middle.
The contour box keeps a 10-pixel margin above it.
Contour chart images scale up by half and carry their own colours.
The `llyn-axis` block is an inline-flex column, so the contour box centres on the accent above it.
Its accent line keeps the 37-pixel row height.
The label cell of that row sits at the top, on the same line.
The player beside the block keeps its baseline, which is the accent's.

### Regard row

The `llyn-regard` row centres the heart, stars and rating on one line, as view mode does.
Its margins are set against the glyphs' own side bearings.
The visible gaps read 17 pixels after the heart, 7 between stars and 13 before the rating.
The heart and star glyphs sit low in their boxes, so they rise 0.5 and 1.5 pixels.
Those figures were measured in Chromium at the theme's Segoe UI.
A faded star lies under a full `accent` star, as in view mode.
A rated entry's empty star is `accent` at 55 percent over `surface`.
An unrated entry's star is `muted` at 35 percent over `surface`.
A half star overlays its left half in full `accent` through `::before`.

### Chips

The frequency chip and the paradigm box fill with `surfaceRaised`.
The unit and freedom chips are outlined in `accentSoft` with `muted` text.
That sets them apart from the filled speech chips.
Each frequency band class colours the chip text and its lit pips with the band's theme colour.
The register chip takes `helper`, `helperSoft` and `helperEdge`.

### Cards and folds

A card whose first paragraph holds a `llyn-number` turns that paragraph into a ruled header.
A `details` card does the same with its `summary` and hides the native marker.
A `muted` chevron at the right end replaces it, mirroring the app's right-aligned hinge.
It is the only fold affordance a card shows in Joplin.
A folded card drops the rule under its summary and the space below it.
A `llyn-collocations` block is a bordered box with 8-pixel corners.
A `llyn-heading` span inside it is a muted caption, not a pill.
That rule is scoped to `llyn-collocations`, so every other heading keeps the pill.
Each `llyn-collocation` row is ruled from the one above.
Its summary leads with a small chevron, pointing right while closed and down while open.

### Etymology tree

A `.lineage` block draws the etymology tree with the lineage rules of `LSheetCascade`, scoped under `.llyn`.
Its colours come from the palette.

### Reconstruction notes

A `llyn-vacant` span is the muted text for an empty part.
A `llyn-diwei` div is boxed like a card but keeps its characters left-aligned.
A `llyn-rounded` mark takes `situation`, the amber the rime table gives it.
A `llyn-mark` chip is a tally mark with its count in a muted `llyn-count`.

### Pictures

An empty `llyn-blank` span is the box left for a dropped picture.

### Inflection

A `llyn-inflection` table has a solid rule under its header and above each group, and dotted rules between lines.
A marked run takes `warning`, the colour the app gives a marked stretch.
The cut hyphen and the glyph of a missing form are `muted`.

The note mirrors the app's Short/Full switch without script, through the `llyn-inflection-full` details.
The `llyn-inflection-box` fills with `surface` and takes a `line` border and a 12-pixel corner.
It never shares `llyn-paradigm`, whose `surfaceRaised` fill belongs to the rime box.
The summary is a block as wide as its pill, pushed right by an auto left margin.
The expanded table therefore renders below it.
The caption of the current state takes `accent` on `surface` with a `line` border.
The other caption stays `muted` and turns `ink` on hover.
A closed details makes Short current, and an open one makes Full current.
The `llyn-inflection-short` table right after an open details hides, so one sheet shows at a time.

## `LLiveryXiesheng`

It writes the body of a series note.
It also holds the character chip and the entry list the category note shares.

### `LLiveryXieshengAppend`

It writes a series chip and the series key as the heading.
Members follow in page order, each in its own card.
A series without characters writes `Xiesheng.StemEmpty` as a `llyn-vacant` span.
Entries of the series come last, leaving out any entry whose headword is a member's character.
So every member appears exactly once in the note.
`Xiesheng.KindredVacant` is written only when the series holds no entries at all.

### `LLiveryMemberAppend`

A member with reflex rows opens through `LLiveryFoldFormat`, with a top right chevron.
The fold is open when the ready opened flag is set, as the Series pane shows it.
The summary holds the character and the reading, so a folded member keeps both in view.
The reading span is left out when the member has no reading.
The reflex rows form the folded body, with no fanqie rows, so the old sound column stays empty.
A member without reflex rows stays a plain div, since it has nothing to fold.

### `LLiveryCharacterFormat`

Joplin parses no Markdown inside a member's `<summary>` line, so the chip uses an HTML anchor.
The link targets the entry whose headword equals the character, the lowest id first.
A character without such an entry, or with an empty note id, stays plain text.
`LLiveryYunjing` reuses it for the characters of each line.

### `LLiveryEntryAppend`

It writes one list line per entry in the order the record holds, its headword linked.
No entries write `vacant` as a span, or nothing when `vacant` is empty.
`LLiveryYunjing` passes an empty `vacant`, since a category note has no text for that case.

## `LLiveryYunjing`

It writes the body of a rime-table category note.
The category key is the heading, and a tone key goes through `Display.FanqieTone`, as the rime card prints tone labels.
A page without sections writes `Yunjing.DiweiEmpty` as a `llyn-vacant` span.
Sections follow in page order, then the entries of the category.

### `LLiverySectionAppend`

A section is a four-column table inside a `llyn-diwei` div.
A non-empty label stands above it as a heading chip.
The header row is empty, so the table shows no column titles.
Lines come first in page order, then the tally rows.

### Row writers

`LLiveryLineFormat` holds the reading, label, rounded mark and characters.
The rounded mark is the glyph the rime table shows and stays empty for an open line.
Each character goes through `LLiveryCharacterFormat`, so a stored character links to its entry.
`LLiveryTallyFormat` holds language and kind chips, an empty cell and the marks.
The empty cell keeps the marks in the column the characters use.

## Stamp

Commit: `aae5349dbb94`
- `LLiverySheet.cs` `960715135a175c55`
- `LLiveryStyle.cs` `c5e268204a726c80`
- `LLiveryXiesheng.cs` `7fe0c4fa2c46ae6e`
- `LLiveryYunjing.cs` `fdb8c41af05a7c10`
