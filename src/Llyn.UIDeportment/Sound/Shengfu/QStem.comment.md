# QStem.cs

## `internal sealed class QStem`

The driver of the series page of the xiesheng panel, the counterpart of the category page of the yunjing panel.
It decides nothing: the page is composed by the engine and this file writes the controls.
`QXiesheng` builds it over the nested page, as `QPhonology` builds `QArticulation`.

## `internal QStem(UserControl surface)`

Takes the veneer page as its surface and binds the entry command the character chips raise.

## `private TextBlock QStemHeadword`

Each named part of the page is pulled through `QContract.QContractFind`.

## `internal void QStemAttach(LWindow window, LXiesheng xiesheng)`

Keeps the window deportment the fonts of a language are read from, and the panel deportment a chip asks.

## `internal void QStemShow(CStemPage page)`

Writes one page onto the controls: its key, its language, its flag and its characters.
The blank page leaves the headword empty and shows the empty line.

## `private void QStemEntryHandle(object sender, ExecutedRoutedEventArgs e)`

Hands the character of the pressed chip to the panel deportment, which asks the shell for its entry.
