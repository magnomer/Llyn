# TCardHinge.cs
Hash: `c6d4de04feb2c3f8`

## `public sealed class TCardHinge`

Covers the writing card's fold hinge as the row painter and the click handler see it.
The reading card's cases live in `TLecternHinge`, which borrows the row builders here.
Each case runs on its own STA thread, since its rows and look resources are WPF objects.
The rows are bare templates that carry only the parts the painters touch, under their real names.
Each list holds the two fold keys the theme defines, so a folded header resolves them.
The click cases reach the real fold gates over a real workspace.
So the store is read back rather than a double asked.

## `internal static readonly CStateWording TCardHingeBlank`

The shared untitled wording is muted, allowing folded cards to show their peek.

## `internal static readonly CStateWording TCardHingeTitle`

The shared specified title keeps titled-card cases distinct from untitled ones.

## `internal static readonly CStateWording TCardHingeMeaning`

The shared definition provides the peek expected by both painting suites.

## `public void CardRowRefine_FoldedCard_CollapsesBodyChecksHingeAndClosesHeader()`

A folded writing card hides its body, checks its hinge and closes its header as the reading card does.

## `public void CardRowRefine_UnfoldedCard_ShowsBodyAndTitleBox()`

An unfolded writing card without a title still shows its title box and no peek.

## `public void CardRowRefine_FoldedCardWithBlankTitle_ShowsPeekBehindTheKeptTitleBox()`

A folded writing card without a title shows the peek and keeps its title box shown and focusable.
So a writer can type a title without unfolding the card first.

## `public void CardRowRefine_FoldedCardWithTitle_ShowsTitleBoxAndHidesPeek()`

A folded writing card with a title keeps its title box and shows no peek.

## `public void CardRowRefine_UnstoredCard_CollapsesHinge()`

A writing card that is not stored hides its hinge and keeps its body.

## `public void CardHingeClick_StoredCard_FoldsTheCardByIdThroughTheEditorList()`

A hinge click on a writing card hands the card's id and the checked state to the editor's list gate.
The store then holds the fold, and a second click clears it.

## `public void CardHingeClick_RefusedWrite_PutsTheHingeBack()`

A hinge click on a writing card while no stored entry is held is refused by the list gate.
The hinge returns to unchecked, since no refresh will repaint it.

## `public void CardHingeLook_UnfoldedCard_TurnsTheExpandIconHalfway()`

An unfolded hinge draws the expand icon itself, in its own colours, with no mask.
The icon is turned half a turn, which the app reads as open and able to close.

## `public void CardHingeLook_FoldedCard_LeavesTheExpandIconUnturned()`

A folded hinge keeps the same expand icon and leaves it unturned.
So it reads as closed and able to open, like the other fold controls.

## `private static ContentPresenter TCardHingeRefine(CStateWording title, bool folded, bool stored)`

Lays out one writing card row and paints it with the real card row painter.

## `internal static FrameworkElement TCardHingeResolve(ContentPresenter container, string name)`

Finds a row's part by its template name, as the painters do.

## `internal static FrameworkElementFactory TCardHingeBuild(FrameworkElementFactory crown)`

Builds a card template with a header, its crown, the peek, the hinge and a body.
The header starts with the open corners and bottom line the theme gives it.

## `internal static FrameworkElementFactory TCardHingeCreate()`

Builds a row that holds only a hinge inside a panel.
The hinge is a plain `ToggleButton`, as in the real templates, so a driver's type test finds it.
A template root is not found by name, so the hinge stands one level down.

## `internal static ItemsControl TCardHingeLoad(FrameworkElementFactory card)`

Builds a bare list over the given row template, holding the theme's two fold keys.

## `internal static void TCardHingeSettle(FrameworkElement surface)`

Lays the surface out, so its rows exist.

## `internal static ToggleButton TCardHingeFind(ItemsControl list)`

Finds the hinge in the list's first row.

## `internal static void TCardHingeToggle(ToggleButton hinge, bool folded)`

Sets the hinge's state and raises its click, as a press does.

## `internal static LEntry TCardHingePrepare(LEngine engine)`

Saves an entry with one stored meaning card.

## `private static void TCardLookPrepare()`

Gives the application the resources the look sheet reads when it first loads.
It points the icon root at the icons the test assembly carries.
The two contract animations are blank stand-ins, since the hinge cases never play them.

## `internal static void TCardHingeRun(Action body)`

Runs the body on an STA thread and fails the test with any exception it threw.
