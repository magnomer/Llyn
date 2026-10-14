# Display

This folder is Conduct for the reading view of one entry, the lectern.
It stands apart from the editor folders because it only reads and paints, and edits nothing.
Each record is ready to paint, so a driver draws it and asks for no logic.
Engine types never cross into Deportment, so every `L` type here is internal.
One display per editor or wing builds its rules and its areas once, so every driver hears the same events.

## `CDisplay`

The header area owns the entry's life on the lectern, the favourite gate and the change events.
It builds the rules first, then the sound area over those rules and itself.
Then it builds the accent, playback, grasp, card and route areas and the compass over the same rules.
Each part is handed only the ports that part calls.
A refused read is shown through the envoy, so every driver over this display shows it once.
The area keeps the shown header, and the blank header stands while nothing is shown.
The lectern names the entry's lexical unit and freedom by the keys the static `LEntryPort` formats give.
The rime-book and script box openings stand on `CFold`, built over the shown entry.
`CFold` shares the rules' repaint memory, so its notices share the lectern's memory.
The rules stay internal, so no driver names an `L` type.
The editor's esteem, sound sheet and sound facts take the rules from here, so all share one shown draft.

The favourite, frequency, paradigm and reflex events count only for the chosen entry.
They are raised on the engine's thread, and a driver marshals each onto its own thread before it reads.
The script, fanqie and workspace bulletins count for any entry, since their sections redraw whole.
`CDisplayEntryChanged` counts the chosen entry's own bulletins and any example, situation, reference, author, tag, register or settings bulletin.
Each of those can change what the draft shows, so a driver hands it to `CDisplayEntryResonate`.
`CDisplayWorkspaceChanged` is handed to `CDisplayWorkspaceResonate`.
`CDisplayFoldChanged` counts only for the chosen entry, so a fold elsewhere leaves this view still.
It carries no draft, since a fold is view state and no edit.
`CDisplayOpened` follows an open once `CDisplayShown` holds its header.
`CDisplayClosed` follows a close that stopped playback, so every section empties.

`LDisplayNavigationAttach` hands the route and sound areas' records, rime cells and series to the atelier's navigation.
It hands the route area the mention area too.
The composition that owns the atelier calls it once, so an area built alone opens nothing.
`LDisplayVistaRestore` hands the new vista to the rules, then subscribes the plan of which subjects refresh which part.
Each vista is new, so the plan is subscribed once per vista.
`CDisplayPanelAttach` follows a panel, so each draft it loads opens here and its clearing closes the view.
Each browse-style panel attaches its editor's display when it is created.
The wing's display follows no panel, so it never attaches.

`LDisplayEntryOpen` opens a draft, or closes the view for a missing draft.
A vista choosing nothing closes the view too, since the draft then stands on no entry.
The note arrives parsed, so the driver draws its blocks and never parses.
The unit and the freedom arrive as localization keys.
`LDisplayStampRead` reads the stamp from the stored entry.
A refused read shows `Display.StampFailed` through repaint memory and hides the stamp.
An empty stamp lets the entry still open.
`CDisplayEntryClose` drops the shown draft and stops this view's play.
A driver calls it when it starts afresh, and a followed panel's clearing calls it too.
`CDisplayEntryResonate` reloads the chosen entry's draft on the driver's thread and opens it again.
Nothing chosen reloads nothing, and a refused load leaves the view as it stands.
An entry gone from the store closes the view.
`CDisplayWorkspaceResonate` closes the view once the workspace was swapped.

`CDisplayFavoriteToggle` returns a fresh favourite read after asking the rules to store the mark.
A refused mark thus answers the old value, and the display reports the failure.
`CDisplayFrequencyRead` answers the chosen entry's frequency chip, or null when none stands.
Conduct chooses the key of the one-off wording, and the driver's lookup turns it into text.

## `CDisplayAccent`

The pronunciation area is split from the sound area by concern, since the block reads through the language port alone.
It holds no state, and every read answers the mute block while nothing is shown.
So a late notice paints what a close painted.
Every read shows its failure through the repaint memory, since it runs on every repaint.
`CDisplayAccentScale` comes from the language port, so the scale keeps the one owner that parses the levels.
The reading view and the editor's contour share it.
`CDisplayAccentRead` has the engine answer the readings, the brackets and the pack's verdicts in one read.
Conduct only chooses each variety's label key through `CVariety.CVarietyRead`.
A refused read shows `Sound.LoadFailed` once and answers the mute block.
`CDisplayAccentLoad` loads the flags a driver draws once an entry opened.
The engine picks the varieties and loads nothing when the pack shows no flags.
The driver's own image store is handed the rows through the one flag map.
The load answers the block again once the flags are in, so the driver repaints its rows.
Another entry shown meanwhile wins, so a late load answers null and paints nothing.
A failed load shows `Sound.LoadFailed` once and answers null.

