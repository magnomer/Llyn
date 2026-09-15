# PDiwei.xaml.cs

## `public partial class PDiwei : UserControl`

The category page as a control, handed a category and its grouped placements by the rime table panel.

## `private const string PDiweiKindInitial = "Yunjing.Shengmu";`

The localization key of the kind chip for an initial.

## `private const string PDiweiKindRime = "Yunjing.Yunmu";`

The localization key of the kind chip for a rime.

## `private LEngine _lEngine = null!;`

The engine, asked only for the language's fonts.

## `internal Action<string>? PDiweiEntryNotice { get; set; }`

What a click on a character chip runs, handed the character.

## `internal void PDiweiAttach(LEngine engine)`

Keeps the engine for the fonts.

## `internal void PDiweiApply(LDiwei diwei, IReadOnlyList<PDiweiItem> items)`

Draws the headword line for the category and lists the sections.
The headword takes the language's headword font and the list its glyph font.
An empty list shows the empty notice instead.

## `internal void PDiweiClear()`

Drops the sections, so a hidden page holds nothing.

## `private void PDiweiEntryHandle(object sender, ExecutedRoutedEventArgs e)`

Answers a character chip with the notice.
