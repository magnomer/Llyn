# LLiveryStyle.cs
Hash: `e0a76dabc146025f`

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
The writers give every table such a header, since Markdown needs one.
The local `table` matches those tables, and drops Joplin's borders and fills on them.
The local `reading` matches the five-column readings tables of `LLiverySound`.
The local `sound` matches its two-column sound rows, outside the paradigm box and the `llyn-phonology` div.
It chains two simple `:not` clauses, since older engines reject a selector list inside `:not`.
A `llyn-tone` span is superscript, except inside a `llyn-card`, where the rime card shows it as text.
The `llyn-more` summary loses its marker and shows only its text.
The contour chart images scale up by half.
Their colours sit inside each image, so no rule here paints them.
The heart that is on takes `favorite`.
The stars follow view mode, which lays a faded star under a full `accent` star.
A rated entry's empty star is `accent` at 55 percent over `surface`, through `color-mix`.
An unrated entry's star is `muted` at 35 percent over `surface`.
A filled star is full `accent`.
A half star overlays its left half in full `accent` through `::before`.
The frequency chip and the paradigm box fill with `surfaceRaised`.
The unit chip is outlined in `accentSoft` with `muted` text, so it reads apart from the filled speech chips.
Each frequency band class colours the chip text and its lit pips with that band's theme colour.
The register chip takes `helper`, `helperSoft` and `helperEdge`, as the situation chip takes its own three.
A role missing from the theme and the spare palette reads as black, which a literal-colour test would catch.
A card whose first paragraph holds a `llyn-number` turns that paragraph into a ruled header.
An empty `llyn-blank` span is the box `LLiverySheet` leaves for a dropped picture.
A `llyn-vacant` span is the muted text a reconstruction note shows for an empty part.
A `llyn-diwei` div is boxed like a card, but its characters stay left-aligned.
Its first column holds the reading, set bold as the rime table does.
A `llyn-rounded` mark takes `situation`, the amber the rime table gives it.
A `llyn-mark` chip is a tally mark, with its count in a muted `llyn-count`.
A `llyn-phonology` table keeps headwords at body size and mutes the sounds.
