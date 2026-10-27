# Timbre

This folder holds the sound facts of the entry an editor holds.
They cover the sound sheet, anchors, transcription and glyph blocks, flag, paradigm view, contour, variety and font.

## `TContourLevel`

Covers the contour map `CContour.CContourRead` over a scale each fact chooses.
The cases reach the internal map through the interface relay, with no desk at all.
Every level the given scale lacks is dropped, while a repeated level on the scale stays.
A syllable left with no level comes back untoned.
An empty scale keeps every syllable's text but drops every level and every tone.
No syllables give no contour, and nothing throws.

## `TFont`

Covers the one font rule over fake settings ports.
Fonts map across, and an unsized font carries no size.
A blank language passes on to the port, whose empty font maps to nothing set.
A refused font read is not caught by the font rule, so the case expects the port's exception.
Every engine font role maps to its same-named mirror through `TInterfaceFont.TFontRoleRead`.
The font rule is driven with its settings port handed in, since every area reads it in its own language.
A pack's slant in any case arrives as its slant, and an unknown slant keeps the theme's.

## `TParadigmView`

Covers the Conduct records of the inflection box, with the engine's view faked at the paradigm port.
A real workspace holds the entry, so the reads meet the real wing and desk.
The lectern carries the engine's view into its records.
Core's pieces, group and label keys, and header keys arrive unchanged.
Where Core cuts the pieces is `TParadigmPiece`'s fact.
The faked cells carry pieces already cut, as the paradigm clerk answers them.
The joined piece text must read the form, since the record carries no text of its own.
The editor asks for the view with `held` on, so a lost cell reads as held, never as lost.
The fake engine answers the ready held cell only when asked with `held` on, so the fact pins that flag.
The held cell's pieces join to the ellipsis, and it keeps the `Paradigm.Held` tooltip key the engine answered.
Its one piece is the plain ellipsis, since a status cell has no marks or split.
A refused view read shows `Display.ParadigmReadFailed` and answers a null view.

## `TSounding`

Covers the editor's sound sheet gates and reads end to end, driven with no window.
A real desk holds the entry, and fake ports let a test shape or refuse each engine answer.
The variety and contour maps live in `TVariety` and `TContourLevel`.
A fresh draft has no stored entry, so every read answers empty.
A refused read answers empty, since a box that cannot fetch still has to draw.
A refused pack check offers no rebuild.
A refused reading answers empty and shows `Display.ReadingFailed` through the envoy.
An editor whose settings port answers no morphology verdict shows `Sound.MorphologyFailed`.
A rebuild that lands raises the change event once and shows no notice.
Each refused rebuild and rank edit asks the envoy to show its own notice key and announces no change.
An unanswered paradigm slot answers the pending tip while the fetch runs, whatever the morphology setting says.
After the fetch it answers the held tip when morphology is on, else the absent tip.
The pack checks and the glyph fonts are asked in the draft's language.
A desk with no stored entry offers no rebuild and waits for nothing, whatever the pack says.
Every font read reaches the settings port, even for a blank language.
A pressed initial or rime key reaches the navigation's yunjing opener with the draft's language.
A blank key of either kind never reaches the navigation.

## `TSoundingAnchor`

Covers the anchor menu gates of the editor end to end, over a real workspace and no window.
The menu reads the chosen row's anchors off the draft, so the test hands only the row's id.
`TSoundingAnchorPack` names one fanqie book, so the stored rows of its language are read.
An anchored reflex marks the held reading, and an id the draft does not hold answers the empty menu.
A draft never stored has no readings, so the menu is flagged empty.
A tick ties the chosen row to the reading in the draft, and a second tick unties it.
After the menu closes no row is chosen, so a late tick changes nothing.

## `TTimbre`

