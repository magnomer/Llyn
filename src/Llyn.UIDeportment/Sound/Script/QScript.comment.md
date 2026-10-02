# QScript.cs

## `public sealed class QScript : Decorator`

The script box as a control, so the reading view and the editor draw the same thing.
It is handed the rows and whether a fetch runs, and shows or hides itself from those alone.
Folded, it carries a head with the box's name and a switch.
It then keeps its rows out of sight until the switch is on.
The editor folds it, since the pictures are reference beside the fields, and the reading view leaves it open.

## `public static readonly DependencyProperty QScriptItemsProperty`

The rows shown, one per character and style.

## `public static readonly DependencyProperty QScriptPendingProperty`

Whether a fetch runs for the entry.
That shows the loading line and keeps the box up while nothing is stored.

## `public static readonly DependencyProperty QScriptFoldedProperty`

Whether the box starts closed under a head with a switch.

## `public static readonly DependencyProperty QScriptRenewableProperty`

Whether the editor offers a rebuild.
That shows the regenerate button and keeps the box up while nothing is stored.

## `private readonly Grid _qScriptHead = new();`

The head row: the box's name on the left, then the regenerate button and the switch, present only when folded.

## `private readonly ToggleButton _qScriptSwitch = new();`

The switch opening the body, drawn like the marker switch with the expand chevron.

## `private readonly StackPanel _qScriptBody = new();`

The rows and the loading line, hidden while the box is folded and the switch is off.

## `private readonly ItemsControl _qScriptList = new();`

The list of rows, its own shared-size scope so the columns line up across rows.

## `private readonly TextBlock _qScriptLoading = new();`

The loading line under the rows.

## `private readonly Button _qScriptRefresh = new();`

The shared regenerate button in the head, shown only where the editor hands the box a rebuild.
It drops the stored pictures of the entry's characters and fetches them again from every style source.
Its mark turns while a fetch runs, as the rime and readings buttons' do.

## `protected override AutomationPeer OnCreateAutomationPeer()`

The surface peer, so thirty glyph pictures and their captions are not reported one by one to accessibility subscribers.

## `public QScript()`

Builds the box from the theme's script styles, unfocusable, collapsed until it has something to show.
Its list is attached to `QScriptItem.QScriptItemRefine`, which fills each row and its pictures.
It also listens to the localization catalog, because the age under a picture is the box's own text to redraw.
There are two boxes for the program's life, one per view, so the listening is never unhooked.

## `internal IReadOnlyList<QScriptItem>? QScriptItems`

The rows shown, or `null` for none.

## `public bool QScriptPending`

Whether a fetch runs for the entry shown.

## `public bool QScriptFolded`

Whether the box starts closed under its head.

## `internal bool QScriptRenewable`

Whether the editor offers a rebuild, which is `false` in a reading view.
It is a property of the box, so handing one over redraws the head as the rows do.

## `internal event Action? QScriptRenewalNotice;`

What a click on the regenerate button runs, with no value.
The editor driver subscribes once to its sounding gate.

## `private void QScriptRefreshObserve(object sender, RoutedEventArgs e)`

Hears the regenerate click and raises `QScriptRenewalNotice`.

## `private void QScriptLanguageRefine(object? sender, PropertyChangedEventArgs e)`

Tells every picture on show that its age reads differently, once the catalog has taken a new language.
Nothing is read from the engine and nothing is fetched, because only the printed words change.

## `private void QScriptStateRefine()`

Feeds the list and shows the loading line while pending.
The regenerate button stands wherever one was handed over, and it carries the `Pending` cue while a fetch runs.
The head shows only when folded, and the body only when open.
The box is visible when it has rows, a fetch runs, or a rebuild was handed over, and collapsed otherwise.

## `internal void QScriptRefine(IReadOnlyList<CScriptGroup> groups, bool pending)`

The seam the lectern's sound draws through, mapping the groups to rows and setting whether a fetch runs.
It only sets values, so the box redraws itself as for any other change.

## `internal void QScriptFailureRefine(Exception exception)`

Shows a picture that would not decode through the window's failure notice under `Display.ScriptFailed`.
The notice is posted to the dispatcher, because a scan or a decode runs inside a layout pass.
