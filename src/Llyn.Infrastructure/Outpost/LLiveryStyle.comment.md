# LLiveryStyle.cs
Hash: `2634b6a664c70f6d`

## `public static class LLiveryStyle`

Writes the CSS of the Joplin style note for `LLiverySheet.LLiveryRead`.
It reads only the `LTheme` it is handed.
It is public so the engine tests can call it without the sheet.

## `public static string LLiveryStyleFormat(LTheme theme)`

`LLiverySheet.LLiveryRead` calls it and fences the result.
Every selector starts with `.llyn`, so no rule reaches Joplin's own page.
Every colour comes from `LTheme.LThemeRead`, so no colour is written as a literal.
`LThemeRead` falls back to the spare palette, so a theme missing a role still yields CSS.
The family and the serif come from `LThemeFamily` and `LThemeSerif`.
It styles only the classes the livery writers emit, plus bare tags inside `.llyn`.
Recordings and videos are the exception, styled through the classes of Joplin's own player.
Joplin renders a resource link and puts its player right after it.
The link hides, so a recording shows only a small player beside its reading.
Joplin also puts its own icon in front of every link to a note or resource.
That icon hides, so a link to another entry reads as in Llyn.
A table whose header cells are all empty hides its header row.
The writers give every Markdown table such a header, since Markdown needs one.
The local `table` matches those tables, and drops Joplin's borders and fills on them.
The local `reading` matches the seven-column readings tables of `LLiverySound`, through the `reflex` header test.
Its labels and varieties are 14 pixels, its readings 20, and every later column 13.
The local `card` matches card tables other than readings, so a series member card keeps the entry's sizes.
Its `:where` adds no weight, so the card rules keep the weight of `.llyn .llyn-card table`.
The local `sound` matches its two-column sound rows, outside the paradigm box.
A `llyn-tone` span is superscript, except inside a `card` table, where the rime card shows it as text.
The `llyn-more` summary loses its marker and shows only its text.
The audio player sits 9 pixels below the baseline rather than at `middle`.
`middle` centres it on the 15-pixel cell text, below the middle of the 18-pixel reading beside it.
At 9 pixels down, its 32-pixel height centres 7 pixels above the baseline, level with the reading's middle.
The contour box keeps a 10-pixel margin above it, so it never touches the player in the row above.
The contour chart images scale up by half.
Their colours sit inside each image, so no rule here paints them.
The `llyn-regard` row centres the heart, the stars and the rating on one line, as view mode does.
Its margins are set against the glyphs' own side bearings.
So the visible gaps read 17 pixels after the heart, 7 between stars and 13 before the rating.
The heart and star glyphs sit low in their boxes, so they rise 0.5 and 1.5 pixels.
Their ink then centres within a tenth of a pixel of the rating's capital height.
Those figures were measured in Chromium, which Joplin renders with, at the theme's Segoe UI.
The heart that is on takes `favorite`.
The stars follow view mode, which lays a faded star under a full `accent` star.
A rated entry's empty star is `accent` at 55 percent over `surface`, through `color-mix`.
An unrated entry's star is `muted` at 35 percent over `surface`.
A filled star is full `accent`.
A half star overlays its left half in full `accent` through `::before`.
The frequency chip and the paradigm box fill with `surfaceRaised`.
The unit and freedom chips are outlined in `accentSoft` with `muted` text.
That sets them apart from the filled speech chips.
Each frequency band class colours the chip text and its lit pips with that band's theme colour.
The register chip takes `helper`, `helperSoft` and `helperEdge`, as the situation chip takes its own three.
A role missing from the theme and the spare palette reads as black, which a literal-colour test would catch.
A card whose first paragraph holds a `llyn-number` turns that paragraph into a ruled header.
Any `details` card does the same with its `summary`, which hides its native marker.
A `muted` chevron at the right end replaces it, turned for the open and folded states.
The chevron mirrors the app's right-aligned hinge, and the native marker would sit at the left.
The chevron is the only fold affordance a card shows in Joplin.
A folded card drops the rule under its summary and the space below it.

A `llyn-collocations` block is a bordered box with 8-pixel corners.
The paragraph holding its `llyn-heading` count is ruled off from the rows below it.
A `llyn-heading` span inside the block is a caption, not a pill.
The caption is `muted` at 13 pixels and weight 600.
Its rule is scoped to `llyn-collocations` and resets the pill's box, fill and line height.
Every other `llyn-heading` span keeps the pill, with its `accentSoft` fill and `accent` text.
Each `llyn-collocation` row is ruled from the one above it.
Its summary leads with a small `muted` chevron, pointing right while closed and down while open.
The phrase is 16 pixels at weight 500, and the meaning follows it on the same line.

An empty `llyn-blank` span is the box `LLiverySheet` leaves for a dropped picture.
A `llyn-vacant` span is the muted text a reconstruction note shows for an empty part.
A `llyn-diwei` div is boxed like a card, but its characters stay left-aligned.
Its first column holds the reading, set bold as the rime table does.
A `llyn-rounded` mark takes `situation`, the amber the rime table gives it.
A `llyn-mark` chip is a tally mark, with its count in a muted `llyn-count`.
A `llyn-inflection` table drops Joplin's borders and fills, as the local `table` does.
A solid rule sits under its header and above each group, and a dotted one between lines.
Its group cell takes `accent` and its header and label cells stay `muted`.
A marked run takes `warning`, the colour the app gives a marked stretch.
The cut hyphen and the glyph of a missing form are `muted`, as the app shows them.
The note mirrors the app's Short/Full switch without script, through the `llyn-inflection-full` details.
The `llyn-inflection-box` mirrors the app's white box, so it fills with `surface`, as a card body does.
It takes a `line` border and a 12 pixel corner, as `Theme.Paradigm.Box` draws it.
It never shares `llyn-paradigm`, whose `surfaceRaised` fill belongs to the rime box.
Inside it a `llyn-inflection` table drops its bottom margin, as the box pads it.
The summary mirrors the app's right-aligned switch, on its own row above the sheet.
It is a block as wide as its pill, pushed right by an auto left margin.
The expanded table therefore renders below it.
The pill fills with `surfaceRaised`, as `Theme.Paradigm.Switch` does, and hides its native marker.
Its two captions sit side by side, like the app's radio pair.
The caption of the current state takes `accent` on `surface` with a `line` border.
That is the checked look of `Theme.Paradigm.Fold`.
The other caption stays `muted`, with a border in the pill's fill, and turns `ink` on hover.
A closed details makes Short current, and an open one makes Full current.
The `llyn-inflection-short` table right after an open details hides, so one sheet shows at a time.