Covers the sound facts an editor reads for its held draft, over fake sound and settings ports or a workspace.
A phonemic respelling needs a respelling pack first, and a silent pack is not spoken.
An empty desk asks the pack for no language.
A top level reads the key `Theme.Contour.Top`, chosen in Conduct from the Core contour role.
The contour read draws only a tonal pack's toned reading, and its scale runs from five down to one.
Levels off the scale drop and the rest keep order and repeats, and a syllable left with none is untoned.
The accent sheet carries every row in the printed form the respelling switch picks.
An empty desk answers the mute sheet.
A typed accent answers the text the row now holds, and an empty desk takes nothing.
A desk filling its view takes nothing and answers the text its sheet holds for the row.
Adding a pronunciation places the blank row below its accent row, or first for the primary or a gone row.
Removing a pronunciation drops that accent row or the primary.
An empty desk still reaches the font port, so its fake answers the empty font, which maps to nothing set.
The flag read is covered by `TTimbreEnsign`, the reflex block by `TKindred`, and playback by `TPlayback`.

## `TTimbreAccentPrepare`

An English editor holding a draft with a primary reading and one accent row per text given.
`TPlayback` builds its accent rows here too.

## `TTimbreFlaggedPrepare`

An editor holding a draft in a language whose primary reading carries the British variety.
`TTimbreEnsign` builds its flagged drafts here too.

## `TTimbrePrepare`

Builds an editor through `TTimbreEditorPrepare` and hands back its sound facts.
`TTimbreEnsign` and `TTimbreGlyph` build their empty desks here too.

## `TTimbreEditorPrepare`

Builds an editor over stub ports, with its sound and settings ports answering one map.
The respelling checks sit on the settings port, so one map answers every sound fact.
`TKindred` builds its empty desk here too.

## `TTimbreEnsign`

Covers the editor's flag read for its held draft, over a real engine and language packs.
The flag read stores the pack's variety flags and answers the flagged accent with its primary variety.
A language changed meanwhile, a failed store or an empty desk answers nothing.
An unflagged pack answers nothing and stores no flag.

## `TTimbreGlyph`

Covers the editor's glyph block and its transcription text gate end to end on a real workspace.
A Korean draft splits its rows by the Hanja scheme, and the blank row stays among the transcription rows.
A language without a glyph section, or an empty desk, answers the hidden block.
The transcription text gate writes the typed text into the held glyph row, and a filling desk writes nothing.

## `TTranscriptionSheet`

Covers the editor's transcription block and its row gates end to end.
Every test runs on a real workspace except the empty desk, which stands on stub ports.
The read marks each row's dropdown by the one-scheme rule and answers whether a row can be added.
A language without schemes hides the block but keeps its rows, and an empty desk answers nothing.
A plus takes the first free scheme, after the pressed row or at the end.
A glyph row before the pressed row does not shift the place.
A plus stops once no scheme is free.
A scheme pick switches the row, and a pick another row holds is refused.
A minus drops the row and frees its scheme.
A filling desk writes nothing for a plus, a scheme pick or a minus.

## `TVariety`

Covers the variety map `CVariety.CVarietyRead`, which needs no desk and no engine.
A named variety keys its label under `Variety.` and its flag under the ensign format.
A blank variety has no flag key, and its label key is `Variety.` alone.

## Stamp

Commit: `9377e657b395`
- `TContourLevel.cs` `34c2d097b9b39233`
- `TFont.cs` `1904874473cdbca2`
- `TParadigmView.cs` `53c8052a3ab43fe5`
- `TSounding.cs` `21345a10b8ffaba7`
- `TSoundingAnchor.cs` `9b56f21a27c160bf`
- `TTimbre.cs` `4b43c5271b1ebf02`
- `TTimbreEnsign.cs` `59aeab6d16ea2bcd`
- `TTimbreGlyph.cs` `f86c158e8021a132`
- `TTranscriptionSheet.cs` `9f9332d696f8cb86`
- `TVariety.cs` `fc5ae869ebbe62d1`
