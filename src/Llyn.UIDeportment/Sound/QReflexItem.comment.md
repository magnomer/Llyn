# QReflexItem.cs
Hash: `44f12a638273652f`

## `public sealed class QReflexItem : INotifyPropertyChanged`

One reflex row as the reading view and the input panel bind it.
It carries the language, kind, text, romanization, meaning, note, region and main mark of one draft row.
It also knows whether it leads its language group, and whether it is folded away under the visible rows.
It holds the anchor label the row prints.
It also knows whether the row may be anchored at all.
The view only reads it, and the panel's cell editors write its editable properties.
Reflex values arrive ready from Conduct.
The list supplies separate lead, fold and anchor presentation.
So the row holds no session and asks nothing.
`PropertyChanged` only tells the view a value changed and never carries a request.

## `private CReflex _qReflexItemReflex;`

The ready reflex the row last took, or its copy after a typed answer.
Editable text and label keys read the held reflex.
Lead, hidden state and anchor presentation remain separate.

## `public QReflexItem(CReflex reflex)`

Builds the row from one ready reflex.
Construction copies the reflex and its lead.
Hidden state is painted separately by `QReflexList.QReflexFoldRefine`.

## `public long QReflexItemId { get; }`

The draft row's id, by which the panel's gates name it.

## `public string QReflexItemOpener`

The opener Conduct chose for the row's language.

## `public string QReflexItemCloser`

The closer Conduct chose for the row's language.

## `public event PropertyChangedEventHandler? PropertyChanged;`

The binding's only notice that a property changed, raised through `QReflexChangeRefine`.
It names the property and carries no request.

## `internal event Action<QReflexItem, CReflexField, string>? QReflexItemTyped;`

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
The panel's and the view's `QReflexList` set it from their entry's stored opening.

## `public string QReflexItemAnchor`

The anchored placements printed after the note, written through `QReflexList.QReflexAnchorRefine`.

## `public bool QReflexItemAnchorable`

Whether the editor row shows its anchor label at all.
A multi-character headword and a character without stored placements show none.

## `public string QReflexItemHead`

The field of the row shows the language on a lead row and nothing beneath it.
Writing it sends the language through `QReflexItemTyped`, so typing into a blank field starts a new group.

## `public string QReflexItemLabel`

The localized language on a lead row, or nothing beneath it, for the reading view.
A typed language takes its key from the gate's answer, so the label follows the edit at once.

## `public string? QReflexItemArea`

The region on a lead row, or null beneath it, so the language name alone carries the hover.
Conduct hands a blank region as null, so the value binds as the tooltip as it stands.
The region is the place the reading is taken from, as fetched, never typed.

## `public string QReflexItemTag`

The localized kind, or nothing when the row has none.

## `internal CReflex QReflexItemReflex`

The reflex the row holds, which `QReflexList` asks whether the row hides under the fold.

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
The anchor labels arrive apart, through `QReflexList.QReflexAnchorRefine`.
It keeps the reflex and copies its lead.
Compared properties announce only real changes, and no announcement sends a request.

## `internal void QReflexTypeRefine(CReflex? reflex)`

Keeps the row `CKindred.CKindredSet` answered, which Conduct copied through `CReflex.CReflexTypedApply`.
A null answer names a row the draft lacks, so the row keeps what it holds.
It accepts the gate's answer rather than inventing a typed copy.
The binding reads the cell back after the write, so the editor shows the answer.
A typed language or kind keys its label from the copy, so the label and tag name the typed text.
The lead is left alone, since the answer's heads reach it through `QReflexList.QReflexLeadRefine`.

## `private static string QReflexLabelRefine(string key, string name)`

A language or kind as the view prints it, looked up under the key Conduct chose.
A null lookup falls back to `name`.
A successful lookup is used even when `name` is blank.

## `private void QReflexValueRefine(CReflex reflex)`

Keeps `reflex` and compares text, labels, main mark and bracket presentation.
The folded flag itself raises no property notification.
Bracket changes are notified before text changes.
A changed language announces the head and label, and a changed kind announces the kind and tag.

## `private void QReflexChangeRefine(string name)`

Announces the change of one property, so the list fills the row again.
