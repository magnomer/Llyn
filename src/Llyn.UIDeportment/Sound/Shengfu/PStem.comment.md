# PStem.cs

## `public class PStem : UserControl`

The series page of the xiesheng panel, the counterpart of the category page of the yunjing panel.
It decides nothing: the page is composed by the engine and this file writes the controls.

## `public PStem()`

Loads the page's markup from the Veneer and wears it as its content, with the markup's name scope.
It binds the entry command the character chips raise.

## `private TextBlock PStemHeadword`

Each named part of the markup is read through `FindName`, so call sites keep the old generated names.

## `internal Action<string?>? PStemEntryNotice`

Raised with the character a chip carries, so the panel can ask the shell for its entry.

## `internal void PStemAttach(LWindow window)`

Keeps the window deportment the fonts of a language are read from.

## `internal void PStemShow(LStemPage page)`

Writes one page onto the controls: its key, its language, its flag and its characters.
The blank page leaves the headword empty and shows the empty line.

## `private void PStemEntryHandle(object sender, ExecutedRoutedEventArgs e)`

Forwards the character of the chip that was pressed.
