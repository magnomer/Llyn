# CAnthology.cs

## `public sealed class CAnthology`

The corpus panel's example list: the example vista it holds and the panel over it.
It finds the rows and their usage, and takes the query, order and kind filter.
Its panel loads, edits and deletes the chosen Example, worded under the Example scope.
It also answers the transcript's citation field and prints the chosen Example.

## `internal CAnthology(LEntryPort entries, LPortraitPort portraits, LSettingsPort settings, CDesk desk, Func<bool> shownSeam, CEnvoy envoy, Func<bool, bool> finishSeam)`

Takes the ports it reads and prints through, the corpus desk holding the transcript, and the panel's seams.
The panel and the citation gates read a failure's ready notice through `settings` and show it through `envoy`.
The panel asks whether the desk changed before it leaves an Example.

## `internal static CAnthology LAnthologyCreate(CAtelier atelier, CDesk desk, Func<bool> shownSeam, CEnvoy envoy, Func<bool, bool> finishSeam)`

Builds the list over the atelier's ports, so the driver hands it no port.
Building it is no user action, so it is no gate on the atelier.

## `internal bool LAnthologyNarrowed`

Whether the query or the filter hides any row, as the vista answers it.

## `internal void LAnthologyVistaRestore(LVista vista)`

Binds the list and its panel to the corpus vista a workspace start or switch hands over.

## `public void CAnthologyOrderSet(CCatalogOrder? order)`

Hands a chosen ordering to the vista, which keeps its own when the sender is no order row.

## `internal IReadOnlyList<CCatalogExample> LAnthologyRowsRead(string unknown, string unwritten)`

The rows the vista lists, none before a vista arrives.
The driver hands the words for an unknown or unwritten text, and the engine words each row with them.

## `public IReadOnlyDictionary<long, int> CAnthologyUsageRead()`

How many places quote each Example, keyed by id.

## `public CMentionResult? CAnthologyMentionRead(int offset)`

What a click at `offset` in the chosen Example's text found, read fresh from the engine.
Null while no Example is chosen, so the excerpt hands the window nothing.

## `internal Task LAnthologyPortraitPrint(CEnvoy envoy, LSettingsPort settings)`

Prints the chosen Example through `CPortrait`, with the Example realm's legend.
No driver calls it, since `CCorpusPortraitPrint` chooses which side prints.

## `public CProffer CAnthologyCitationRead(string word)`

The Sources the transcript's citation drawer offers for the typed word, ready to show.
It is the card sentence's citation find over the draft's own Example, so both fields offer alike.
The engine trims, splits, caps and counts, and answers no rows on a failed search.
No held transcript offers nothing, and typing a citation writes nothing.

## `public void CAnthologyCitationSet(long referenceId)`

Points the transcript's citation at a Source the drawer offered.
Nothing is written while the desk fills its fields.

## `public void CAnthologyCitationSet(string title)`

Points the transcript's citation at the Source the engine resolves the typed title to, in one engine call.
The engine reads the Source cited now off the held draft, so an unchanged title keeps it.
A failure shows `Reference.CreateFailed` and leaves the citation as it was.

## `private LTenure? CAnthologyTenure`

The held transcript draft, or null while the desk fills one, as the quill reads then.

## `public void CAnthologyGlossSet(long glossId, string text)`

The transcript's gloss text, deferred like the sentence text.
It is the card sentence's gloss gate over the Example itself, with card and sentence zero.

## `public void CAnthologyLanguageSet(long glossId, string language)`

The gate for a language picked in a transcript gloss's language menu, sent at once.

## `public void CAnthologyGlossAdd(int below)`

The gate for the gloss button, adding a gloss below the row at that place.
Below means the next place, as `CSentenceAdd` puts a sentence.
The engine picks the gloss language.

## `public bool CAnthologyGlossPrepare()`

The gate for the empty transcript's seed field taking focus.
The engine adds the first gloss only when the Example holds none, and answers whether it did.

## `public void CAnthologyGlossRemove(long glossId)`

The gate for the cross on a transcript gloss row.

## `public static bool CAnthologyTextCheck(string text, CStateValue value)`

Whether a field showing `text` already shows `value`, as the engine would store the field.
The transcript keeps a matching field untouched, so a bulletin never moves the caret.
It is static, since it reads no state of the list.

## `internal CExample? LAnthologyDraftRead(LDraft? draft)`

Maps the Example a draft carries to its shape, and a draft of another subject to null.
The engine answers the cited Source's line for the draft, so the driver never looks a name up.
`CCorpus` calls it for the transcript desk and for the anthology's loaded drafts.
So the corpus names no engine record.

## `private string LAnthologyCitationRead(LDraft draft)`

The ready line of the Source the draft's Example cites.
A failed read shows `Reference.LoadFailed` and shows no line, as the old Source list read did.

## `internal static CExample? LAnthologyExampleRead(LExample? example, string citation)`

Maps a stored Example and its ready citation line to its shape, and null to null.
The excerpt links only the Mentions the Example lets a reading link, while the transcript keeps them all.

## `private static CCatalogExample LAnthologyRowRead(LCatalogExample row)`

Maps one found Example to its row, with the text the engine worded.
