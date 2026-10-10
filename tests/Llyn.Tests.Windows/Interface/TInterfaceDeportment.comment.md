# TInterfaceDeportment.cs
Hash: `97c0ea9afb5c428c`

## `[assembly: CollectionBehavior(DisableTestParallelization = true)]`

The Windows tests build workspaces from the portable suite's fixture, whose disposal clears every SQLite pool.
The portable suite's own switch covers only its assembly, so this one repeats it here.
Two workspaces alive in parallel let one disposal close a connection the other is opening.

## `internal static class TInterfaceDeportment`

The relays for the deportment classes the Windows tests drive.
It is a class of its own rather than a part of `TInterface`.
This relay class is not partial.
Some relays construct bare WPF surfaces to expose production painters and handlers.
The posture relay builds its deportment over a real workspace folder.
The index item and caret key relays reach internal statics and make no WPF object.
The rime-book and script switch relays hand a WPF toggle through, so their caller runs them on an STA thread.
The reflex type relay copies the held row with the typed text and hands it to the row's refine.
The etymology relays reach the field's internal show and read its two faces by their place in the body.
The card position attach hangs one card in a bare list with only the number box in its row.
It lays the list out so its row exists, then hands the row painter to the real item watcher.
It returns the box the painter finds by its part name, so the caller runs it on an STA thread.
The card row relay hands transcription rows to the shared row matcher with the sheet's key and painters.
The scheme relay reaches a transcription row's internal choice refresh, which makes no WPF object.
The localization relays build the settings language choice over a given surface and refine it.
The choice is internal, so its create relay hands it back as an object, as the card relay does.
Its surface holds a WPF combo box, so the caller runs the localization relays on an STA thread.
The analysis relays build the custom analysis switch over a given surface and refine it.
Its create relay hands the internal driver back as an object, as the localization relay does.
Its surface holds a WPF toggle, so the caller runs the analysis relays on an STA thread.

## `internal static QLecternReflex TLecternCreate(CDisplay display, FrameworkElement surface)`

Builds the reading reflex section over `display`, pulling its list, loading line and hinge from `surface`.
The section subscribes the hinge itself, so the test wires no observer.
The surface holds WPF controls, so the caller runs it on an STA thread.

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

## `internal static void TDisplayEntryShow(this CDisplay display, long id, LEntryDraft draft)`

Shows `draft` as entry `id` on the display's sound half, so its reflex reads answer for that entry.

## `internal static void TReflexCreate(FrameworkElement surface, CEditor editor)`

Builds the editor's reflex block over `surface` with its own anchor menu, then wires it to `editor`.
The surface holds WPF controls, so the caller runs it on an STA thread.

## `internal static void TCadenceCreate(FrameworkElement surface, CEditor editor, CLedger ledger, CEnvoy envoy)`

Builds `QCadence` over `surface` and introduces the editor's facets to it.
The surface carries the rime-book and script boxes under their markup names.
So a switch click reaches the real fold gate through the real driver.
The boxes are WPF controls, so the caller runs it on an STA thread.

## `internal static QLecternSound TLecternSoundCreate(CDisplay display, FrameworkElement surface, CLedger ledger, CEnvoy envoy)`

Builds the reading view's sound section over the display's sound and fold areas and `surface`.
So a reading-view box switch click reaches the display's real fold gate through the real driver.

## `internal static ToggleButton TFanqieSwitchRead(this QFanqie box)`

The rime-book box's open switch, as the box exposes it to its driver.

## `internal static ToggleButton TScriptSwitchRead(this QScript box)`

The script box's open switch, as the box exposes it to its driver.

## `internal static void TEtymologySourceShow(this QEtymology etymology, bool linked)`

Shows the field with no etymon and the source link on or off as given.

## `internal static (Visibility, Visibility) TEtymologyFaceRead(this QEtymology etymology)`

Reads the visibility of the field's two faces by their place in the body.

## `internal static void TReflexTypeRefine(QReflexItem row, CReflexField field, string text)`

Copies the held row through `CReflex.CReflexTypedApply`, as the gate does, and hands it to the row's refine.

## `internal static object TCardCreate()`

Builds a card with no rows over a blank draft.
The card is internal, so the relay hands it back as an object.

## `internal static void TCardLabelShow(object card, IReadOnlyList<CTagDraft> drafts)`

Shows the tag drafts on the card's label through `QLabel.QLabelShow`, as the card list does.

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

## `internal static object TCardFoldCreate(CCardDraft draft)`

Builds a card over `draft`, which also names its id.
Its peek is the definition, as a meaning list picks it.
It shows the draft's fold on the card as the list does.
The card is internal, so the relay hands it back as an object.

## `internal static void TCardRowRefine(FrameworkElement container, object card)`

Runs the card row painter on a laid out row with no changed property, as a full fill does.

## `internal static void TCardHingeAttach(ItemsControl list, CEditor editor)`

Builds a card driver and introduces the editor's field and list facets to it.
The field drivers it never reaches are passed as null, since the row holds only a hinge.
It attaches the real card fill to `list`, so a hinge click reaches the real fold gate.

## `internal static object? TLookSettingRead(string style, string part, DependencyProperty property, bool folded)`

Reads the fixed value the look sheet gives `property` on `part` of `style`.
It picks the last row whose cues hold, as the look handler does.
A folded hinge holds the checked cue, and an unfolded one holds only the base cue.
It returns null when no row holds, or when the winner is no fixed `Freezable`.

## `internal static object TLeafItemCreate(CLeaf leaf, CStateWording peek)`

Builds the reading card item over a ready leaf and its peek.
The item is internal, so the relay hands it back as an object.

## `internal static void TLeafCardRefine(FrameworkElement container, object card)`

Runs the reading card painter on a laid out row.

## `internal static QLecternCard TLecternCardCreate(CDisplay display, FrameworkElement surface)`

Builds the reading card section over the display's card, route and sound areas and the given surface.
The surface holds WPF lists, so the caller runs it on an STA thread.

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

## `internal static object TInflectionCreate(FrameworkElement settings, CLedger ledger, CEnvoy envoy)`

Builds the custom analysis switch over the given surface and introduces the ledger and the envoy to it.
The switch driver is internal, so the relay hands it back as an object.
The surface holds a WPF toggle, so the caller runs it on an STA thread.

## `internal static void TInflectionRefine(object analysis)`

Refines the switch from the ledger's read of the stored setting.
The caller runs it on an STA thread, since the switch is a WPF toggle.
