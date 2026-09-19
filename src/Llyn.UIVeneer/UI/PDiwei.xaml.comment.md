# PDiwei.xaml.cs

## `public partial class PDiwei : UserControl`

The category page as a control, handed the composed page by the rime table panel.

## `private LEngine _lEngine = null!;`

The engine, asked only for the language's fonts.

## `internal Action<string?>? PDiweiEntryNotice { get; set; }`

What a click on a character chip runs, handed the character.

## `internal Action<bool?>? PDiweiSwitchNotice { get; set; }`

What the IPA or respelling switch runs, handed true for respelling.

## `internal void PDiweiAttach(LEngine engine)`

Keeps the engine for the fonts.

## `internal void PDiweiShow(LDiweiPage page, string kind)`

Draws the headword line for the page and copies its sections into rows.
The headword takes the language's headword font and the list its glyph font.
The kind chip reads the resource under the key the deportment names.
An empty page shows the empty notice instead.

## `private void PDiweiSwitchHandle(object sender, ExecutedRoutedEventArgs e)`

Answers a click on either half of any section's switch with the notice, the parameter saying which half.

## `private void PDiweiEntryHandle(object sender, ExecutedRoutedEventArgs e)`

Answers a character chip with the notice.
