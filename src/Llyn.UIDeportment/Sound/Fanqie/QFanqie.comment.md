# QFanqie.cs
Hash: `afbea8e8cc33b7bf`

## `public sealed class QFanqie : Decorator`

Editor and reading views share the same Fanqie presentation control.
Its layout fold flag is separate from the entry's stored opening.
The caller owns persistence and paints opening through `QFanqieFoldRefine`.

## `public static readonly DependencyProperty QFanqieItemsProperty`

Ready blocks determine whether the control has content to show.

## `public static readonly DependencyProperty QFanqiePendingProperty`

A pending fetch keeps the control visible even without blocks.

## `public static readonly DependencyProperty QFanqieFoldedProperty`

Changing layout folding resets body visibility before the remaining state is repainted.
A folded layout waits for an explicit opening paint rather than flashing its body first.
That callback does not change the switch's checked state.

## `public static readonly DependencyProperty QFanqieRenewableProperty`

Renewability keeps an empty editor box available for rebuilding.

## `private readonly Grid _qFanqieHead = new();`

Only folded layouts show the heading and its controls.

## `private readonly Button _qFanqieRefresh = new();`

Rebuilding leaves through a notice, keeping fetching outside the control.
Its visibility and pending cue follow supplied verdicts.

## `private readonly ToggleButton _qFanqieSwitch = new();`

The editor and reading view drivers each hear clicks on the exposed switch.
Programmatic checked-state paints do not request persistence.

## `private readonly StackPanel _qFanqieBody = new();`

The body groups content and loading presentation under one opening verdict.

## `private readonly ItemsControl _qFanqieList = new();`

A shared-size scope keeps block columns aligned within this box.

## `private readonly TextBlock _qFanqieLoading = new();`

The loading line follows pending state independently of block count.

## `protected override AutomationPeer OnCreateAutomationPeer()`

The surface peer exposes focusable descendants instead of every placement cell.

## `public QFanqie()`

Theme resources supply the control's presentation, while row fills consume ready Fanqie items.
The control starts collapsed until content, pending work or renewability makes it visible.

## `internal IReadOnlyList<QFanqieItem>? QFanqieItems`

Null and an empty list both represent absent blocks.

## `public bool QFanqiePending`

Pending state describes the supplied fetch verdict, not work started by this control.

## `internal bool QFanqieRenewable`

The supplied rebuild verdict controls availability without passing a Conduct object into the panel.

## `internal event Action? QFanqieRenewalNotice;`

Rebuild clicks carry no row value, leaving the caller to select the sounding gate.

## `private void QFanqieRefreshObserve(object sender, RoutedEventArgs e)`

A rebuild click raises only the renewal notice.

## `internal event Action<bool, string>? QFanqieDiweiNotice;`

Category clicks retain their initial verdict and raw key for the caller's navigation gate.

## `internal event Action<string?>? QFanqieStemNotice;`

Stem text remains raw, so the receiving view owns its interpretation.

## `internal event Action<long, int, bool>? QFanqieRepresentativeNotice;`

Representative clicks carry identity, held rank and Ctrl state without selecting a replacement rank here.

## `private void QFanqieDiweiObserve(object sender, ExecutedRoutedEventArgs e)`

Only a Fanqie line supplies category identity.
The command determines whether the initial or rime key is forwarded.

## `private void QFanqieStemObserve(object sender, ExecutedRoutedEventArgs e)`

Stem commands cross the notice boundary without resolving their text.

## `private void QFanqieRepresentativeObserve(object sender, ExecutedRoutedEventArgs e)`

Only a Fanqie line can supply representative identity and rank.
Ctrl is read at execution time and forwarded unchanged.

## `public bool QFanqieFolded`

This flag selects headed layout, not persistent opening state.
An unfolded layout keeps the body visible.

## `internal ToggleButton QFanqieSwitch`

Exposing the switch lets each view's driver own clicks and refused-click recovery.
The panel holds no Conduct dependency.

## `internal void QFanqieFoldRefine(bool opened)`

An opening paint updates the switch and body together without calling a gate.
Unfolded layouts remain visible regardless of the supplied opening.

## `private void QFanqieStateRefine()`

Content, pending work or renewability keeps the control visible.
Ordinary state changes do not overwrite the stored-opening paint.
Only the folded-property callback separately resets body visibility.
