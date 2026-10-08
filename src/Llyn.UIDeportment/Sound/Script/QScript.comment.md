# QScript.cs
Hash: `748ac38e65c0ad35`

## `public sealed class QScript : Decorator`

The script box as a control, so the reading view and the editor draw the same thing.
It shows or hides itself from its rows, a running fetch and a handed rebuild alone.
Folded, it carries a head with the box's name and a switch.
It then keeps its rows out of sight until the remembered state is open.
The editor folds it, since the pictures are reference beside the fields, and the reading view leaves it open.

## `public static readonly DependencyProperty QScriptItemsProperty`

The rows shown, one per character and style.

## `public static readonly DependencyProperty QScriptPendingProperty`

Whether a fetch runs for the entry.
That shows the loading line and keeps the box up while nothing is stored.

## `public static readonly DependencyProperty QScriptFoldedProperty`

Whether the box starts closed under a head with a switch.
A change sets the body first, collapsed when folded and visible when not, then redraws the rest.
So a folded box never flashes open before `QScriptFoldRefine` paints the remembered state.
The switch is not touched there, so no flip reaches the gate.
It is unchecked from construction until that paint.

## `public static readonly DependencyProperty QScriptRenewableProperty`

Whether the editor offers a rebuild.
That shows the regenerate button and keeps the box up while nothing is stored.

## `private readonly Grid _qScriptHead = new();`

The head row holds the box's name on the left, then the regenerate button and the switch.
It is present only when folded.

## `private readonly ToggleButton _qScriptSwitch = new();`

The switch opening the body, drawn like the marker switch with the expand chevron.
It shows the remembered open state and hands each click to the gate, never deciding itself.
The switch is heard on click only.

## `private readonly StackPanel _qScriptBody = new();`

The rows and the loading line, hidden while the box is folded and the remembered state is closed.
A folded box starts with it collapsed, set by the `QScriptFolded` change, until the first paint.

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

Builds the box from the theme's script styles, collapsed until it has something to show.
Its list is attached to `QScriptItem.QScriptItemRefine`, which fills each row and its pictures.
It also listens to the localization catalog, because the age under a picture is the box's own text to redraw.
There are two boxes for the program's life, one per view, so the listening is never unhooked.

## `internal IReadOnlyList<QScriptItem>? QScriptItems`

The rows shown, or `null` for none.

## `public bool QScriptPending`

Whether a fetch runs for the entry shown.

## `public bool QScriptFolded`

Whether this screen shows the box under a head with an open switch.
It is a layout choice of the screen, set in markup, not a remembered state.
A folded box starts closed until the remembered state is painted.
An unfolded box always shows its body.

## `internal event Action<bool>? QScriptFoldNotice;`

What a click on the open switch runs, handed whether the switch is now on.
The editor driver subscribes its sounding's toggle gate once.

## `private void QScriptFoldObserve(object sender, RoutedEventArgs e)`

Hears a click on the switch and hands its checked state on through `QScriptFoldNotice`.
The switch is heard on click only.

## `internal void QScriptFoldRefine(bool opened)`

Paints the remembered open state the editor driver read from its sounding.
It sets the switch and shows the body when open or when the box is not folded.
A paint never reaches the gate.

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
The head shows only when folded.
The body is left to `QScriptFoldRefine`, which paints it from the remembered state.
The box is visible when it has rows, a fetch runs, or a rebuild was handed over, and collapsed otherwise.

## `internal event Action<Exception>? QScriptFailureNotice;`

What a picture that would not decode runs, handed its fault.
The editor and the reading view each subscribe once and show it through the ledger under `Display.ScriptFailed`.

## `internal void QScriptFailureRefine(Exception exception)`

Raises `QScriptFailureNotice` for a picture that would not decode.
The notice is posted to the dispatcher, because a scan or a decode runs inside a layout pass.
