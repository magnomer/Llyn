# LDisplay.cs
Hash: `044f063134de4a8d`

## `internal sealed class LDisplay`

The reading view's conduct, which holds its state, decisions and engine reads.
It is internal, so no driver names it and no `L` type crosses into Deportment.
The lectern's gates and change events live in its area, [CDisplay](CDisplay.comment.md), and the areas it hands out.
That area builds this class and holds it, so this class holds only the rules they share.
Most members forward one read or one mark request to the ports, so the view never holds an engine.
A refused read or mark reaches the user through the envoy the panel handed over.

## `internal LDisplay(LDraftPort drafts, LEntryPort entries, LFavoritePort favorites, LVistaPort vistas, LGraspPort grasps, LPronunciationPort pronunciations, LLanguagePort languages, LReflexPort reflexes, LFanqiePort fanqies, LScriptPort scripts, LParadigmPort paradigms, LSettingsPort settings, LMediaPort media, CEnvoy envoy, CLedgerNoticed noticed)`

Builds the sound half once, so the area and the sound area over it share one shown draft.
The five sound ports pass straight to the sound half, which alone calls them.
It takes the atelier's repaint memory, so a read failing on every repaint shows its notice once.
The sound half's read failures go through that memory, and its failed sends show every time.
No vista is held yet, so the display stands on no entry until `LDisplayVistaRestore`.

## `internal LDisplaySound LDisplaySound { get; }`

The sound half, which holds the shown draft, its entry, the fold state and the latest play's ticket.
Only Conduct reads it, so no driver is offered the Core draft it holds.

## `internal CLedgerNoticed LDisplayNoticed { get; }`

The atelier's repaint memory, which every repaint read of this display shows its failure through.
The area, the sound area and the editor's sound sheet and sound facts reach it here.
So they share one memory.

## `internal LMediaPort LDisplayMediaPort { get; }`

The port a card's picture addresses are resolved through.
The lectern's cards and the editor's draft read it, so both carry the same ready address.

## `private readonly LEntryPort _lDisplayEntryPort;`

The entry port the shown entry is loaded through.

## `private readonly LFavoritePort _lDisplayFavoritePort;`

The favourite port the favourite mark is read and toggled through.

## `private readonly LVistaPort _lDisplayVistaPort;`

The port the shown entry is loaded through from the display's vista.

## `private readonly LGraspPort _lDisplayGraspPort;`

The grasp port the grasp is read, worded and saved through.

## `private readonly LPronunciationPort _lDisplayPronunciationPort;`

The pronunciation port the frequency gauge is resolved through.

## `private LVista? _lDisplayVista;`

The vista the display follows, null until `LDisplayVistaRestore` takes one.

## `private readonly CEnvoy _lDisplayEnvoy;`

The envoy a refused load or mark is shown through, with its text key.
The sound half's failures go to it too.

## `private readonly LSettingsPort _lDisplaySettings;`

The port a failure's ready notice is read through before the envoy shows it.

## `internal long? LDisplayChosen`

The entry the display vista has chosen, or null before a vista is restored.

## `internal void LDisplayVistaRestore(LVista vista)`

Takes the vista the display follows.
The area subscribes its plan on it right after, so the plan is subscribed once per vista.
It stays internal, since the vista is an engine type no driver may name.

## `internal void LDisplayChosenAttach(CSubject subject, Action<CBulletin> observer)`

Hands `observer` the display vista's bulletins about `subject` that name the chosen entry or no entry.
The area names the subject and hears the bulletin in Conduct's records, never the engine's.
The subject maps through `CCatalog.LCatalogSubjectRead`, by name, so an unknown subject throws.

## `internal void LDisplayObserverAttach(CSubject subject, Action<CBulletin> observer)`

Hands `observer` every bulletin about `subject` the display vista hears, in Conduct's records.

## `internal int LDisplayGraspStep`

The last grasp step, which the display's star control takes as its limit.
A negative engine limit is raised to zero, since a star row cannot count below none.

## `internal string LDisplayGraspFormat(long? entry, int step)`

The wording of a grasp step, or empty when no entry is given.

## `internal void LDisplayDraftLoad(Action<LEntryDraft?> show)`

Reloads the chosen entry's draft from the vista and hands it to `show`, or null once the entry is gone.
A refused load calls nothing and shows `Sound.LoadFailed` once through the repaint memory.
The page keeps what it shows, so no draft is paired with an id it was not loaded for.

## `internal LEntryDraft? LDisplayEntryLoad(long id)`

Loads one entry for a pick, chooses it on the vista, and answers its draft.
The wing opens that draft in the area, since this class no longer knows the area.
A gone entry chooses nothing and answers null, so the side stands on no entry and the area closes.
A refused load throws before anything changes, so the caller can report it.

## `internal bool LDisplayFavoriteRead(long? entry)`

Whether the entry is a favorite, false when no entry is given.
A failed read shows `Favorite.ReadFailed` once until the user acts, and answers false.
The area passes the chosen entry and the editor's esteem the stored one, so both share this rule.

## `internal void LDisplayFavoriteSave(long? entry, bool marked)`

Marks or unmarks the entry as a favorite.
A refused mark shows `Favorite.MarkFailed` through the envoy every time, and the heart then redraws from the stored value.

## `internal int LDisplayGraspRead(long? entry)`

The entry's grasp step, zero when no entry is given.
The engine's step is held between zero and `LDisplayGraspStep`, since the stars draw only that range.
A failed read shows `Grasp.ReadFailed` once until the user acts, and answers zero.
The step it answers is held with its entry, since that is the step the stars now show.

## `private (long, int)? _lDisplayGrasp;`

The entry and grasp step last read or stored, which a repeated press compares against.

## `internal void LDisplayGraspSave(long? entry, int step)`

Stores `step` as the entry's grasp.
Picking the step that already stands clears the grasp, so one press unrates.
Only the step held for the same entry counts, so another entry's step never clears this one.
The area's gate and the editor's esteem both store through it, so the rule has one owner.
A refused save shows `Grasp.MarkFailed` through the envoy every time, and the stars then redraw from the stored value.

## `internal CFrequency? LDisplayFrequencyRead(long? entry, string once)`

The entry's frequency chip, since the draft does not carry the frequency.
The engine gathers the rows, words them, and names the rank and the spare stars.
This read reshapes the answer and raises a negative band or spare count to zero.
A count below zero has no star row, so the chip never receives one.
The rank name becomes a tier through `LDisplayTierRead`.
A failed read shows `Frequency.ReadFailed` once until the user acts, and answers no chip.
It answers null when no entry is given, the entry has no frequency, or the read fails.

## `private static CFrequencyTier LDisplayTierRead(string rank)`

The tier an engine rank name stands for, matched by name.
Any other name is the unknown tier, so the chip always finds a brush and a name.