## `CDisplayPlayback`

The playback area is split from the sound area by concern, since playback reads through the media port alone.
It holds no state, and the shown draft and the latest play stay in `LDisplaySound`.
The read shows its failure through repaint memory, since it runs on every repaint.
The play gates answer a user act, so they show every failure.
`CDisplayPlaybackRead` has the engine answer the button and volume tray verdicts, so Conduct reads no audio field.
A refused read shows `Sound.LoadFailed` once and hides both.
The draft's play lets the engine pick the file, so Conduct reads no field of the draft.
The accent row's play takes the accent's id, and the recording is read from the shown draft's accent.
An id with no accent in the shown draft, or no shown draft, plays nothing and shows nothing.
`CDisplayPlaybackCancel` stops this view's own play, and a sound another view started plays on.

## `CDisplayGrasp`

The grasp area is split from the header by concern, since the grasp touches no other part of the header.
It holds no state, and every member works on the entry the rules say is chosen.
The grasp rule and the chosen entry both stay in `LDisplay`.
`CDisplayGraspStep` is never negative, since `LDisplay` raises a negative engine limit to zero.
The star control takes it as its limit, so it names no engine constant.
`CDisplayGraspFill`, `CDisplayGraspUnrated` and `CDisplayGraspHit` are ready tables.
The driver hands them to the control once, so drawing and pointing request no logic.
`LDisplayGraspAttach` subscribes the grasp subject of the current vista once per vista.
`CDisplayGraspRead` never answers a step below zero or above `CDisplayGraspStep`, since the stars draw only that range.
The read with a step answers the wording for the step under the pointer, or empty while nothing is chosen.
`CDisplayGraspSet` answers the stored stars.
`CDisplayGraspMove` shifts the standing step by a signed amount, clamped to zero and `CDisplayGraspStep`.
A move that lands on the standing step only reads, so a key at an end never clears the grasp.
A press on the standing step comes back cleared, since the display's one rule owns that.

## `CDisplayRoute`

The route area handles the clicks that open another place.
A chip, a source link, or a word in a line opens through it.
It holds no state beyond the atelier's mention area, which stays null until the navigation is attached.
`CDisplayRowChosen` carries the tab and the record a clicked chip names, and the display hands it to the navigation.
`CDisplayChipOpen` opens a stored chip's record in the tab its kind picks, chosen by name in `LDisplayTabRead`.
Any other chip falls to the link, which opens the library tab on its entry.
The engine says which entry a link names, and an empty id names none, so nothing opens.
It answers whether a tab was asked, so the driver marks the click handled.
`CDisplayMentionFind` takes the whole shown text and the clicked UTF-16 unit.
The engine reads the row's text, language and Mentions from the shown entry itself.
So no Mention travels up through the driver and back.
`LDisplayMentionOpen` answers null when no mention area is held or nothing is shown.
`LMentionOffsetRead` turns the click into a code-point offset.
`LMentionResultOpen` then opens a stored Mention or a sole entry at once.
The answer is where the menu stands and the entries it offers, empty when one opened.
The menu is placed on the found word's start, as a UTF-16 unit in the whole shown text.
The reading view asks no leave question, since the navigation asks it on opening.
A refused lookup shows `Mention.FindFailed` and answers null, so the click opens nothing.
The offset read and the open sit inside the same catch.

## `CLectern`

The header of the reading view for the shown entry, ready to show.
The display builds it once when an entry opens, so the driver only draws it.
`CLecternMarked` is true when the entry carries a part of speech or a lexical unit, which shows the speech section.
`CLecternNote` holds the note's Markdown blocks, parsed once when the entry opens.
`CLecternStamped` is whether the stored entry was read, which shows the stamp row.
The creation and update times are in local short form.
`CLecternUnit` and `CLecternFreedom` are localization keys, empty when none is chosen.
`CLecternLanguage` picks the headword font and the flag.

## `CLecternAccent`

The pronunciation block for the shown entry, ready to show.
The contour is empty when it hides.
The text is the primary reading as the respelling switch shows it.
`CLecternAccentSpoken` shows its surface when it holds text.
The rows are the further pronunciations in the entry's order.
`CLecternAccentFlagged` is whether the pack draws varieties as flags rather than names.

