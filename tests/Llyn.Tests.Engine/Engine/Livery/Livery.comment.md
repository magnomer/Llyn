# Livery

This folder holds the tests of the Joplin page writes and reads of an entry or a language.
They cover cards, inflection, language and page.

## `TEngineLivery.cs`

It writes stored and hand-built pages through the `TLiveryFormat` relay, one fact per rendered part.
The page-read facts live in `TEngineLiveryPage`.
The inflection-table facts live in `TEngineLiveryInflection`.
The `.llyn` wrapper opens with a blank line, and no export sheet class of `portrait`, `crest` or `band` appears.
A full header prints the headword, language chip, heart, star, rating key, speech chip and pronunciation.
A page marked with a speech of no name writes no chip line.
So the chip writer neither cuts earlier text nor adds a blank line.
A toned contour draws one chart image per syllable, never an inline `<svg>`, since Joplin's sanitizer strips it.
The syllable falling to the bottom ends its gradient on `contourBottom`, and the other's dot paints `contourMid`.
A toned contour beside a stored recording shares one `llyn-axis` block with the accent.
The player link follows the block, outside it.
The style note lays that block out as a centred column.
So the plot centres on the reading and never on the player.
A stored recording becomes one audio parcel behind a Markdown link, with no `<audio>` tag.
A missing recording file leaves no parcel and no player, while the pronunciation still prints.
A rime book repeated across two sources prints its book chip once, while both source badges print.
A full page keeps view mode's order for paradigm, series, book chip, note and stamps.
The courier note map sends the entry's id to a 32-character note id and any other id to empty.

## `LiveryStyleFormat_TokenTheme_ScopesRulesWithoutColour`

Every selector of the CSS starts with `.llyn`, and no hex, `rgb`, `hsl` or named colour appears.
`TThemeStyleFormat` maps every role the style reads to a `var` token.
A role missing from the theme would fall back to a hex colour and fail the colour fact.

## `LiveryStyleFormat_CardSummary_DrawsFoldChevronAtRightEnd`

The chevron mirrors the app's right-aligned hinge.
The native webkit details marker would sit at the left, so the style hides it.

## `LiveryStyleFormat_OpenFullDetails_HidesShortTable`

The note mirrors the app's Short/Full switch without script, so CSS alone swaps the sheets.
The summary rule pushes the pill right with an auto left margin and right text alignment.

## `LiveryStyleFormat_ReflexTableInCard_KeepsReadingSizes`

No rule styles every cell or tone of a `llyn-card`.
The card cell rule skips the readings table, so a series member card matches the entry.

## `LiveryStyleFormat_CollocationHeading_LeavesHeadingPillAlone`

Exactly one rule has the bare `.llyn .llyn-heading` selector, keeping the `accentSoft` fill and `accent` text.
The collocations heading rule clears the fill and takes `muted`.
So the collocation caption never restyles a heading chip elsewhere.

## `TEngineLiveryCard.cs`

It writes Joplin bodies through the `TLiveryFormat` relay, one fact per script, card or incoming part.
`TLiveryPicture` is a one-pixel PNG in base64, so the picture parser reads a real image.
A script image becomes one PNG parcel, and style, image, caption and quote print in that order.
A meaning card keeps the order of heading, title, meaning, situation, register, translation, child, example, tag, image and video.
A stored banner maps Korean to a PNG file.
The target chip, incoming language and gloss each open with that flag.
An incoming usage links its entry's note under the incoming heading and carries the owner chip.
A video file becomes one video parcel behind a Markdown link, with no `<video>` tag left.
A stored open card is an open `details` block with its number chip inside a `summary`.
A folded card is a `details` block with a `summary` and no `open` attribute.
A card with no stored id stays a plain `div` with no `summary`.

## `TEngineLiveryInflection.cs`

It writes the inflection table of a stored page through the `TLiveryFormat` relay.
The note mirrors the app's Short/Full switch without script, so both sheets reach the body.
The box has its own `llyn-inflection-box` class, since the rime box's `llyn-paradigm` fill would leak in.
The `llyn-inflection-full` details holds the expanded table, and its summary carries both caption keys in switch spans.
The collapsed sheet's table follows the details, and no collapsed label sits inside it.
The block closes with the blank line Markdown needs.
An empty collapsed sheet writes one table without the switch.
Identical sheets write one table without the switch.
A marked split form writes the marked run and the cut hyphen spans.
A lost form writes a muted ellipsis, and an unknown form a muted dash, each titled with its tip key.
A null inflection view writes no inflection class.

