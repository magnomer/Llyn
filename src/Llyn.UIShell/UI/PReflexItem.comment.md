# PReflexItem.cs

## `internal sealed class PReflexItem : INotifyPropertyChanged`

One reflex row as the reading view and the input panel bind it.
It carries the language, kind, text, note, region, remark and main mark of one draft row.
It also knows whether it leads its language group, and whether it is folded away under the visible rows.
It holds the fanqie ids the row is anchored to and the label they print as.
It also knows whether the row may be anchored at all.
The panel edits it through its properties, and the view only reads them.

## `public long PReflexItemId { get; }`

The draft row's id, by which the panel's requests name it.

## `public string PReflexItemLanguage`

The borrowing language as stored, on every row of the group.

## `public string PReflexItemKind`

The kind printed before the reading as stored, such as the kind of a Japanese on'yomi.
Empty when the language sorts its readings by no kind.

## `public bool PReflexItemRespelled`

Whether the text is the row's respelling, fixed when the row is built.
It is read from the switch and the row's own language.
So a Korean row stays plain while a Mandarin row is respelled.
A change of that state rebuilds the row.

## `public bool PReflexItemPhonemic { get; }`

Whether the row's language is phonemic, fixed when the row is built, so its reading is drawn between slashes.

## `public bool PReflexItemFolded { get; }`

Whether the row belongs to a language the pack folds away, fixed when the row is built.

## `public string PReflexItemOpener`

The slash drawn before the reading of a phonemic language, or nothing.

## `public string PReflexItemCloser`

The slash drawn after the reading of a phonemic language, or nothing.

## `public string PReflexItemText`

The reading as typed or fetched, bare, in the form the switch picks for the row's language.

## `public string PReflexItemRegion`

The place the reading is taken from, as fetched, never typed.
Both the view and the editor show it on the lead row when its language is hovered.

## `public string PReflexItemRemark`

What the source says of the reading, printed after the note and edited on the row.

## `public bool PReflexItemHidden`

Whether the row is hidden in the stack, true for a folded row while the fold is closed.
The panel and the view set it from the shared fold state.

## `public string PReflexItemNote`

The note printed after the reading as stored, such as the pinyin of a Mandarin reading or the Korean 훈.
Empty when the reading carries none.

## `public bool PReflexItemMain`

Whether the row is the reading in common use, drawn in the accent colour.

## `public bool PReflexItemLead`

Whether the row is the first of its language group, so it alone prints the language.
The panel sets it after every reorder, since a row leads by position rather than by content.

## `public IReadOnlyList<long> PReflexItemAnchors`

The fanqie ids the row is tied to, sorted, raised only when the set changes.
The label is not derived here, since the item never sees the fanqie rows.

## `public string PReflexItemAnchor`

The anchored placements printed after the remark, written by the pane that holds the fanqie rows.

## `public bool PReflexItemAnchorable`

Whether the editor row shows its anchor label at all.
A multi-character headword and a character without stored placements show none.

## `public string PReflexItemHead`

The language the field of the row shows: the language on a lead row, nothing beneath it.
Writing it writes the language, so typing into a blank field starts a new group.

## `public string PReflexItemLabel`

The localized language on a lead row, or nothing beneath it, for the reading view.

## `public string PReflexItemArea`

The region on a lead row, or nothing beneath it, so the language name alone carries the hover.

## `public string PReflexItemTag`

The localized kind, or nothing when the row has none.

## `internal static PReflexItem PReflexItemCreate(`

Builds the row for one draft reflex, printing the form `respelling` picks for the row's language.
`phonemic` decides the slashes and `folded` whether the row starts hidden.

## `internal static string PReflexTextRead(LReflexDraft draft, PRespelling respelling)`

The form of one draft reflex to print, its respelling while shown and filled and its reading otherwise.

A row from one draft row.

## `internal static string PReflexLabelFormat(PWindow host, string name)`

A language or kind as the view prints it, through the `Reflex.` localization keys, or as the pack spells it.

## `private bool PReflexItemSet(ref string field, string? value, string name)`

Writes one text field and raises its change, reporting whether anything changed.

## `private void PReflexItemRaise(string name)`

Raises the change of one property.