## `CLecternPlayback`

The playback verdicts for the shown entry.
`CLecternPlaybackRecorded` is whether the draft's own recording exists, which shows the play button.
`CLecternPlaybackAudible` is whether anything plays at all, which shows the volume tray.

## `CGrasp`

The grasp stars of the chosen entry, ready to show.
The step is zero when nothing is chosen or the read fails, and never above the limit.
The label is the engine's wording of that step, empty when nothing is chosen.

## `LDisplay`

The rules the display's areas share, holding their state, decisions and engine reads.
It is internal, so no driver names it and no `L` type crosses into Deportment.
Most members forward one read or one mark request to the ports, so the view never holds an engine.
A refused read or mark reaches the user through the envoy the panel handed over.
It builds the sound half once, so the area and the sound area over it share one shown draft.
The sound half's read failures go through the atelier's repaint memory, and its failed sends show every time.
No vista is held yet, so the display stands on no entry until `LDisplayVistaRestore`.
`LDisplayChosen` is null before a vista is restored.
`LDisplayMediaPort` is read by `CDisplayCard`, which resolves every picture and video address of the lectern's cards through it.
`LDisplayChosenAttach` hands an observer the vista's bulletins about a subject that name the chosen entry or no entry.
The subject maps through `CCatalog.LCatalogSubjectRead`, by name, so an unknown subject throws.
`LDisplayObserverAttach` hands every bulletin about a subject the vista hears.
`LDisplayGraspStep` raises a negative engine limit to zero, since a star row cannot count below none.
`LDisplayGraspStar` is read once, since the limit never changes while the app runs.
It passes the engine's answers on unread and computes no fill, count or hit itself.
`LDisplayGraspFormat` words a step, or answers empty when no entry is given.
`LDisplayDraftLoad` reloads the chosen entry's draft from the vista and hands it on, or null once the entry is gone.
A refused load calls nothing and shows `Sound.LoadFailed` once through the repaint memory.
The page keeps what it shows, so no draft is paired with an id it was not loaded for.
`LDisplayEntryLoad` loads one entry for a pick, chooses it on the vista, and answers its draft.
The wing opens that draft in the area, since this class no longer knows the area.
A gone entry chooses nothing and answers null, so the side stands on no entry and the area closes.
A refused load throws before anything changes, so the caller can report it.
`LDisplayFavoriteRead` answers false when no entry is given.
A failed read shows `Favorite.ReadFailed` once until the user acts, and answers false.
The area passes the chosen entry and the editor's esteem the stored one, so both share this rule.
`LDisplayFavoriteSave` shows a refused mark as `Favorite.MarkFailed` through the envoy every time.
The heart then redraws from the stored value.
`LDisplayGraspRead` holds the engine's step between zero and `LDisplayGraspStep`.
A failed read shows `Grasp.ReadFailed` once until the user acts, and answers zero.
The step it answers is held with its entry in `_lDisplayGrasp`, since that is the step the stars now show.
`LDisplayGraspSave` clears the grasp when the step already stands, so one press unrates.
Only the step held for the same entry counts, so another entry's step never clears this one.
The area's gate and the editor's esteem both store through it, so the rule has one owner.
A refused save shows `Grasp.MarkFailed` through the envoy every time, and the stars redraw from the stored value.
`LDisplayFrequencyRead` reshapes the engine's chip, since the draft does not carry the frequency.
It raises a negative band or spare count to zero, since a count below zero has no star row.
A failed read shows `Frequency.ReadFailed` once until the user acts, and answers no chip.
It answers null when no entry is given, the entry has no frequency, or the read fails.
`LDisplayTierRead` matches the engine rank name to a tier by name.
Any other name is the unknown tier, so the chip always finds a brush and a name.

## Stamp

Commit: `c6e60141a172`
- `CDisplay.cs` `39bf40eb3a593f09`
- `CDisplayAccent.cs` `9fddfe5a415f4f7e`
- `CDisplayGrasp.cs` `0acd77ec4e934a75`
- `CDisplayPlayback.cs` `bde1c68e992d1856`
- `CDisplayRoute.cs` `53fe8dbcb726393d`
- `CGrasp.cs` `726f35d01c119673`
- `CLectern.cs` `fed67f5c9720b3bf`
- `CLecternAccent.cs` `92ed38553d3d9914`
- `CLecternPlayback.cs` `182378266a721075`
- `LDisplay.cs` `b24c48447e0db118`
