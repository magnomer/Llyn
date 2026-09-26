# PScreen.cs

## `public partial class PScreen : UserControl`

The surface one Video plays on, used by the form and the reading view alike.
A video row means the same thing in both modes.
One Screen answers for both rather than two players drifting apart.

The frame is the Veneer's markup, loaded as the control's content.
The class stays the control that `PVideoTemplate` and `PDisplayVideo` place, so its dependency properties stay here.
The players are written by logic, so they stay in Deportment beside it.

A location is one of two different things, and the Screen decides which.
A file on this machine is handed to the machine's own player, which opens it directly and costs nothing.
A web address is handed to an embedded browser page.
A page address is not a film and no local player can resolve one.
A YouTube address is played through YouTube's own embedded player.
That is the only way the site allows and the only way that keeps its terms.

This file holds the surface itself and the choice between the two players.
Each player is written where it is used and nowhere else.
The machine's own player lives in [PScreenMedia.cs](PScreenMedia.comment.md).
The embedded browser lives in [PScreenBrowser.cs](PScreenBrowser.comment.md).
The page it is given lives in [PScreenPaper.cs](PScreenPaper.comment.md).
The address a YouTube film is read from lives in [PScreenFilm.cs](PScreenFilm.comment.md).
A reader who wants one of them reads one file rather than a player split across a long one.

The span is enforced wherever the film is playing.
A file is watched by a clock that sends it back to the start when it passes the end.
A page is given the span before it loads, so the site's own player never plays outside it.
An unknown span is no span at all, and the film plays whole.

The Screen fills the width it is given and takes the height that keeps a film's shape.
A card is read at the width of the window.
A film boxed to a corner of it is smaller than the reader asked for.

The Screen owns the switch beneath it rather than the row that holds the Screen.
Play is a thing the surface does.
A row that had to relay it would know how the surface works.

Nothing is opened until the user asks for play.
A card lists every film of every meaning.
Opening each would start a player or a browser page for films nobody watches.
The stage stands empty with its switch until the switch is pressed.
Only then is a player chosen and given the address.
Once a player stands, a changed address or span reaches it as before.

## `private const int PScreenDelay = 500`

A short delay lets rapid source changes settle before media resources are reopened.

## `private readonly DispatcherTimer _pScreenPending`

The timer coalesces source changes and delays opening until the control is ready.

## `private bool _pScreenWeb`

The flag selects browser commands or local media commands during playback synchronization.

## `private bool _pScreenOpened`

This tracks whether media has been opened so property changes can trigger a reload safely.

## `public PScreen()`

Loads the Veneer's `PScreen.xaml` by pack URI, sets it as content and copies its name scope.
The stage's size, the player's open, end and failure, and the switch's click are subscribed after the load.
The markup carries no event attribute, so every handler is wired here.
Deferred loading and lifetime cleanup are then tied to the control's visual lifecycle.

## `private Border PScreenStage`

The named parts of the frame are read through the copied name scope.
Call sites across the partial parts read them as they read generated fields before.

## `public static readonly DependencyProperty PScreenAddressProperty`

The address property selects a local file or remote page and reloads active media when changed.

## `public static readonly DependencyProperty PScreenFromProperty`

The start time lets a player begin at the relevant point in a media item.

## `public static readonly DependencyProperty PScreenUntilProperty`

The optional end time bounds playback to a selected media segment.

## `public static readonly DependencyProperty PScreenVolumeProperty`

The volume property updates both native media and embedded web playback.

## `public static readonly DependencyProperty PScreenPlayingProperty`

It binds two ways by default, so the video row's playing flag follows the screen's own switch.
A row's flag survives the screen being taken down as the card scrolls.

## `public Uri? PScreenAddress`

Where the film is read from, as the row already resolved it through the engine.
It is null while the location names no place.

## `public TimeSpan PScreenFrom`

Where play begins and where it returns to.

## `public TimeSpan? PScreenUntil`

Where play returns to the start, or `null` for a film played out.

## `public bool PScreenPlaying`

Whether the film should be running, held by the row.
It survives the Screen being taken down and put back.

## `public double PScreenVolume`

How loud to play, taken from the one slider the program keeps.
A film and a pronunciation are both the program speaking, so one slider answers for both.

## `private static void PScreenSourceHandle(DependencyObject holder, DependencyPropertyChangedEventArgs e)`

Every change to what is played waits half a second before anything is built.
The location field updates on each keystroke.
A half-typed address would otherwise open a browser page per letter.
A Screen nobody has asked to play ignores the change, since there is no player to hand it to.

## `private static void PScreenPlayingHandle(DependencyObject holder, DependencyPropertyChangedEventArgs e)`

The callback mirrors the playing state into the switch.
The first ask for play opens the player, and every later ask is relayed to the one standing.
A row put back into the tree while marked playing opens on load instead, so this waits for the tree.

## `private static void PScreenVolumeHandle(DependencyObject holder, DependencyPropertyChangedEventArgs e)`

Volume changes are applied immediately so both playback paths remain in sync.

## `private void PScreenVolumeApply()`

The level is given to whichever player is standing.
The page is sent a culture independent command rather than rebuilt.
Reloading a film to turn it down would lose where it was.
A page still loading is skipped, since the level it will be built with is the current one anyway.

## `private void PScreenSizeHandle(object sender, SizeChangedEventArgs e)`

The height follows the width at sixteen to nine.
Only a width change is answered, because the answer is a height change and answering that would not end.

## `private void PScreenOpenHandle(object sender, RoutedEventArgs e)`

A Screen entering the tree opens only when its row already says it is playing.
That is the row returning after the card was taken down, not a card being read.

## `private void PScreenPendingHandle(object? sender, EventArgs e)`

The timer callback opens only the latest pending source after earlier requests have settled.

## `private void PScreenSwitchHandle(object sender, RoutedEventArgs e)`

User toggles update the playing property rather than bypassing its callback.

## `private void PScreenDropHandle(object sender, RoutedEventArgs e)`

Unloading stops pending work and disposes browser resources to release the media surface.

## `private void PScreenShow()`

The one place the two players are chosen between.
A file address is the machine's own player and anything else is the browser page.
Whichever was standing is stopped first, because a Screen shows one film at a time.
The Screen remembers a player was opened.
Later changes of address reach it, and an unopened one stays idle.

## `private void PScreenSync()`

Which player is running decides who is told to play or pause.
The flag is kept here rather than asked of either player, since only this file starts them.

## `private void PScreenStop()`

Stopping clears visible failure state and halts both playback paths before a reload.

## `private void PScreenNoticeShow()`

A localized notice replaces the browser surface when remote video cannot be displayed.
