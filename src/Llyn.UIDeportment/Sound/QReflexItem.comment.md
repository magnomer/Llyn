# QReflexItem.cs

## `public sealed class QReflexItem : INotifyPropertyChanged`

One reflex row as the reading view and the input panel bind it.
It carries the language, kind, text, romanization, meaning, note, region and main mark of one draft row.
It also knows whether it leads its language group, and whether it is folded away under the visible rows.
It holds the fanqie ids the row is anchored to and the label they print as.
It also knows whether the row may be anchored at all.
The panel edits it through its properties, and the view only reads them.
Every value comes ready from a `CReflex`, so the row holds no session and asks nothing.

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

The draft row's id, by which the panel's requests name it.

## `public bool QReflexItemRespelled`

Whether the text is the row's respelling, as the mark Conduct chose says.
So a Korean row stays plain while a Mandarin row is respelled.

## `public bool QReflexItemFolded`

Whether the row belongs to a language the pack folds away.

## `public string QReflexItemTone`

The tone of the reading as the engine cut it, such as `55`, or empty.
The anchor dropdown reads it to estimate which placements the reading descends from.

## `public IReadOnlyList<long> QReflexItemAnchors`

The fanqie ids the row is tied to, as the engine's draft holds them.
A tick in the anchor menu reaches the engine, and the row takes the new ids in place.
The label is not derived here, since the item never sees the fanqie rows.

## `public string QReflexItemOpener`

The opener Conduct chose for the row's language.

## `public string QReflexItemCloser`

The closer Conduct chose for the row's language.

## `public string QReflexItemLanguage`

The borrowing language as stored, on every row of the group.

## `public string QReflexItemKind`

The kind printed before the reading as stored, such as the kind of a Japanese on'yomi.
Empty when the language sorts its readings by no kind.

## `public string QReflexItemText`

The reading as typed or fetched, bare, in the form the mark picks for the row's language.

## `public string QReflexItemRomanization`

The romanized reading printed after the reading itself.

## `public string QReflexItemMeaning`

The lexical meaning paired with the reading and editable by the user.

## `public string QReflexItemNote`

What the source says of the reading, printed after its meaning, such as `literary`.
Empty when the reading carries none.

## `public string QReflexItemRegion`

The place the reading is taken from, as fetched, never typed.
Both the view and the editor show it on the lead row when its language is hovered.

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

The language the field of the row shows: the language on a lead row, nothing beneath it.
Writing it writes the language, so typing into a blank field starts a new group.

## `public string QReflexItemLabel`

The localized language on a lead row, or nothing beneath it, for the reading view.
A typed language keeps the last key until the draft comes back with the new one.

## `public string QReflexItemArea`

The region on a lead row, or nothing beneath it, so the language name alone carries the hover.

## `public string QReflexItemTag`

The localized kind, or nothing when the row has none.

## `internal static void QReflexItemRefine(FrameworkElement container, object item, string? _)`

Fills a row of `Theme.Reflex.Row` or `Theme.Reflex.Display` from the row's values.
A row hidden under the fold is unseen and flat, yet still measured for the shared columns.
The language carries the region as its tooltip, and no tooltip when the row names none.
A row in common use draws its reading and slashes in the accent colour.
The editor's reading cell names the accent field style for such a row, so its box opens accented.
The fill also gives each side cell its `QFieldCell` order and tags the slot with `Theme.Reflex.Control`.
Those order strings live here, so the Veneer markup carries none.
The anchor button shows only for an anchorable row, its prompt standing in while no anchor is set.

## `private static void QReflexTextRefine(FrameworkElement container, string name, string text, bool main)`

Sets one named text of a row, in the accent colour for a row in common use.

## `internal void QReflexStateRefine(CReflex reflex)`

Takes every value of a ready reflex in place.
So a changed mark, fold or anchor set needs no new row.
It keeps the reflex and copies its label keys and its lead.
The respelled mark is taken before the text, so an edit echo names the field the text now shows.
Each setter announces only a real change.

## `private static string QReflexLabelRefine(string key, string name)`

A language or kind as the view prints it, looked up under the key Conduct chose.
A missing translation prints the name as the pack spells it, and a blank name prints nothing.

## `internal static void QReflexLeadRefine(IReadOnlyList<QReflexItem> rows, IReadOnlyList<CReflexHead> heads)`

Pairs each row with the answer `CEditor.CEditorLeadRead` gave for its id and copies its lead.
The editor calls it while a language is typed, before the draft brings the marks back.

## `internal static void QReflexAnchorRefine(`

Writes each row's anchor label through `format`, and whether the row may be anchored at all.
The caller's Conduct read decides both, so the fanqie rows never reach this item.
Shared by both panes, so the label reads the same in the editor and the reading view.

## `internal static void QReflexFoldRefine(IReadOnlyList<QReflexItem> rows, ToggleButton fold, bool opened)`

Hides each row its reflex's `CReflexHiddenCheck` hides, and shows the fold state on the toggle.
The toggle is visible only when some row is folded.

## `private bool QReflexValueRefine(ref string field, string? value, string name)`

Writes one text field and announces its change, reporting whether anything changed.

## `private void QReflexChangeRefine(string name)`

Announces the change of one property, so the list fills the row again.
