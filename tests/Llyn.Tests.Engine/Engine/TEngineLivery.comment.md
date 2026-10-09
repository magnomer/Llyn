# TEngineLivery.cs
Hash: `edab8f2d4a0755a0`

## `public sealed class TEngineLivery`

Writes stored and hand-built pages through the `TLiveryFormat` relay, one fact per rendered part.
The page-read facts live in `TEngineLiveryPage`.

## `public void LiveryFormat_StoredEntry_OmitsExportMarkup()`

Writes a stored entry's body through `TLiveryFormat`.
Checks the `.llyn` wrapper opens with a blank line after it.
Checks none of the export sheet's `portrait`, `crest` or `band` classes appears.

## `public void LiveryFormat_FullHeader_RendersEveryPart()`

Saves a favorite entry with a grasp, a pronunciation and a speech, then writes its body.
Checks the headword, language chip, filled heart, half star, rating key, speech chip and pronunciation.

## `public void LiveryFormat_TonedContour_DrawsChartPerSyllable()`

Saves a Mandarin entry, then gives the read page a two-syllable contour through `with`.
Writes the body through `TLiveryFormat` and checks the contour span follows the accent span.
Checks the body holds no inline `<svg>`, since Joplin's sanitizer strips it.
Checks two chart images point at two SVG parcels, and one parcel ends with the last syllable's text.
Checks that chart's gradient stop paints the theme's `contourBottom` colour.
Checks the other chart's dot paints the theme's `contourMid` colour.

## `public void LiveryFormat_StoredRecording_AddsAudioParcel()`

Writes a small mp3 file in the workspace folder and points the first pronunciation at it.
Writes the note through `TLiveryFormat` and checks the one audio parcel's media type and title.
Checks the body holds a Markdown link to that parcel and no `<audio>` tag.

## `public void LiveryFormat_MissingRecording_OmitsPlayer()`

Points the first pronunciation at a file that does not exist.
Checks the note carries no parcel and no player, while the pronunciation still prints.

## `public void LiveryFormat_StoredEtymon_RendersNoteLink()`

Saves a target entry and a second entry, then gives the second page the target as an etymon through `with`.
Writes the body through `TLiveryFormat` and checks the etymology heading and the etymon's `:/` link.

## `public void LiveryFormat_OutsideEtymon_KeepsPlainText()`

Gives a stored page an etymon whose note id `TLiveryFormat` answers empty.
Checks the etymon prints as plain text with its language chip and never as a link.

## `public void LiveryFormat_RepeatedRimeBook_OmitsBookChip()`

Groups two rows of one book from two sources through `TFanqieGroupScan` and puts them on a stored page.
Checks the book chip prints once while both source badges print.

## `public void LiveryFormat_FullPage_KeepsViewOrder()`

Gives a stored page a paradigm row, a rime group with a series, a linked etymology and a note.
Checks the paradigm, series, book chip, etymology, mention link, note and both stamps print in view mode's order.

## `public void CourierNoteBuild_OutsideEntry_ReturnsEmpty()`

Builds the courier's `note` map through the `TCourierNoteBuild` relay over one entry.
Checks that the entry's id maps to a 32-character note id and any other id maps to empty.

## `public void LiveryStyleFormat_TokenTheme_ScopesRulesWithoutColour()`

Builds a theme that maps every role `TLiveryStyleFormat` reads to a `var` token.
The roles include the favorite, helper, raised surface and frequency band colours.
A role missing from the theme would fall back to a hex colour and fail the fact.
Checks every selector of the CSS starts with `.llyn`.
Checks the CSS holds no hex, `rgb`, `hsl` or named colour.
