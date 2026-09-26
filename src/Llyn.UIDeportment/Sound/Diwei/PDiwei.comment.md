# PDiwei.cs

## `public class PDiwei : UserControl`

The category page as a control, handed the composed page by the rime table panel.
Its list is attached to `PDiweiItem.PDiweiItemApply`, which fills each section and the lists inside it.

## `private LWindow _lWindow = null!;`

The window deportment, asked only for the language fonts.

## `public PDiwei()`

Loads the page's markup from the Veneer and wears it as its content, with the markup's name scope.
It binds the entry and switch commands the chips raise, then attaches the section fill.

## `private TextBlock PDiweiHeadword`

Each named part of the markup is read through `FindName`, so call sites keep the old generated names.

## `internal Action<string?>? PDiweiEntryNotice { get; set; }`

What a click on a character chip runs, handed the character.

## `internal Action<bool?>? PDiweiSwitchNotice { get; set; }`

What the IPA or respelling switch runs, handed true for respelling.

## `internal void PDiweiAttach(LWindow window)`

Keeps the window deportment for the fonts.

## `internal void PDiweiShow(LDiweiPage page, string kind)`

Draws the headword line for the page and copies its sections into rows.
The headword takes the language's headword font and the list its glyph font.
The kind chip reads the resource under the key the deportment names.
An empty page shows the empty notice instead.

## `private void PDiweiSwitchHandle(object sender, ExecutedRoutedEventArgs e)`

Answers a click on either half of any section's switch with the notice, the parameter saying which half.

## `private void PDiweiEntryHandle(object sender, ExecutedRoutedEventArgs e)`

Answers a character chip with the notice.
