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

## `private readonly Grid _pFanqieHead = new();`

The head row: the box's name on the left, the switch on the right, present only when folded.

## `private readonly Button _pFanqieRefresh = new();`

The regenerate button at the foot of the body, shown only where the editor hands the box a rebuild.
It fetches the placements of the entry's characters again and files them into their categories afresh.
Hidden while a fetch runs, so it is not pressed twice.

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

## `internal IReadOnlyList<PFanqieItem>? PFanqieItems`

The blocks shown, or `null` for none.

## `public bool PFanqiePending`

Whether a fetch runs for the entry shown.

## `internal Action? PFanqieRebuildNotice { get; set; }`

What the regenerate button runs, or `null` in a reading view, where the button is not shown.

## `internal Action<string, string>? PFanqieDiweiNotice { get; set; }`

What a click on an initial or a rime runs, handed the category kind and key.
It is `null` when no view listens.

## `private void PFanqieDiweiHandle(object sender, ExecutedRoutedEventArgs e)`

Answers the initial and rime commands of a line with the category the clicked part names.
The rime key is the rime without its 重紐 letter, as the diwei store keys it.
An empty part is ignored.

## `public bool PFanqieFolded`

Whether the box starts closed under its head.

## `private static void PFanqieStateHandle(DependencyObject sender, DependencyPropertyChangedEventArgs e)`

Redraws the box when any of its three properties change.

## `private void PFanqieStateApply()`

Feeds the list and shows the loading line while pending.
The head shows only when folded, and the body only when open.
The box itself is visible when it has blocks or a fetch runs, and collapsed otherwise.
