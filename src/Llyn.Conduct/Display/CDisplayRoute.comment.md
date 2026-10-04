# CDisplayRoute.cs
Hash: `b781ff7bdc86f8e2`

## `public sealed class CDisplayRoute`

The reading view's clicks that open another place.
A chip, a source link, or a word in a line or the etymology opens through it.
Only [CDisplay](CDisplay.comment.md) builds one, over its own rules, and hands it out.
It holds no state beyond the atelier's mention area.

## `private readonly LDisplaySound _cDisplayVoice;`

The rules' sound half, which holds the shown draft a word is found in.

## `private readonly LEntryPort _cDisplayPort;`

The entry port both word finds go through.

## `private readonly LSettingsPort _cDisplaySettings;`

The settings port the ready notice is read from.

## `private readonly CEnvoy _cDisplayEnvoy;`

The envoy a refused find is shown through.

## `private CMention? _cDisplayMention;`

The atelier's mention area, null until the navigation is attached.

## `internal CDisplayRoute(LDisplay display, LEntryPort entries, LSettingsPort settings, CEnvoy envoy)`

Only the display builds it, over its rules and the ports the atelier handed down.
It keeps the rules' sound half, since that half holds the shown draft.

## `internal event Action<string, long>? CDisplayRowChosen;`

Raised with the tab and the record a clicked chip names.
The display hands it to the atelier's navigation, which opens the record there.

## `internal void LDisplayMentionAttach(CMention mention)`

Holds the atelier's mention area, which opens what a clicked word found.
The composition calls it through the display's `LDisplayNavigationAttach`, so an area built alone finds nothing.

## `public bool CDisplayChipOpen(CLeafChip? chip, long? link)`

The gate for a chip clicked on a card or a source link clicked in the etymology.
`chip` is the clicked situation, register or tag chip, and `link` the entry id a link chip names.
A stored chip opens its record in the tab its kind picks.
Any other chip falls to the link, which opens the library tab on its entry.
The engine says which entry a link names, and an empty id names none, so nothing opens.
It answers whether a tab was asked, so the driver marks the click handled.

## `private static string LDisplayTabRead(CSubject subject)`

The tab that opens a chip's record, picked by the chip's kind, by name.

## `public CMentionOffer? CDisplayMentionFind(long sentence, string text, int unit)`

The gate for a word clicked in an example line of the reading view.
The driver names the line's sentence row and the raw click values.
They are the whole shown text and the clicked UTF-16 unit in it.
The open turns the click into a code-point offset before the find.
The engine reads the row's text, language and Mentions from the shown entry itself.
So no Mention travels up through the driver and back.

## `public CMentionOffer? CDisplayEtymologyFind(string text, int unit)`

The gate for a word clicked in the etymology prose of the reading view.
The driver hands the raw click: the whole shown text and the clicked UTF-16 unit in it.
The engine reads the prose and its language from the shown entry itself.

## `private CMentionOffer? LDisplayMentionOpen(string text, int unit, Func<LEntryDraft, int, LMentionResult> find)`

The open shared by both find gates, given the engine find over the shown entry and an offset.
It answers null when no mention area is held or nothing is shown.
The mention area's `LMentionOffsetRead` turns the click into the offset the find reads.
`LMentionResultOpen` then opens a stored Mention or a sole entry at once.
The answer is where the menu stands and the entries it offers, empty when one opened.
The open places it on the found word's start, as a UTF-16 unit in the whole shown text.
The reading view asks no leave question of its own, since the navigation asks it on opening.
A refused lookup shows `Mention.FindFailed` and answers null, so the click opens nothing.
The offset read and the open sit inside the same catch.
