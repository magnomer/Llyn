# QStack.cs
Hash: `012671148ee4a618`

## `internal sealed class QStack`

Which region of the editor is open.
The tab strip picks one of meaning, collocation, etymology or note, and collapses the other three.
The same selection shape as the navigation strip, over the editor's own contents rather than over the window's panels.
It pulls every part from the editor's Veneer by contract ID.

### `private const QLookCue QStackSelected = QLookCue.QLookCueSelected;`

Selection lives in each button's cue rather than in a swapped style.
A style swap rebuilds the template, which would drop any running animation and lose the hover state under the cursor.
The cue also tells the template apart from a hover.
The selected tab never paints a hover fill over its own pill.

## `internal QStack(FrameworkElement surface)`

Holds the editor scope and subscribes the tab strip's load, size and clicks, which the markup named.
It first gives the meaning tab the selected cue and the others the idle cue, since the markup carries none.
The editor builds it with the window, so the strip's first load is always heard.
The strip needs no Conduct editor, so nothing waits for an introduce.

## `private FrameworkElement QEtymologyField`

The etymology region, the field the etymology driver fills, pulled here only to show or collapse it.

### `private void QStackPillPlace(bool glide)`

One pill sits behind the whole strip and moves to whichever tab is selected.
Giving each button its own indicator would make the marker blink between positions rather than travel.
The width follows the tab because the tabs size to their own text, which differs by label and by language.
A click glides, and a layout change places the pill outright, since nothing moved from the reader's view.

## `private void QStackPillRefine(object sender, RoutedEventArgs e)`

Answers both the strip's load and its size change, since each only places the pill outright.
