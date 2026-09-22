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

## `public static readonly DependencyProperty PScriptRenewalProperty`

What the regenerate button runs, or nothing where no rebuild is offered.

## `private readonly Grid _pScriptHead = new();`

The head row: the box's name on the left, then the regenerate button and the switch, present only when folded.

## `private readonly ToggleButton _pScriptSwitch = new();`

The switch opening the body, drawn like the marker switch with the expand chevron.

## `private readonly StackPanel _pScriptBody = new();`

The rows and the loading line, hidden while the box is folded and the switch is off.

## `private readonly ItemsControl _pScriptList = new();`

The list of rows, its own shared-size scope so the columns line up across rows.

## `private readonly TextBlock _pScriptLoading = new();`

The loading line under the rows.

## `private readonly Button _pScriptRefresh = new();`

The shared regenerate button in the head, shown only where the editor hands the box a rebuild.
It drops the stored pictures of the entry's characters and fetches them again from every style source.
Its mark turns while a fetch runs, as the rime and readings buttons' do.

## `protected override AutomationPeer OnCreateAutomationPeer()`

The surface peer, so thirty glyph pictures and their captions are not reported one by one to accessibility subscribers.

## `public PScript()`

Builds the box from the theme's script styles, unfocusable, collapsed until it has something to show.
It also listens to the localization catalog, because the age under a picture is the box's own text to redraw.
There are two boxes for the program's life, one per view, so the listening is never unhooked.

## `internal IReadOnlyList<PScriptItem>? PScriptItems`

The rows shown, or `null` for none.

## `public bool PScriptPending`

Whether a fetch runs for the entry shown.

## `public bool PScriptFolded`

Whether the box starts closed under its head.

## `internal Action? PScriptRenewal`

What the regenerate button runs, or `null` in a reading view, where the button is not shown.
It is a property of the box, so handing one over redraws the head as the rows do.

## `private static void PScriptStateHandle(DependencyObject sender, DependencyPropertyChangedEventArgs e)`

Redraws the box when any of its four properties change.

## `private void PScriptLanguageHandle(object? sender, PropertyChangedEventArgs e)`

Tells every picture on show that its age reads differently, once the catalog has taken a new language.
Nothing is read from the engine and nothing is fetched, because only the printed words change.

## `private void PScriptStateApply()`

Feeds the list and shows the loading line while pending.
The regenerate button stands wherever one was handed over, and its tag turns its mark while a fetch runs.
The head shows only when folded, and the body only when open.
The box is visible when it has rows, a fetch runs, or a rebuild was handed over, and collapsed otherwise.
