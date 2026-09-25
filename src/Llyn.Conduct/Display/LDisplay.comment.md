# LDisplay.cs

## `public sealed class LDisplay`

The reading view's conduct, which holds its state, decisions and engine reads.
Only [LLectern](../../Llyn.UIDeportment/Display/LLectern.comment.md) names it, so no veneer reaches Conduct.
It forwards the pending checks the editor reads to the sound half.
Every other member forwards one read or one mark request to the ports, so the view never holds an engine.
The card reads answer for the shown draft, and a refused one answers empty so the page still draws.

## `public LDisplaySound LDisplaySound { get; }`

The sound half, which holds the shown draft, the fold state and every phonology read.

## `public event Action<string, Exception>? LDisplayFailed;`

Raised with the text key and the exception when a word lookup or a mark is refused.
The card deportment hands it to the window's notice.

## `public void LDisplayShow(LEntryDraft draft)`

Hands the shown draft to the sound half.

## `public void LDisplayClear()`

Drops the shown draft from the sound half.

## `public int LDisplayGraspStep => _lEntryPort.LEngineGraspStep;`

The last grasp step, which the display's star control takes as its limit.

## `public IReadOnlyList<string> LDisplayNameResolve(IReadOnlyList<string> labels)`

The compass labels made distinct, numbered in order where two sections share a name.

## `public static string LDisplayTitleRead(LCardDraft card, string kind, string unknown)`

A card is named by its own title.
An untitled card is named by what kind of card it is, so the row is never blank.
A title still uncertain reads as `unknown`, so a guess never passes for a name.
It is static, because the label depends on the card alone.

## `public static bool LDisplayCardCheck(LCardDraft card, long id)`

Whether `card` is the one with `id`, which a card scroll looks for.

## `public string LDisplayGraspFormat(int step)`

The wording of a grasp step, or empty while no entry is shown.

## `public LEntryDraft? LDisplayDraftLoad()`

The chosen entry's draft reloaded from the vista, or null once the entry is gone.
A refused load answers the shown draft, so the page keeps what it shows.

## `private LEntryDraft? _lDisplayLoaded;`

The draft the last entry load found, or null when that entry was gone.

## `public void LDisplayEntryLoad(long id)`

Loads one entry for a pick, holds its draft, and chooses it on the vista.
An entry that is gone chooses nothing, so the side stands on no entry.
A refused load throws before anything changes, so the caller can report it.

## `public string LDisplayLanguageRead()`

The shown draft's language, or empty while nothing is shown.
A flag arriving late is drawn for it, so a stale language never gets its flag.

## `public bool LDisplayFavoriteRead()`

Whether the chosen entry is a favorite, false while nothing is chosen or the read fails.

## `public void LDisplayFavoriteSave(bool marked)`

Marks or unmarks the chosen entry as a favorite.
A refused mark raises `LDisplayFailed` with `Favorite.MarkFailed`, and the heart then redraws from the stored value.

## `public int LDisplayGraspRead()`

The chosen entry's grasp step, zero while nothing is chosen or the read fails.

## `public void LDisplayGraspSave(int step)`

Stores `step` as the chosen entry's grasp.
A refused save raises `LDisplayFailed` with `Grasp.MarkFailed`, and the stars then redraw from the stored value.

## `public LDisplayStamp LDisplayStampRead()`

The creation and update times from the stored entry, since the draft carries no clock.
An entry that cannot be read answers a hidden stamp.

## `public static string LDisplayStampFormat(string? utc)`

Turns a stored ISO 8601 UTC stamp into local time in the short general format of the current culture.
A missing or unreadable stamp answers empty.

## `public IReadOnlyList<string> LDisplaySpeechRead()`

The shown draft's parts of speech as the names a reader reads.
The draft keeps the stored value beside the name, and the page shows only the name.

## `public IReadOnlyList<LFrequency> LDisplayFrequencyRead()`

The chosen entry's frequency rows, since the draft does not carry them.
It answers empty while nothing is chosen or the read fails.
An entry with no value asks the engine to fill it, and the fill announces itself when done.

## `public static bool LDisplayFrequencyCheck(IReadOnlyList<LFrequency> rows)`

Whether an entry has any frequency to show, true when at least one source answered.

## `public static int LDisplayBandResolve(IReadOnlyList<LFrequency> rows)`

The star count of the first band any row carries in pack order, four for core, one for rare.
Zero when no row carries a band or the band is not a ladder name.

## `public static string LDisplayBandRead(int count, string prefix)`

The ladder name at a star count behind `prefix`, or the unknown name at zero.
The chip passes the localization prefix for its name and the theme prefix for its brush.

## `public static string LDisplaySourceFormat(IReadOnlyList<LFrequency> rows, string once)`

One line per row, each naming its source, so the band is never the only thing said.
A row with a word interval prints it through the `once` pattern as a round number.
The line then reads once every 1,300 words.
A row without one prints its unit before the raw figure when the pack names one, as CantoDict's Level does.
A row with neither, such as a Longman list mark or an HSK level, prints its raw answer.

## `public bool LDisplayFanqieCheck(long? id)`

Forwards to the sound half, since the editor asks whether the rime-book rows are still being fetched.

## `public bool LDisplayScriptCheck(long? id)`

Forwards to the sound half, since the editor asks whether the script images are still being fetched.

## `public bool LDisplayParadigmCheck(long? id)`

Forwards to the sound half, since the editor asks whether the inflections are still being fetched.

## `public IReadOnlyList<LUsage> LDisplayIncomingRead()`

The usages pointing at the shown entry, each carrying the epithet of the entry that holds it.
No entry shown, or a refused read, answers no usages.

## `public static string LDisplayOwnerRead(LUsage usage)`

The text key naming what holds a usage: a Meaning or a Collocation.

## `public IReadOnlyList<LTranslationTarget> LDisplayTargetRead()`

The headwords the shown draft's cards link to, or none when nothing is shown or the read is refused.

## `public IReadOnlyList<LTranslationTarget> LDisplayEtymonRead()`

The source links of the shown draft's etymology.

## `public LSentenceOrder LDisplayOrderRead()`

The sentence order of the shown draft's language, or the default when nothing is shown or the read is refused.

## `public IReadOnlyDictionary<long, string> LDisplayCitationRead()`

The byline of every Source, or none when the read is refused.
It is read on every show, so a Source edited elsewhere reads fresh.

## `public LMentionResult LDisplayMentionFind(`

What stands at `offset` of a clicked sentence, asked of the engine with its Mentions and language.
A sentence carrying no language is read in the shown entry's language.
A refused lookup raises `LDisplayFailed` and answers nothing found, so the click opens nothing.

## `public IReadOnlyList<LTranslationTarget> LDisplayEtymonRead(LEntryDraft draft)`

The source links of an etymology, named by the engine.
A read the engine refuses answers no links, so the page still draws.

## `public static bool LDisplayNarrativeCheck(bool editable, string text)`

Whether the read face of the narrative stands: only on the read side, and only with words in it.

## `public static bool LDisplayEtymonCheck(bool editable, int count)`

Whether the row of source links shows: always while editable, else only with a link.

## `public static bool LDisplayEtymologyCheck(string text, int count)`

Whether a read etymology is worth a place at all: a narrative or a link.
