# PReflexItem.cs

## `internal sealed class PReflexItem : INotifyPropertyChanged`

One reflex row as the reading view and the input panel bind it.
It carries the language, the kind, the text, the note and the main mark of one draft row.
It also knows whether it leads its language group.
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

## `public string PReflexItemText`

The reading as typed or fetched, in the form the switch picks for the row's language.

## `public string PReflexItemNote`

The note printed after the reading as stored, such as the pinyin of a Mandarin reading or the Korean 훈.
Empty when the reading carries none.

## `public bool PReflexItemMain`

Whether the row is the reading in common use, drawn in the accent colour.

## `public bool PReflexItemLead`

Whether the row is the first of its language group, so it alone prints the language.
The panel sets it after every reorder, since a row leads by position rather than by content.

## `public string PReflexItemHead`

The language the field of the row shows: the language on a lead row, nothing beneath it.
Writing it writes the language, so typing into a blank field starts a new group.

## `public string PReflexItemLabel`

The localized language on a lead row, or nothing beneath it, for the reading view.

## `public string PReflexItemTag`

The localized kind, or nothing when the row has none.

## `internal static PReflexItem PReflexItemCreate(PWindow host, LReflexDraft draft, PRespelling respelling)`

Builds the row for one draft reflex, printing the form `respelling` picks for the row's language.

## `internal static string PReflexTextRead(LReflexDraft draft, PRespelling respelling)`

The form of one draft reflex to print, its respelling while shown and filled and its reading otherwise.

A row from one draft row.

## `internal static string PReflexLabelFormat(PWindow host, string name)`

A language or kind as the view prints it, through the `Reflex.` localization keys, or as the pack spells it.

## `private bool PReflexItemSet(ref string field, string? value, string name)`

Writes one text field and raises its change, reporting whether anything changed.

## `private void PReflexItemRaise(string name)`

Raises the change of one property.