## `TEngineLiveryLanguage.cs`

It calls the language read of `LEngineLiveryRead` through the `TLiveryRead` relay, one fact per part.
The `LiveryFormat` facts write those reads as notes through the `TLiveryFormat` relay.
`TLiveryLanguagePack` names one fanqie book and a series rule for the fetched and respelled facts.
`TLiveryLanguageBook` is a shipped language with a fanqie book, so its categories pass the book check.
`TLiveryLanguageTables` are the reconstruction tables `TLiveryLanguagePlace` hands to `TDiweiApply`, so the placed row also yields a tone.
`TLiveryLanguagePatience` is five seconds for the fetch `TLiveryLanguageSettle` watches.
A fetched series carries its one page and its one entry.
A placed character lists its initial, rime and tone categories in that order, each with kind, key and entry.
The anchor sits on a reflex row, so without the reflex every category reads empty.
Sections start unrespelled, and respelling must be on for the book.
With respelling and the tally on, the initial and rime sections carry both flags and the tally rows.
A reflex has no tone part to tally, so only those two sections hold them.
A language without books or series answers empty lists.
A series note links a stored character as an HTML anchor and leaves an unstored one plain.
Neither shows Markdown link syntax.
An opened stored member is an open `llyn-card` details block.
A folded member is a closed block, and a bare member a plain div.
Each block holds the member's reflex table, and the stored member's table equals its entry note's table.
Each member character appears exactly once, and no `llyn-more` block remains.
A respelled series row equals the entry note's row, picked by its language label.
The row shows slashed respelling, romanization, gloss and note, and no raw IPA.
The romanization is plain ASCII, since the escaper writes accented Latin letters as numeric entities.
A rime note prints sections in page order with readings, labels, the rounded mark and the tally.
A tone note shows the formatted tone heading and links the placed entry.

## `TEngineLiveryPage.cs`

It calls `LEngineLiveryRead` through the `TLiveryRead` relay, one fact per part group.
The folded-language format fact lives here too, since it shares the folding pack.
`TEngineLiveryReflex` folds Jin and leaves Wu open.
`TEngineLiveryFanqie` names one fanqie book, and `TEngineLiveryScript` names one script style.
`TEngineLiveryPages` are the pages the fake client serves to the script fetch.
A stored entry keeps its header, favorite, grasp and both stamps.
A missing entry reads null.
A folded reflex language keeps the folded list, the reflex order and the guise folded flags.
Only a folded stored card's id is in `LLiveryPageFold`.
A stored fanqie row keeps the one group `LEngineFanqieDivide` answers.
Only the opened box is in `LLiveryPageOpened`.
A script page reads no script before the fetch, then keeps the one style group afterwards.
With morphology off, a stored Spanish form sits in the expanded table and an unstored cell has the absent tip.
So `enabled` follows the morphology setting.
The English verb break carries two break rows in the pack's slot order.
Each has one shown cell and no tip.
A card with a stored target keeps the target, and the linked entry keeps the incoming usage.
A folded language writes its rows inside `<details>` with the `Reflex.More` summary, after the shown one.
A linked Ancestor box prints as an HTML anchor in the lineage block under the etymology heading.
The linked entry's own map nests above it in a stock.
The borrowed boxes carry the `borrowed` class.
An etymon text such as `*bʰreg-` takes no Markdown backslash, since the block is HTML.
Each box keeps its tongue and gloss spans, and the headword closes the tree in `div.root`.

## Stamp

Commit: `aae5349dbb94`
- `TEngineLivery.cs` `171299b8324074fb`
- `TEngineLiveryCard.cs` `e96a6501a7a378a6`
- `TEngineLiveryInflection.cs` `f92f64ecc5403946`
- `TEngineLiveryLanguage.cs` `93be9c93139a750d`
- `TEngineLiveryPage.cs` `4cdafc88e834f0f8`
