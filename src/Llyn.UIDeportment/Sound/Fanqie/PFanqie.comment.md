# PFanqie.cs

## `public sealed class PFanqie : ContentControl`

The fanqie box as a control, so the reading view and the editor draw the same thing.
It is handed the blocks and whether a fetch runs, and shows or hides itself from those alone.
Folded, it carries a head with the box's name and a switch.
It then keeps its blocks out of sight until the switch is on.
The editor folds it, since the placements are reference beside the fields, and the reading view leaves it open.

## `public static readonly DependencyProperty PFanqieItemsProperty`

The blocks shown, one per character, book and source.

## `public static readonly DependencyProperty PFanqiePendingProperty`

Whether a fetch runs for the entry.
That shows the loading line and keeps the box up while nothing is stored.

## `public static readonly DependencyProperty PFanqieFoldedProperty`

Whether the box starts closed under a head with a switch.

## `public static readonly DependencyProperty PFanqieRenewalProperty`

What the regenerate button runs, or nothing where no rebuild is offered.

## `private readonly Grid _pFanqieHead = new();`

The head row: the box's name on the left, then the regenerate button and the switch, present only when folded.

## `private readonly Button _pFanqieRefresh = new();`

The shared regenerate button in the head, shown only where the editor hands the box a rebuild.
It fetches the placements of the entry's characters again and files them into their categories afresh.
Its mark turns while a fetch runs, as the readings button's does.

## `private readonly ToggleButton _pFanqieSwitch = new();`

The switch opening the body, drawn like the marker switch with the expand chevron.

## `private readonly StackPanel _pFanqieBody = new();`

The blocks and the loading line, hidden while the box is folded and the switch is off.

## `private readonly ItemsControl _pFanqieList = new();`

The list of blocks, its own shared-size scope so the columns line up across blocks.

## `private readonly TextBlock _pFanqieLoading = new();`

The loading line under the blocks.

## `protected override AutomationPeer OnCreateAutomationPeer()`

The surface peer, so the many cells of the placement lines are not reported one by one to accessibility subscribers.
The rebuild button and the category buttons stay reachable through it.

## `public PFanqie()`

Builds the box from the theme's fanqie styles, unfocusable, collapsed until it has something to show.
Its list is attached to `PFanqieItem.PFanqieItemRefine`, which fills each block and its lines.

## `internal IReadOnlyList<PFanqieItem>? PFanqieItems`

The blocks shown, or `null` for none.

## `public bool PFanqiePending`

Whether a fetch runs for the entry shown.

## `internal Action? PFanqieRenewal`

What the regenerate button runs, or `null` in a reading view, where the button is not shown.
It is a property of the box, so handing one over redraws the head as the blocks do.

## `internal event Action<bool, string>? PFanqieDiweiNotice;`

What a click on an initial or a rime runs, handed the initial flag and the raw key.
Each view subscribes its own diwei gate's observer.

## `internal event Action<string?>? PFanqieStemNotice;`

What a click on a stem runs, handed the raw stem text.
Each view subscribes its own stem gate.

## `internal event Action<long, int, bool>? PFanqieRepresentativeNotice;`

What a click on a representative rank runs, handed the row id, the held rank and the Ctrl flag.
The fanqie clerk below the gate resolves the new rank.

## `private void PFanqieDiweiObserve(object sender, ExecutedRoutedEventArgs e)`

Hears the initial and rime commands of a line and hands the pressed part's raw key on.
The engine names the kind and ignores a blank key.

## `private void PFanqieStemObserve(object sender, ExecutedRoutedEventArgs e)`

Hears the stem command of a block and hands the raw stem text on.

## `private void PFanqieRepresentativeObserve(object sender, ExecutedRoutedEventArgs e)`

Hears the representative command of a line and hands its id, its held rank and the Ctrl state on.

## `public bool PFanqieFolded`

Whether the box starts closed under its head.

## `private void PFanqieStateRefine()`

Redraws the box when any of its four properties change or its switch flips.
Feeds the list and shows the loading line while pending.
The regenerate button stands wherever one was handed over, and it carries the `Pending` cue while a fetch runs.
The head shows only when folded, and the body only when open.
The box itself is visible when it has blocks or a fetch runs, and collapsed otherwise.

## `internal void PFanqieRefine(IReadOnlyList<CFanqieGroup> groups, bool pending)`

The seam the lectern's sound draws through, mapping the groups to blocks and setting whether a fetch runs.
It only sets values, so the box redraws itself as for any other change.
