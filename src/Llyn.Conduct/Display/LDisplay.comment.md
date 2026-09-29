# LDisplay.cs

## `public sealed class LDisplay`

The reading view's conduct, which holds its state, decisions and engine reads.
Only the display drivers, such as [QLectern](../../Llyn.UIDeportment/Display/QLectern.comment.md), name it, so no veneer reaches Conduct.
The lectern's gates and change events live in its area, [CDisplay](CDisplay.comment.md), so this class holds the rules they share.
It forwards the pending checks the editor reads to the sound half.
Every other member forwards one read or one mark request to the ports, so the view never holds an engine.
A refused read or mark reaches the user through the envoy the panel handed over.

## `public LDisplaySound LDisplaySound { get; }`

The sound half, which holds the shown draft, the fold state and every phonology read.

## `internal void LDisplayNavigationAttach(CNavigation navigation)`

Hands every record, rime cell and series the display's gates raise to the atelier's navigation.
The composition that owns the atelier calls it once, so a display built alone opens nothing.

## `public CDisplay CDisplayArea { get; }`

The lectern's area, built once here, so every driver over this display hears the same events.

## `public CDisplaySound CDisplaySound { get; }`

The lectern's sound area, built once here over the sound half and the header area.

## `private readonly CEnvoy _lDisplayEnvoy;`

The envoy a refused load, mark or name lookup is shown through, with its text key.
The sound half's failures go to it too, so every driver over this display shows each one once.

## `private readonly LSettingsPort _lDisplaySettings;`

The port a failure's ready notice is read through before the envoy shows it.

## `public void LDisplayVistaRestore(LVista vista)`

Takes the vista the display follows, then lets the area subscribe its plan on it.
Each vista is new, so the plan is subscribed once per vista.

## `internal void LDisplayChosenAttach(CSubject subject, Action<CBulletin> observer)`

Hands `observer` the display vista's bulletins about `subject` that name the chosen entry or no entry.
The area and the wing name the subject and hear the bulletin in Conduct's records, never the engine's.
The subject maps through `CPanel.CPanelSubjectRead`, by name, so an unknown subject throws.

## `internal void LDisplayObserverAttach(CSubject subject, Action<CBulletin> observer)`

Hands `observer` every bulletin about `subject` the display vista hears, in Conduct's records.

## `internal int LDisplayGraspStep => _lEntryPort.LEngineGraspStep;`

The last grasp step, which the display's star control takes as its limit.

## `public IReadOnlyList<CCompassRow> CDisplayCompassRead(`

The read behind the floating contents, one row per shown part and one per card under it.
The driver names the parts that stand on the page, in page order, and its own text lookup.
Conduct chooses every wording key and the driver's lookup turns it into text.
A card is named by its own title, else by its kind, and an uncertain title by the unknown mark.
Its number is carried apart from its name, since a number is not part of a sentence.
A card's title goes through the one state map, and its number is copied as the engine holds it.
The full card map is not run, since the contents need no card's lists or links.
The engine then numbers the names two rows share, so the contents never show two rows alike.

## `private static string LDisplayKeyRead(CCompassPart part)`

The wording key of each part, mapped by name so an unknown part throws.

## `private IReadOnlyList<string> LDisplayNameResolve(IReadOnlyList<string> labels)`

The compass wordings made distinct by the engine's twin rule.
A refused lookup is shown through the envoy and answers the plain wordings.

## `internal string LDisplayGraspFormat(long? entry, int step)`

The wording of a grasp step, or empty when no entry is given.

## `internal void LDisplayDraftLoad(Action<LEntryDraft?> show)`

Reloads the chosen entry's draft from the vista and hands it to `show`, or null once the entry is gone.
A refused load calls nothing and shows `Sound.LoadFailed` through the envoy.
The page keeps what it shows, so no draft is paired with an id it was not loaded for.

## `internal void LDisplayEntryLoad(long id)`

Loads one entry for a pick, chooses it on the vista, and opens its draft in the area.
An entry that is gone chooses nothing, so the side stands on no entry and the area closes.
A refused load throws before anything changes, so the caller can report it.

## `internal bool LDisplayFavoriteRead(long? entry)`

Whether the entry is a favorite, false when no entry is given or the read fails.
The area passes the chosen entry and the editor's esteem the stored one, so both share this rule.

## `internal void LDisplayFavoriteSave(long? entry, bool marked)`

Marks or unmarks the entry as a favorite.
A refused mark shows `Favorite.MarkFailed` through the envoy, and the heart then redraws from the stored value.

## `internal int LDisplayGraspRead(long? entry)`

The entry's grasp step, zero when no entry is given or the read fails.
The step it answers is held with its entry, since that is the step the stars now show.

## `private (long, int)? _lDisplayGrasp;`

The entry and grasp step last read or stored, which a repeated press compares against.

## `internal void LDisplayGraspSave(long? entry, int step)`

Stores `step` as the entry's grasp.
Picking the step that already stands clears the grasp, so one press unrates.
Only the step held for the same entry counts, so another entry's step never clears this one.
The area's gate and the editor's esteem both store through it, so the rule has one owner.
A refused save shows `Grasp.MarkFailed` through the envoy, and the stars then redraw from the stored value.

## `internal CFrequency? LDisplayFrequencyRead(long? entry, string once)`

The entry's frequency chip, since the draft does not carry the frequency.
The engine gathers the rows, words them, and names the rank and the spare stars.
This read only reshapes the answer.
It answers null when no entry is given, the entry has no frequency, or the read fails.
An entry with no value asks the engine to fill it, and the fill announces itself when done.

