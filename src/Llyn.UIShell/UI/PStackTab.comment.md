# PStackTab.cs

## `public partial class PEditor`

Which region of the editor is open.
The tab strip picks one of meaning, collocation, or note, and collapses the other two.
The same selection shape as the navigation strip, over the editor's own contents rather than over the window's panels.

### `private const string PStackSelected = "Selected";`

Selection lives in each button's tag rather than in a swapped style.
A style swap rebuilds the template, which would drop any running animation and lose the hover state under the cursor.
The tag also tells the template apart from a hover, so the selected tab never paints a hover fill over its own pill.

### `private void PStackPillPlace(bool glide)`

One pill sits behind the whole strip and moves to whichever tab is selected.
Giving each button its own indicator would make the marker blink between positions rather than travel.
The width follows the tab because the tabs size to their own text, which differs by label and by language.
A click glides, and a layout change places the pill outright, since nothing moved from the reader's view.
