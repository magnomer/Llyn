# PScreen.xaml.cs

## `public partial class PScreen : UserControl`

This control coordinates local video playback and web media while exposing playback state to its owner.

## `private const int PScreenDelay = 500`

A short delay lets rapid source changes settle before media resources are reopened.

## `private readonly DispatcherTimer _pScreenPending`

The timer coalesces source changes and delays opening until the control is ready.

## `private bool _pScreenWeb`

The flag selects browser commands or local media commands during playback synchronization.

## `private bool _pScreenOpened`

This tracks whether media has been opened so property changes can trigger a reload safely.

## `public PScreen()`

Initialization wires deferred loading and lifetime cleanup to the control's visual lifecycle.

## `public static readonly DependencyProperty PScreenAddressProperty`

The address property selects a local file or remote page and reloads active media when changed.

## `public static readonly DependencyProperty PScreenFromProperty`

The start time lets a player begin at the relevant point in a media item.

## `public static readonly DependencyProperty PScreenUntilProperty`

The optional end time bounds playback to a selected media segment.

## `public static readonly DependencyProperty PScreenVolumeProperty`

The volume property updates both native media and embedded web playback.

## `public static readonly DependencyProperty PScreenPlayingProperty`

Two way binding keeps external playback controls synchronized with the screen switch.

## `public Uri? PScreenAddress`

The selected address is exposed for binding by the control's owner.

## `public TimeSpan PScreenFrom`

The start offset is exposed so callers can select a useful excerpt.

## `public TimeSpan? PScreenUntil`

The optional end offset lets callers bound playback without requiring a full duration.

## `public bool PScreenPlaying`

The playback state can be bound both into and out of the control.

## `public double PScreenVolume`

The normalized volume value is exposed for caller binding and media updates.

## `private static void PScreenSourceHandle(DependencyObject holder, DependencyPropertyChangedEventArgs e)`

Active or opened media is reloaded after its address or excerpt boundaries change.

## `private static void PScreenPlayingHandle(DependencyObject holder, DependencyPropertyChangedEventArgs e)`

The callback mirrors bound playback state into the switch and starts unopened media when needed.

## `private static void PScreenVolumeHandle(DependencyObject holder, DependencyPropertyChangedEventArgs e)`

Volume changes are applied immediately so both playback paths remain in sync.

## `private void PScreenVolumeApply()`

Applying volume updates local playback and sends a culture independent command to the web player.

## `private void PScreenSizeHandle(object sender, SizeChangedEventArgs e)`

The stage follows a sixteen to nine ratio so video remains framed as the control resizes.

## `private void PScreenOpenHandle(object sender, RoutedEventArgs e)`

Loaded controls defer opening until an assigned playing source can be shown.

## `private void PScreenPendingHandle(object? sender, EventArgs e)`

The timer callback opens only the latest pending source after earlier requests have settled.

## `private void PScreenSwitchHandle(object sender, RoutedEventArgs e)`

User toggles update the bindable playback property rather than bypassing its callback.

## `private void PScreenDropHandle(object sender, RoutedEventArgs e)`

Unloading stops pending work and disposes browser resources to release the media surface.

## `private void PScreenShow()`

Opening chooses the local player or browser based on the address scheme.

## `private void PScreenSync()`

Playback changes are translated into commands understood by the currently active player.

## `private void PScreenStop()`

Stopping clears visible failure state and halts both playback paths before a reload.

## `private void PScreenNoticeShow()`

A localized notice replaces the browser surface when remote video cannot be displayed.
