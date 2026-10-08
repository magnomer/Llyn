# PSentence.cs
Hash: `90fa6d54bdf5b393`

## `internal sealed class PSentence : INotifyPropertyChanged`

One Example row on a card.
The row carries the id of the row itself.
So a sentence the user rewrites stays the same row.
The id is the engine's, read from the draft, and the row never mints one.

A row standing empty is not one thing.
The sentence may never have been written.
Or it may have been written and be unknown now.
The second is marked rather than shown, and the converter reads the mark off the engine's value.
The row holds that value and nothing typed.
What is typed leaves through the editor as written text, and the draft answers with the state.
So the row never resolves a state and never holds a second copy of one.

The frame the card reads the sentence under lives in [PSentenceFrame](PSentenceFrame.comment.md).
The row re-raises every change the frame announces, so a list watching the row repaints it.

Each of the two frame fields offers what has already been saved for the language.
Nothing is shipped, so an empty store offers nothing.
The field is a plain box until something is written in it.

## `internal PSentence(ObservableCollection<QCitationItem> catalog, ObservableCollection<string> particles, ObservableCollection<string> dependences, ObservableCollection<PLanguageItem> languages, CSentenceDraft draft)`

The row for one of the engine's rows, blank or filled.
It holds the sentence, the Source it cites, the frame, and the Gloss rows as the draft holds them.
All of it stands under the id that names the row.
The language catalog is handed in for the Gloss pickers, shared with every other row.

## `internal long PSentenceRow`

The id of the row itself, which every request about the row names.

## `internal bool PSentenceCited { get; private set; }`

Whether the row cites a Source, as Conduct's sentence record states it.
The list driver reads it to keep a cited row's citation box shown while the list is idle.

## `public ObservableCollection<QCitationItem> PSentenceCitationCatalog { get; }`

The Sources the whole form offers, shared by every row.
So a Source written on one row is on offer to the next without reloading anything.

## `public ObservableCollection<PLanguageItem> PSentenceLanguageCatalog { get; }`

The languages the Gloss pickers offer, shared by every row.

## `public ObservableCollection<string> PSentenceParticleCatalog { get; }`

The markers already saved for the language, shared by every row.
Nothing ships one, so the list is empty until a user writes and saves one.

## `public ObservableCollection<string> PSentenceDependenceCatalog { get; }`

The roles already saved for the language, shared by every row.
Nothing ships one, so the list is empty until a user writes and saves one.

## `public PSentenceFrame PSentenceFrame { get; }`

The marker, the role and their placement, built once from the row's first draft.
Later drafts reach it through `PSentenceShow`, so the frame is never replaced.

## `public ObservableCollection<PGloss> PSentenceGloss { get; }`

The Gloss rows the row shows under its sentence, in the order the Example keeps them.

## `public PMentionLine PSentenceChip { get; }`

The chip line under the sentence field, showing the Mentions its Example holds.
The editor paints the chips from the sentence area's ready read after each redraw.
The row never resolves a Mention itself.
Every change goes out through a gate and comes back through the redraw.

## `internal event Action<PGloss, string>? PSentenceGlossNotice;`

Where a language picked in one of the Gloss rows goes, with the raw language.
The card subscribes when it builds the row, so the pick reaches the editor with its sentence.

## `public CStateWording PSentenceText`

The sentence as the draft holds it, set only from the draft.

## `public long? PSentenceCitation`

The Source the row cites, as the draft holds it.
The field shows its byline through a lookup made when the field is drawn, so the row keeps no name.

## `internal void PSentenceShow(CSentenceDraft draft)`

Redraws the row from the engine's row, field by field, only where the value changed.
The row id is always taken, because the engine is the only minter.
A field already reading what the engine holds is left alone, so the caret survives its own echo.

## `internal void PSentenceCitationShow()`

Says the cited Source again, for when the list of Sources changed underneath the row.
The field then looks the byline up afresh.

## `internal static string PSentenceCitationFind(ObservableCollection<QCitationItem> catalog, long? anchor)`

The `Author (Year)` line of the Source an anchor names, or nothing when it names none.
The row fill calls it when a citation field is drawn.
The citation driver calls it to reset the field once a typed line is settled or abandoned.

## `public event PropertyChangedEventHandler? PropertyChanged;`

Raised for the row's own text and Source, and for every change its frame announces.

## `private void PSentenceGlossShow(IReadOnlyList<CGlossDraft> drafts)`

Redraws the Gloss rows from the drafts, keeping the rows whose ids survive.

## `private PGloss PSentenceGlossCreate(CGlossDraft draft)`

One Gloss row, subscribed so its language pick reaches the card through the notice.
