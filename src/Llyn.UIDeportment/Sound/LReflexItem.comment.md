# LReflexItem.cs

## `public sealed class LReflexItem : INotifyPropertyChanged`

One reflex row as the reading view and the input panel bind it.
It carries the language, kind, text, romanization, meaning, note, region and main mark of one draft row.
It also knows whether it leads its language group, and whether it is folded away under the visible rows.
It holds the fanqie ids the row is anchored to and the label they print as.
It also knows whether the row may be anchored at all.
The panel edits it through its properties, and the view only reads them.

## `public long LReflexItemId { get; }`

The draft row's id, by which the panel's requests name it.

## `public string LReflexItemLanguage`

The borrowing language as stored, on every row of the group.

## `public string LReflexItemKind`

The kind printed before the reading as stored, such as the kind of a Japanese on'yomi.
Empty when the language sorts its readings by no kind.

## `public bool LReflexItemRespelled`

Whether the text is the row's respelling, fixed when the row is built.
It is read from the switch and the row's own language.
So a Korean row stays plain while a Mandarin row is respelled.
A change of that state rebuilds the row.

## `public bool LReflexItemMatch(bool respelled, bool phonemic, bool folded, IReadOnlyList<long> anchors)`

Whether the row was built under these three pack facts and these anchors, as the engine compares them.
A changed pack or anchor set rebuilds the row rather than patching it.

## `public bool LReflexItemPhonemic { get; }`

Whether the row's language is phonemic, fixed when the row is built, so its reading is drawn between slashes.

## `public bool LReflexItemFolded { get; }`

Whether the row belongs to a language the pack folds away, fixed when the row is built.

## `public string LReflexItemTone { get; set; }`

The tone of the reading as the engine cut it, such as `55`, or empty.
The anchor dropdown reads it to estimate which placements the reading descends from.

## `public string LReflexItemOpener`

The slash drawn before the reading of a phonemic language, or nothing.

## `public string LReflexItemCloser`

The slash drawn after the reading of a phonemic language, or nothing.

## `public string LReflexItemText`

The reading as typed or fetched, bare, in the form the switch picks for the row's language.

## `public string LReflexItemRomanization`

The romanized reading printed after the reading itself.

## `public string LReflexItemMeaning`

The lexical meaning paired with the reading and editable by the user.

## `public string LReflexItemRegion`

The place the reading is taken from, as fetched, never typed.
Both the view and the editor show it on the lead row when its language is hovered.

## `public bool LReflexItemHidden`

Whether the row is hidden in the stack, true for a folded row while the fold is closed.
The panel and the view set it from the shared fold state.

## `public string LReflexItemNote`

What the source says of the reading, printed after its meaning, such as `literary`.
Empty when the reading carries none.

## `public bool LReflexItemMain`

Whether the row is the reading in common use, drawn in the accent colour.

## `public bool LReflexItemLead`

Whether the row is the first of its language group, so it alone prints the language.
The panel sets it after every reorder, since a row leads by position rather than by content.

## `public IReadOnlyList<long> LReflexItemAnchors { get; }`

The fanqie ids the row is tied to, fixed when the row is built from the engine's draft.
A tick in the anchor menu reaches the engine, and the row comes back rebuilt.
The label is not derived here, since the item never sees the fanqie rows.

## `public string LReflexItemAnchor`

The anchored placements printed after the note, written by the pane that holds the fanqie rows.

## `public bool LReflexItemAnchorable`

Whether the editor row shows its anchor label at all.
A multi-character headword and a character without stored placements show none.

## `public string LReflexItemHead`

The language the field of the row shows: the language on a lead row, nothing beneath it.
Writing it writes the language, so typing into a blank field starts a new group.

## `public string LReflexItemLabel`

The localized language on a lead row, or nothing beneath it, for the reading view.

## `public string LReflexItemArea`

The region on a lead row, or nothing beneath it, so the language name alone carries the hover.

## `public string LReflexItemTag`

The localized kind, or nothing when the row has none.

## `public static LReflexItem LReflexItemCreate(`

Builds the row for one draft reflex, printing the form `respelling` picks for the row's language.
`phonemic` decides the slashes and `folded` whether the row starts hidden.

## `public static List<LReflexItem> LReflexItemScan(LWindow window, IReadOnlyList<LReflexDraft> reflexes, HashSet<string> folded)`

The rows for `reflexes` in order, each built as the single form below builds it.
The reading view hands its reflexes in whole, so it never walks a draft itself.

## `public static LReflexItem LReflexItemCreate(LWindow window, LReflexDraft reflex, HashSet<string> folded)`

A row for one draft reflex, asking the engine for its language's respelling state and phonemic flag.
The row is folded when its language is in `folded`.
Shared by the reading view and the editor, whose rows are built by the same rule.

## `public static string LReflexTextRead(LReflexDraft draft, LRespellingMark respelling)`

The form of one draft reflex to print, its respelling while shown and filled and its reading otherwise.

## `public static string LReflexLabelFormat(string name)`

A language or kind as the view prints it, through the `Reflex.` localization keys, or as the pack spells it.

## `public static HashSet<string> LReflexFoldRead(LWindow window, string language)`

The languages the pack of `language` folds away, read from its reflex rules.
Empty for a blank language or a pack without rules.

## `public static void LReflexLeadApply(IReadOnlyList<LReflexItem> rows)`

Marks each row that opens a run of one language as its lead, so the language prints once per run.
Shared by both panes, whose rows lead by the same rule.

## `public static void LReflexAnchorApply(`

Writes each row's anchor label from `fanqie`, and whether the row may be anchored at all.
The engine decides both, so a row it refuses prints nothing.
Shared by both panes, so the label reads the same in the editor and the reading view.

## `public static void LReflexFoldApply(IReadOnlyList<LReflexItem> rows, ToggleButton fold, bool opened)`

Hides every folded row while the fold is closed and shows it while open.
The toggle shows the state and is visible only when some row is folded.

## `private bool LReflexItemSet(ref string field, string? value, string name)`

Writes one text field and raises its change, reporting whether anything changed.

## `private void LReflexItemRaise(string name)`

Raises the change of one property.

## Inline notes

### `private const string LReflexAnchorSeparator = " · ";`

The mark between two anchored placements in a row's label.
