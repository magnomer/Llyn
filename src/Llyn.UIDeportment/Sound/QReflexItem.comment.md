# QReflexItem.cs
Hash: `074d998969cf1720`

## `public sealed class QReflexItem : INotifyPropertyChanged`

One reflex row as the reading view and the input panel bind it.
It carries the language, kind, text, romanization, meaning, note, region and main mark of one draft row.
It also knows whether it leads its language group, and whether it is folded away under the visible rows.
It holds the anchor label the row prints.
It also knows whether the row may be anchored at all.
The view only reads it, and the panel's cell editors write its editable properties.
Every value comes ready from a `CReflex` or from the gate's answer to an edit.
So the row holds no session and asks nothing.
`PropertyChanged` only tells the view a value changed and never carries a request.

## `private CReflex _qReflexItemReflex`

The ready reflex the row last took, which answers whether the row hides under the fold.

## `private string _qReflexItemTitle`

The localization key of the row's language, as Conduct chose it.

## `private string _qReflexItemRubric`

The localization key of the row's kind, as Conduct chose it.

## `public QReflexItem(CReflex reflex)`

Builds the row from one ready reflex.
The fold refine that follows every build decides whether it starts hidden.

## `public long QReflexItemId { get; }`

The draft row's id, by which the panel's gates name it.

## `public bool QReflexItemFolded { get; private set; }`

Whether the row belongs to a language the pack folds away.

## `public string QReflexItemOpener`

The opener Conduct chose for the row's language.

## `public string QReflexItemCloser`

The closer Conduct chose for the row's language.

## `public event PropertyChangedEventHandler? PropertyChanged;`

The binding's only notice that a property changed, raised through `QReflexChangeRefine`.
It names the property and carries no request.

## `internal event Action<QReflexItem, CReflexField, string>? QReflexItemTyped`

The edit channel carries a cell editor's write, with the cell it names and the text typed.
Writing an editable property raises it and changes nothing on the row.
The panel hands it to the gate and writes the answer back through `QReflexTypeRefine`.
A reading-view row has no listener, so nothing happens there.

## `public string QReflexItemKind`

The kind printed before the reading as stored, such as the kind of a Japanese on'yomi.
Empty when the language sorts its readings by no kind.
Writing it, like writing the reading, romanization, meaning or note, only raises `QReflexItemTyped`.

## `public string QReflexItemText`

The reading as typed or fetched, bare, in the form the mark picks for the row's language.

## `public string QReflexItemRomanization`

The romanized reading printed after the reading itself.

## `public string QReflexItemMeaning`

The lexical meaning paired with the reading and editable by the user.

## `public string QReflexItemNote`

What the source says of the reading, printed after its meaning, such as `literary`.
Empty when the reading carries none.

## `public bool QReflexItemMain`

Whether the row is the reading in common use, drawn in the accent colour.

## `public bool QReflexItemLead`

Whether the row is the first of its language group, so it alone prints the language.
Conduct marks it on every read, and the panel asks Conduct again while a language is typed.

## `public bool QReflexItemHidden`

Whether the row is hidden in the stack, true for a folded row while the fold is closed.
The panel and the view set it from the shared fold state.

## `public string QReflexItemAnchor`

The anchored placements printed after the note, written by the pane that holds the fanqie rows.

## `public bool QReflexItemAnchorable`

Whether the editor row shows its anchor label at all.
A multi-character headword and a character without stored placements show none.

## `public string QReflexItemHead`

The field of the row shows the language on a lead row and nothing beneath it.
Writing it sends the language through `QReflexItemTyped`, so typing into a blank field starts a new group.

## `public string QReflexItemLabel`

The localized language on a lead row, or nothing beneath it, for the reading view.
A typed language takes its key from the gate's answer, so the label follows the edit at once.

## `public string QReflexItemArea`

The region on a lead row, or nothing beneath it, so the language name alone carries the hover.
The region is the place the reading is taken from, as fetched, never typed.

## `public string QReflexItemTag`

The localized kind, or nothing when the row has none.

## `internal static void QReflexItemRefine(FrameworkElement container, object item, string? _)`

Fills a row of `Theme.Reflex.Row` or `Theme.Reflex.Display` from the row's values.
A row hidden under the fold is unseen and flat, yet still measured for the shared columns.
The language carries the region as its tooltip, and no tooltip when the row names none.
A row in common use draws its reading and slashes in the accent colour.
The editor's reading cell names the accent field style for such a row, so its box opens accented.
The fill also gives each side cell its `QQuillCell` order and tags the slot with `Theme.Reflex.Control`.
Those order strings live here, so the Veneer markup carries none.
The anchor button shows only for an anchorable row and holds the anchor label.

## `private static void QReflexTextRefine(FrameworkElement container, string name, string text, bool main)`

Sets one named text of a row, in the accent colour for a row in common use.

## `internal void QReflexStateRefine(CReflex reflex)`

Takes every value of a ready reflex in place, except its anchors.
So a changed mark or fold needs no new row.
The anchor labels arrive apart, through `QReflexAnchorRefine`.
It keeps the reflex and copies its label keys and its lead.
The respelled mark is taken before the text, so the text arrives under the mark it is shown with.
Each field announces only a real change, and no announcement sends a request.

## `internal void QReflexTypeRefine(CReflexTyped typed)`

Writes the one cell the gate's answer names with the text the gate says it holds.
It is the edit path's only writer, so the row never keeps a typed copy of its own.
The binding reads the cell back after the write, so the editor shows the answer.
A language or kind cell also takes the answer's key, so the label and tag name the typed text.

## `private static string QReflexLabelRefine(string key, string name)`

A language or kind as the view prints it, looked up under the key Conduct chose.
A missing translation prints the name as the pack spells it, and a blank name prints nothing.

## `internal static void QReflexLeadRefine(IReadOnlyList<QReflexItem> rows, IReadOnlyList<CReflexHead> heads)`

Pairs each row with the answer `CKindred.CKindredSet` gave for its id and copies its lead.
The editor calls it while a language is typed, before the draft brings the marks back.

## `internal static void QReflexAnchorRefine(IReadOnlyList<QReflexItem> rows, bool anchorable, IReadOnlyDictionary<long, string> texts)`

Writes each row's anchor label by its id, and whether the row may be anchored at all.
It is the one writer of both, so the reading view's ready labels and the editor's land alike.
A row the labels do not name shows no anchor.

## `internal static void QReflexFoldRefine(IReadOnlyList<QReflexItem> rows, ToggleButton fold, bool opened)`

Hides each row its reflex's `CReflexHiddenCheck` hides, and shows the fold state on the toggle.
The toggle is visible only when some row is folded.

## `private void QReflexValueRefine(ref string field, string value, params string[] names)`

Writes one text field and announces each named property, only when the text changed.
The names are the properties that read the field, such as the head and label of the language.

## `private void QReflexChangeRefine(string name)`

Announces the change of one property, so the list fills the row again.
