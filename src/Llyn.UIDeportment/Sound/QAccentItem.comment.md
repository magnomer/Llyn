# QAccentItem.cs
Hash: `9174d3507fb27841`

## `public sealed class QAccentItem : INotifyPropertyChanged`

One pronunciation row after the primary, as the editor and the reading view show it.
It carries the draft row's id, its variety, the reading text the field is bound to, and its audio file.
The text arrives already in the form the respelling switch picks, and the brackets follow that choice.
The variety is shown as a flag when the pack draws varieties as flags, and as a label otherwise.
The flag may arrive after the row, so it notifies when it lands.
It stays public, since the reading cell binds its text by name.

## `private QAccentItem(`

Builds the row from plain values, under the mark the row prints between.
The flag key is kept, so a later flag lookup needs no language.

## `public string QAccentItemVariety { get; }`

The variety's stored name, never its localized label.
`QAccent` matches a spoken accent back to its row by it.

## `public string QAccentItemLabel`

The variety's localized name, blank while a flag stands for it.
Blank is what collapses the label, so a row never shows both.

## `internal event Action<QAccentItem, string>? QAccentItemTyped`

The edit channel carries the reading field's write, with the text typed.
Writing `QAccentItemText` raises it and changes nothing on the row.
The panel hands it to the gate and writes the answer back through `QAccentTypeRefine`.
A reading-view row has no listener, so nothing happens there.

## `public ImageSource? QAccentItemFlag`

The variety's flag, or null while none is known.
Looking it up again also announces the label, since the label yields to it.

## `public bool QAccentItemRespelled { get; }`

Whether the text is the row's respelling, fixed when the row is built.
A typed change then becomes a respelling request rather than a reading request.

## `public string QAccentItemOpener`

The bracket drawn before the text, fixed when the row is built.
A flip of the switch rebuilds every row, so a fixed bracket never goes stale.

## `public string QAccentItemCloser`

The bracket drawn after the text, fixed for the same reason.

## `public string QAccentItemText`

The reading the row holds, in the form the switch picks.
Writing it only raises `QAccentItemTyped`, and the row changes when the gate's answer comes back.

## `public string QAccentItemAudio`

The workspace file the row plays, blank while it has none.
Blank is what collapses the play button.
Only the draft's accent writes it, through `QAccentStateRefine`.

## `public bool QAccentItemPlayable`

Whether the row holds audio, so its play button shows.

## `internal static void QAccentItemRefine(FrameworkElement container, object item, string? _)`

Fills a row of `Theme.Accent.Row` or `Theme.Accent.Display` with flag, variety name, brackets, text and play button.
The editor row's text shows the placeholder in the muted colour while the row is blank.
It also gives the editor row's reading cell its `QFieldCell` order and tags its slot with `Theme.Accent.Slot`.
Those order strings live here, so the Veneer markup carries none.
It runs again on every change the row raises, so a typed pronunciation or a new flag shows at once.

## `internal static QAccentItem QAccentItemBuild(CAccent accent, bool flagged, CRespellingMark respelling)`

Builds the row for one accent that `CRespelling.CRespellingAccentRead` made ready.
The text is already the form `respelling` picks, so no driver resolves it.

## `internal static string QAccentLabelRefine(CVariety variety)`

A variety's label is the text under the key Conduct chose, and its raw name when no text exists.
The primary reading, the recorder and the transcription menu label a variety this way too.

## `internal static ImageSource? QAccentEnsignRefine(CVariety variety, bool flagged)`

Looks a variety's flag up in `QEnsignImage` under the key Conduct handed, only in flag mode.
The store may not hold it yet, and null then says so.

## `internal void QAccentFlagRefine(bool flagged)`

Looks the flag up again under the kept key after the store has been filled.

## `internal void QAccentStateRefine(CAccent spoken)`

Brings the row's text and audio up to the ready accent and announces only a real change.
It never raises `QAccentItemTyped`, so a draft render is never taken for an edit.

## `internal void QAccentTypeRefine(CAccentTyped typed)`

Writes the text the gate answered into the row and announces it.

## `private void QAccentValueRefine<QAccentValue>(ref QAccentValue field, QAccentValue value, params string[] names)`

Stores a changed value and announces each name, and stays silent when the value is unchanged.
Text compares ordinally, and a flag image by reference, since WPF seals image equality to reference.
