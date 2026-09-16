# PScript.cs

## `public sealed class PScript : ContentControl`

The script box as a control, so the reading view and the editor draw the same thing.
It is handed the rows and whether a fetch runs, and shows or hides itself from those alone.
Folded, it carries a head with the box's name and a switch.
It then keeps its rows out of sight until the switch is on.
The editor folds it, since the pictures are reference beside the fields, and the reading view leaves it open.

## `public static readonly DependencyProperty PScriptItemsProperty`

The rows shown, one per character and style.

## `public static readonly DependencyProperty PScriptPendingProperty`

Whether a fetch runs for the entry.
That shows the loading line and keeps the box up while nothing is stored.

## `public static readonly DependencyProperty PScriptFoldedProperty`

Whether the box starts closed under a head with a switch.

## `private readonly Grid _pScriptHead = new();`

The head row: the box's name on the left, the switch on the right, present only when folded.

## `private readonly ToggleButton _pScriptSwitch = new();`

The switch opening the body, drawn like the marker switch with the expand chevron.

## `private readonly StackPanel _pScriptBody = new();`

The rows and the loading line, hidden while the box is folded and the switch is off.

## `private readonly ItemsControl _pScriptList = new();`

The list of rows, its own shared-size scope so the columns line up across rows.

## `private readonly TextBlock _pScriptLoading = new();`

The loading line under the rows.

## `protected override AutomationPeer OnCreateAutomationPeer()`

The surface peer, so thirty glyph pictures and their captions are not reported one by one to accessibility subscribers.

## `public PScript()`

Builds the box from the theme's script styles, unfocusable, collapsed until it has something to show.

## `internal IReadOnlyList<PScriptItem>? PScriptItems`

The rows shown, or `null` for none.

## `public bool PScriptPending`

Whether a fetch runs for the entry shown.

## `public bool PScriptFolded`

Whether the box starts closed under its head.

## `private static void PScriptStateHandle(DependencyObject sender, DependencyPropertyChangedEventArgs e)`

Redraws the box when any of its three properties change.

## `private void PScriptStateApply()`

Feeds the list and shows the loading line while pending.
The head shows only when folded, and the body only when open.
The box itself is visible when it has rows or a fetch runs, and collapsed otherwise.
