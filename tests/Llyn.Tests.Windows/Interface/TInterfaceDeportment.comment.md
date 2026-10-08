# TInterfaceDeportment.cs
Hash: `b7136271b67f9e05`

## `[assembly: CollectionBehavior(DisableTestParallelization = true)]`

The Windows tests build workspaces from the portable suite's fixture, whose disposal clears every SQLite pool.
The portable suite's own switch covers only its assembly, so this one repeats it here.
Two workspaces alive in parallel let one disposal close a connection the other is opening.

## `internal static class TInterfaceDeportment`

The relays for the deportment classes the Windows tests drive.
It is a class of its own rather than a part of `TInterface`.
The relay layer grows by owner and not by partial.
Each relay is transparent and carries no test logic of its own.
The posture relay builds its deportment over a real workspace folder.
The index item and caret key relays reach internal statics and make no WPF object.
The lectern fold relays hand a WPF toggle through, so their caller runs them on an STA thread.
The reflex type relay wraps typed text in a gate answer without heads and hands it to the row's refine.
The etymology relays reach the field's internal show and read its two faces by their place in the body.
The card position attach hangs one card in a bare list with only the number box in its row.
It lays the list out so its row exists, then hands the row painter to the real item watcher.
It returns the box the painter finds by its part name, so the caller runs it on an STA thread.
The card row relay hands transcription rows to the shared row matcher with the sheet's key and painters.
The scheme relay reaches a transcription row's internal choice refresh, which makes no WPF object.
The localization relays build the settings language choice over a given surface and refine it.
The choice is internal, so its create relay hands it back as an object, as the card relay does.
Its surface holds a WPF combo box, so the caller runs the localization relays on an STA thread.

## `internal static QLectern TLecternCreate(CEditor editor)`

Builds a lectern over the editor's display alone, with no panel.

## `internal static (bool CDeskBackward, bool CDeskForward) TDeskChronicleRead(this CDesk desk)`

Reads whether the desk's chronicle can step backward and forward.

## `internal static QPosture TPostureCreate(string root)`

Builds a posture and points its root at the given folder.

## `internal static IReadOnlyList<QIndexItem> TIndexItemBuild(IReadOnlyList<CVistaRow> rows)`

Reaches the internal item build, so a test builds index items from rows with no WPF object.

## `internal static bool TIndexItemMatch(QIndexItem held, QIndexItem fresh)`

Reaches the internal match rule between a held index item and a fresh one.

## `internal static void TIndexItemSync(QIndexItem held, QIndexItem fresh)`

Reaches the internal sync of a held index item to a fresh one.

## `internal static bool TCaretEdgeApply(string key, int caret, int length, int selection, Action<int> remove)`

Reaches the internal edge rule of the caret keys and answers whether it handled the key.

## `internal static bool TCaretStepApply(string key, int length, int selection, Func<int, bool> move, Action place)`

Reaches the internal step rule of the caret keys and answers whether it handled the key.

## `internal static void TDisplayFoldSet(this CEditor editor, bool opened)`

Sets the reflex fold of the editor's display open or shut.

## `internal static bool TDisplayFoldRead(this CEditor editor)`

Reads whether the reflex fold of the editor's display is open.

## `internal static void TLecternReflexAttach(this QLectern lectern, ItemsControl reflex, UIElement loading, ToggleButton fold)`

Introduces the reflex list, its loading line and its fold toggle to the lectern's sound.
The toggle is a WPF object, so the caller runs it on an STA thread.

## `internal static void TLecternFoldObserve(this QLectern lectern)`

Has the lectern's sound observe the display fold.

## `internal static void TEtymologySourceShow(this QEtymology etymology, bool linked)`

Shows the field with no etymon and the source link on or off as given.

## `internal static (Visibility, Visibility) TEtymologyFaceRead(this QEtymology etymology)`

Reads the visibility of the field's two faces by their place in the body.

## `internal static void TReflexFoldRefine(IReadOnlyList<QReflexItem> rows, ToggleButton fold, bool opened)`

Hands the rows and a WPF toggle to the internal fold refine.

## `internal static void TReflexTypeRefine(QReflexItem row, CReflexField field, string text)`

Wraps the typed text in a gate answer without heads and hands it to the row's refine.

## `internal static object TCardCreate()`

Builds a card with no rows over a blank draft.
The card is internal, so the relay hands it back as an object.

## `internal static void TCardLabelShow(object card, IReadOnlyList<CTagDraft> drafts)`

Shows the tag drafts on the card's label.

## `internal static bool TCardLabelMove(object card, int step)`

Moves the card's label position by the step and answers whether it moved.

## `internal static long? TCardLabelFind(object card, int step)`

Answers the id of the chip the card finds that step away, or none.

## `internal static (List<long>, int) TCardLabelRead(object card)`

Reads the ids of the card's label chips in order, with the card's label position.

## `internal static TextBox TCardPositionAttach(object card)`

Hangs the card in a bare list whose row holds only the number box.
It lays the list out so the row exists, then attaches the real item watcher.
It answers the box the painter finds by its part name, so the caller runs it on an STA thread.

## `internal static void TCardPositionShow(object card)`

Makes the card's position box active.

## `internal static void TCardPositionHide(object card)`

Hides the card's position box through the card's own hide.

## `internal static void TCardPositionSet(object card, int position)`

Sets the card's position number.

## `internal static void TCardRowShow(ObservableCollection<QTranscriptionItem> rows, IReadOnlyList<CTranscriptionDraft> drafts)`

Hands transcription rows and drafts to the shared row matcher with the sheet's key and painters.

## `internal static void TTranscriptionSchemeRefine(this QTranscriptionItem row, IReadOnlyList<CScheme> schemes)`

Reaches a transcription row's internal choice refresh with the given schemes, which makes no WPF object.

## `internal static object TLocalizationCreate(FrameworkElement settings, CLedger ledger, CEnvoy envoy)`

Builds the settings language choice over the given surface and introduces the ledger and the envoy to it.
The choice is internal, so the relay hands it back as an object.
The surface holds a WPF combo box, so the caller runs it on an STA thread.

## `internal static void TLocalizationRefine(object localization, IReadOnlyList<KeyValuePair<string, string>> languages, string chosen)`

Refines the choice with the given languages and the chosen one.
The caller runs it on an STA thread, since the choice holds a WPF combo box.
