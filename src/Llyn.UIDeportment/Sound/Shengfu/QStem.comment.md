# QStem.cs
Hash: `96461e7115c0da6d`

## `internal sealed class QStem`

The driver of the series page of the xiesheng panel, the counterpart of the category page of the yunjing panel.
It decides nothing.
The engine composes the page and this file writes the controls.
`QXiesheng` builds it over the nested page, as `QPhonology` builds `QArticulation`.

## `internal QStem(UserControl surface)`

Takes the veneer page as its surface and binds the entry command the character chips raise.

## `private TextBlock QStemHeadword`

Each named part of the page is pulled through `QContract.QContractFind`.

## `internal void QStemIntroduce(CXiesheng xiesheng)`

Keeps the panel session a chip asks.

## `internal void QStemRefine(CStemPage page)`

Writes one page onto the controls: its fonts, its key, its language, its flag and its characters.
The headword and glyph fonts arrive ready on the page, as the diwei page carries its own.
The blank page leaves the headword empty and shows the empty line.

## `private void QStemEntryObserve(object sender, ExecutedRoutedEventArgs e)`

Hands the character of the pressed chip to the panel gate, which asks the shell for its entry.
