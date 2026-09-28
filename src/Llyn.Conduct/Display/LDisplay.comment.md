# LDisplay.cs

## `public sealed class LDisplay`

The reading view's conduct, which holds its state, decisions and engine reads.
Only the display drivers, such as [QLectern](../../Llyn.UIDeportment/Display/QLectern.comment.md), name it, so no veneer reaches Conduct.
The lectern's gates and change events live in its area, [CDisplay](CDisplay.comment.md), so this class holds the rules they share.
It forwards the pending checks the editor reads to the sound half.
Every other member forwards one read or one mark request to the ports, so the view never holds an engine.
The card reads answer for the shown draft, and a refused one answers empty so the page still draws.

## `public LDisplaySound LDisplaySound { get; }`

The sound half, which holds the shown draft, the fold state and every phonology read.

## `public CDisplay CDisplayArea { get; }`

The lectern's area, built once here, so every driver over this display hears the same events.

## `public event Action<string, Exception>? LDisplayFailed;`

Raised with the text key and the exception when a word lookup, a mark or a load is refused.
The sound half's failures arrive here too.
The card deportment hands it to the window's notice.

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
The cards are read through the one card map, so no engine card is read here.
The engine then numbers the names two rows share, so the contents never show two rows alike.

## `private static string LDisplayKeyRead(CCompassPart part)`

The wording key of each part, mapped by name so an unknown part throws.

## `private IReadOnlyList<string> LDisplayNameResolve(IReadOnlyList<string> labels)`

The compass wordings made distinct by the engine's twin rule.
A refused lookup raises `LDisplayFailed` and answers the plain wordings.

## `public static bool LDisplayCardCheck(LCardDraft card, long id)`

Whether `card` is the one with `id`, which a card scroll looks for.

## `internal string LDisplayGraspFormat(long? entry, int step)`

The wording of a grasp step, or empty when no entry is given.

## `internal void LDisplayDraftLoad(Action<LEntryDraft?> show)`

Reloads the chosen entry's draft from the vista and hands it to `show`, or null once the entry is gone.
A refused load calls nothing and raises `LDisplayFailed` with `Sound.LoadFailed`.
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
A refused mark raises `LDisplayFailed` with `Favorite.MarkFailed`, and the heart then redraws from the stored value.

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
A refused save raises `LDisplayFailed` with `Grasp.MarkFailed`, and the stars then redraw from the stored value.

## `internal CFrequency? LDisplayFrequencyRead(long? entry, string once)`

The entry's frequency chip, since the draft does not carry the frequency.
The engine gathers the rows, words them, and names the rank and the spare stars.
This read only reshapes the answer.
It answers null when no entry is given, the entry has no frequency, or the read fails.
An entry with no value asks the engine to fill it, and the fill announces itself when done.

## `public bool LDisplayFanqieCheck(long? id)`

Forwards to the sound half, since the editor asks whether the rime-book rows are still being fetched.

## `public bool LDisplayScriptCheck(long? id)`

Forwards to the sound half, since the editor asks whether the script images are still being fetched.

## `public bool LDisplayParadigmCheck(long? id)`

Forwards to the sound half, since the editor asks whether the inflections are still being fetched.

## `public IReadOnlyList<CUsage> LDisplayIncomingRead()`

The usages pointing at the chosen entry, each carrying the epithet of the entry that holds it.
It reads the live choice, as the area's favorite, grasp, stamp and frequency reads do.
No entry chosen answers no usages.
A refused incoming read raises `LDisplayFailed` with `Display.IncomingFailed` and answers no usages.
A refused epithet keeps its usage with an empty epithet, so one bad entry hides nothing else.
Any refused epithet raises `Display.EpithetFailed` once, after every usage is read.
Each usage maps through `COeuvre.COeuvreUsageRead`, the one map the vita's citations share.

## `public IReadOnlyList<LTranslationTarget> LDisplayTargetRead()`

The headwords the shown draft's cards link to, or none when nothing is shown or the read is refused.

## `public IReadOnlyList<LTranslationTarget> LDisplayEtymonRead()`

The source links of the shown draft's etymology.

## `public CSentenceOrder LDisplayOrderRead()`

The sentence order of the shown draft's language, shaped for the display's frame converter.
It is the default order when nothing is shown or the read is refused.

## `public IReadOnlyDictionary<long, string> LDisplayCitationRead()`

The byline of every Source, or none when the read is refused.
It is read on every show, so a Source edited elsewhere reads fresh.

## `public void LDisplayMentionFind<LDisplayAnchor>(`

What stands at `offset` of a clicked sentence, asked of the engine with its Mentions and language.
A sentence carrying no language is read in the shown entry's language.
The answer goes to `show` with `anchor`, which runs only when the lookup succeeds.
A refused lookup raises `LDisplayFailed` and never calls `show`, so the click opens nothing.

## `public IReadOnlyList<LTranslationTarget> LDisplayEtymonRead(LEntryDraft draft)`

The source links of an etymology, named by the engine.
A read the engine refuses answers no links, so the page still draws.

## `public static bool LDisplayEtymologyCheck(string text, int count)`

Whether a read etymology is worth a place at all: a narrative or a link.
