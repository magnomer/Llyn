# QFanqie.cs
Hash: `38b824ec14c71449`

## `public sealed class QFanqie : Decorator`

The fanqie box as a control, so the reading view and the editor draw the same thing.
It shows or hides itself from its blocks, a running fetch and a handed rebuild alone.
Folded, it carries a head with the box's name and a switch.
It then keeps its blocks out of sight until the remembered state is open.
The editor folds it, since the placements are reference beside the fields, and the reading view leaves it open.

## `public static readonly DependencyProperty QFanqieItemsProperty`

The blocks shown, one per character, book and source.

## `public static readonly DependencyProperty QFanqiePendingProperty`

Whether a fetch runs for the entry.
That shows the loading line and keeps the box up while nothing is stored.

## `public static readonly DependencyProperty QFanqieFoldedProperty`

Whether the box starts closed under a head with a switch.
A change sets the body first, collapsed when folded and visible when not, then redraws the rest.
So a folded box never flashes open before `QFanqieFoldRefine` paints the remembered state.
The switch is not touched there, so no flip reaches the gate.
It is unchecked from construction until that paint.

## `public static readonly DependencyProperty QFanqieRenewableProperty`

Whether the editor offers a rebuild.
That shows the regenerate button and keeps the box up while nothing is stored.

## `private readonly Grid _qFanqieHead = new();`

The head row holds the box's name on the left, then the regenerate button and the switch.
It shows only when the box is folded.

## `private readonly Button _qFanqieRefresh = new();`

The shared regenerate button in the head, shown only where the editor hands the box a rebuild.
It fetches the placements of the entry's characters again and files them into their categories afresh.
Its mark turns while a fetch runs, as the readings button's does.

## `private readonly ToggleButton _qFanqieSwitch = new();`

The switch opening the body, drawn like the marker switch with the expand chevron.
It shows the remembered open state and hands each click to the gate, never deciding itself.
The switch is heard on click only.

## `private readonly StackPanel _qFanqieBody = new();`

The blocks and the loading line, hidden while the box is folded and the remembered state is closed.
A folded box starts with it collapsed, set by the `QFanqieFolded` change, until the first paint.

## `private readonly ItemsControl _qFanqieList = new();`

The list of blocks, its own shared-size scope so the columns line up across blocks.

## `private readonly TextBlock _qFanqieLoading = new();`

The loading line under the blocks.

## `protected override AutomationPeer OnCreateAutomationPeer()`

The surface peer, so the many cells of the placement lines are not reported one by one to accessibility subscribers.
The rebuild button and the category buttons stay reachable through it.

## `public QFanqie()`

Builds the box from the theme's fanqie styles, collapsed until it has something to show.
Its list is attached to `QFanqieItem.QFanqieItemRefine`, which fills each block and its lines.

## `internal IReadOnlyList<QFanqieItem>? QFanqieItems`

The blocks shown, or `null` for none.

## `public bool QFanqiePending`

Whether a fetch runs for the entry shown.

## `internal bool QFanqieRenewable`

Whether the editor offers a rebuild, which is `false` in a reading view.
It is a property of the box, so handing one over redraws the head as the blocks do.

## `internal event Action? QFanqieRenewalNotice;`

What a click on the regenerate button runs, with no value.
The editor driver subscribes once to its sounding gate.

## `private void QFanqieRefreshObserve(object sender, RoutedEventArgs e)`

Hears the regenerate click and raises `QFanqieRenewalNotice`.

## `internal event Action<bool, string>? QFanqieDiweiNotice;`

What a click on an initial or a rime runs, handed the initial flag and the raw key.
Each view subscribes its own diwei gate's observer.

## `internal event Action<string?>? QFanqieStemNotice;`

What a click on a stem runs, handed the raw stem text.
Each view subscribes its own stem gate.

## `internal event Action<long, int, bool>? QFanqieRepresentativeNotice;`

What a click on a representative rank runs, handed the row id, the held rank and the Ctrl flag.
The fanqie clerk below the gate resolves the new rank.

## `private void QFanqieDiweiObserve(object sender, ExecutedRoutedEventArgs e)`

Hears the initial and rime commands of a line and hands the pressed part's raw key on.
The engine names the kind and ignores a blank key.

## `private void QFanqieStemObserve(object sender, ExecutedRoutedEventArgs e)`

Hears the stem command of a block and hands the raw stem text on.

## `private void QFanqieRepresentativeObserve(object sender, ExecutedRoutedEventArgs e)`

Hears the representative command of a line and hands its id, its held rank and the Ctrl state on.

## `public bool QFanqieFolded`

Whether this screen shows the box under a head with an open switch.
It is a layout choice of the screen, set in markup, not a remembered state.
A folded box starts closed until the remembered state is painted.
An unfolded box always shows its body.

## `internal event Action<bool>? QFanqieFoldNotice;`

What a click on the open switch runs, handed whether the switch is now on.
The editor driver subscribes its fold's toggle gate once.

## `private void QFanqieFoldObserve(object sender, RoutedEventArgs e)`

Hears a click on the switch and hands its checked state on through `QFanqieFoldNotice`.
The switch is heard on click only.

## `internal void QFanqieFoldRefine(bool opened)`

Paints the remembered open state the editor driver read from its fold.
It sets the switch and shows the body when open or when the box is not folded.
A paint never reaches the gate.

## `private void QFanqieStateRefine()`

Redraws the box when any of its four properties change.
Feeds the list and shows the loading line while pending.
The regenerate button stands wherever one was handed over, and it carries the `Pending` cue while a fetch runs.
The head shows only when folded.
The body is left to `QFanqieFoldRefine`, which paints it from the remembered state.
The box itself is visible when it has blocks, a fetch runs, or a rebuild was handed over.
