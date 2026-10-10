# QScript.cs
Hash: `82428afc8c003204`

## `public sealed class QScript : Decorator`

Editor and reading views share the same Script presentation control.
Its layout fold flag is separate from the entry's stored opening.
The caller owns persistence and paints opening through `QScriptFoldRefine`.

## `public static readonly DependencyProperty QScriptItemsProperty`

Ready rows determine whether the control has content to show.

## `public static readonly DependencyProperty QScriptPendingProperty`

A pending fetch keeps the control visible even without rows.

## `public static readonly DependencyProperty QScriptFoldedProperty`

Changing layout folding resets body visibility before the remaining state is repainted.
A folded layout waits for an explicit opening paint rather than flashing its body first.
That callback does not change the switch's checked state.

## `public static readonly DependencyProperty QScriptRenewableProperty`

Renewability keeps an empty editor box available for rebuilding.

## `private readonly Grid _qScriptHead = new();`

Only folded layouts show the heading and its controls.

## `private readonly ToggleButton _qScriptSwitch = new();`

The editor and reading view drivers each hear clicks on the exposed switch.
Programmatic checked-state paints do not request persistence.

## `private readonly StackPanel _qScriptBody = new();`

The body groups content and loading presentation under one opening verdict.

## `private readonly ItemsControl _qScriptList = new();`

A shared-size scope keeps row columns aligned within this box.

## `private readonly TextBlock _qScriptLoading = new();`

The loading line follows pending state independently of row count.

## `private readonly Button _qScriptRefresh = new();`

Rebuilding leaves through a notice, keeping fetching outside the control.
Its visibility and pending cue follow supplied verdicts.

## `protected override AutomationPeer OnCreateAutomationPeer()`

The surface peer exposes focusable descendants for accessibility.

## `public QScript()`

Theme resources supply presentation, while row fills consume ready Script items.
The control starts collapsed until content, pending work or renewability makes it visible.
Catalog notifications refresh picture labels without fetching again.
This class provides no localization unsubscription.

## `internal IReadOnlyList<QScriptItem>? QScriptItems`

Null and an empty list both represent absent rows.

## `public bool QScriptPending`

Pending state describes the supplied fetch verdict, not work started by this control.

## `public bool QScriptFolded`

This flag selects headed layout, not persistent opening state.
An unfolded layout keeps the body visible.

## `internal ToggleButton QScriptSwitch`

Exposing the switch lets each view's driver own clicks and refused-click recovery.
The panel holds no Conduct dependency.

## `internal void QScriptFoldRefine(bool opened)`

An opening paint updates the switch and body together without calling a gate.
Unfolded layouts remain visible regardless of the supplied opening.

## `internal bool QScriptRenewable`

The supplied rebuild verdict controls availability without passing a Conduct object into the panel.

## `internal event Action? QScriptRenewalNotice;`

Rebuild clicks carry no row value, leaving the caller to select the sounding gate.

## `private void QScriptRefreshObserve(object sender, RoutedEventArgs e)`

A rebuild click raises only the renewal notice.

## `private void QScriptLanguageRefine(object? sender, PropertyChangedEventArgs e)`

Every catalog notification refreshes supplied picture labels without filtering the property name.
No engine read or fetch is needed because only presentation wording changes.

## `private void QScriptStateRefine()`

Content, pending work or renewability keeps the control visible.
Ordinary state changes do not overwrite the stored-opening paint.
Only the folded-property callback separately resets body visibility.

## `internal event Action<Exception>? QScriptFailureNotice;`

Picture decode failures leave through a notice, keeping failure presentation in the owning view.

## `internal void QScriptFailureRefine(Exception exception)`

Failure delivery is deferred through the dispatcher rather than raised synchronously inside a scan or layout pass.
