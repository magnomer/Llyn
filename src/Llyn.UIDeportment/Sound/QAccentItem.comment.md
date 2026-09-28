# QAccentItem.cs

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

## `public string QAccentItemLabel`

The variety's localized name, blank while a flag stands for it.
Blank is what collapses the label, so a row never shows both.

## `public ImageSource? QAccentItemFlag`

The variety's flag, or null while none is known.
Setting it also announces the label, since the label yields to it.

## `public bool QAccentItemRespelled { get; }`

Whether the text is the row's respelling, fixed when the row is built.
A typed change then becomes a respelling request rather than a reading request.

## `public string QAccentItemOpener`

The bracket drawn before the text, fixed when the row is built.
A flip of the switch rebuilds every row, so a fixed bracket never goes stale.

## `public string QAccentItemCloser`

The bracket drawn after the text, fixed for the same reason.

## `public string QAccentItemText`

The reading as typed or as the draft holds it, in the form the switch picks.
It announces only a real change, so a render writing the same text stays silent.

## `public string QAccentItemAudio`

The workspace file the row plays, blank while it has none.
Blank is what collapses the play button.

## `public bool QAccentItemPlayable`

Whether the row holds audio, so its play button shows.

## `internal static void QAccentItemRefine(FrameworkElement container, object item, string? _)`

Fills a row of `Theme.Accent.Row` or `Theme.Accent.Display`: flag, variety name, brackets, text and play button.
The editor row's text shows the placeholder in the muted colour while the row is blank.
It also gives the editor row's reading cell its `QFieldCell` order and tags its slot with `Theme.Accent.Slot`.
Those order strings live here, so the Veneer markup carries none.
It runs again on every change the row raises, so a typed pronunciation or a new flag shows at once.

## `internal static QAccentItem QAccentItemBuild(CAccent accent, bool flagged, CRespellingMark respelling)`

Builds the row for one accent that `CRespelling.CRespellingAccentRead` made ready.
The text is already the form `respelling` picks, so no driver resolves it.

## `internal static string QAccentLabelRefine(CVariety variety)`

A variety's label is the text under the key Conduct chose, and its raw name when no text exists.
An unnamed variety has no text under its key, so its label is empty.
The primary reading, the recorder and the transcription menu label a variety this way too.

## `internal static ImageSource? QAccentEnsignRefine(CVariety variety, bool flagged)`

Looks a variety's flag up in `LEnsignImage` under the key Conduct handed, only in flag mode.
The store may not hold it yet, and null then says so.

## `internal void QAccentFlagRefine(bool flagged)`

Looks the flag up again under the kept key after the store has been filled.
