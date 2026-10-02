# QScreen.cs

## `internal sealed class QScreen`

The driver of the surface one Video plays on, used by the form and the reading view alike.
A video row means the same thing in both modes.
One Screen answers for both rather than two players drifting apart.

The frame is the Veneer's `PScreen`, which each video row template realizes on its own.
Deportment never names that type, so it pulls every part by contract ID.
One driver is built per realized frame and held weakly against it.
The row's values reach the frame as attached properties, which the driver reads back.
The players are written by logic, so they stay in Deportment beside it.

A location is one of two different things, and the Screen decides which.
A file on this machine is handed to the machine's own player, which opens it directly and costs nothing.
A web address is handed to an embedded browser page.
A page address is not a film and no local player can resolve one.
A YouTube address is played through YouTube's own embedded player.
That is the only way the site allows and the only way that keeps its terms.

This file holds the surface itself and the choice between the two players.
Each player is written where it is used and nowhere else.
The machine's own player lives in [QScreenFile.cs](QScreenFile.comment.md).
The embedded browser lives in [QScreenBrowser.cs](QScreenBrowser.comment.md).
The page it is given lives in [QScreenPaper.cs](QScreenPaper.comment.md).
The row hands the Screen the YouTube film id the atelier read, beside the address.
A portrait sheet names films by the same engine rule.
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

## `private const int QScreenDelay = 500`

A short delay lets rapid source changes settle before media resources are reopened.

## `internal static readonly DependencyProperty QScreenAddressProperty`

The address property selects a local file or remote page and reloads active media when changed.
Every property is attached to the frame, since the frame is the Veneer's and the driver is no element.

## `internal static readonly DependencyProperty QScreenFilmProperty`

The hosted film id of the address, or null, as the row read it through the atelier.
It has no callback, because the row sets it before the address, whose change reloads the Screen.

## `internal static readonly DependencyProperty QScreenFromProperty`

The start time lets a player begin at the relevant point in a media item.

## `internal static readonly DependencyProperty QScreenUntilProperty`

The optional end time bounds playback to a selected media segment.

## `internal static readonly DependencyProperty QScreenVolumeProperty`

The volume property updates both native media and embedded web playback.

## `internal static readonly DependencyProperty QScreenPlayingProperty`

It binds two ways by default, so the video row's playing flag follows the screen's own switch.
A row's flag survives the screen being taken down as the card scrolls.

## `private static readonly ConditionalWeakTable<DependencyObject, QScreen> QScreenHold = [];`

Each realized frame's driver, held weakly so a dropped frame frees its entry.

## `private readonly FrameworkElement _qScreenSurface`

The realized Veneer frame this driver answers for.

## `private readonly DispatcherTimer _qScreenPending`

The timer coalesces source changes and delays opening until the control is ready.

## `private readonly QScreenFile _qScreenFile`

The machine's own player of this frame.

## `private readonly QScreenBrowser _qScreenBrowser`

The embedded browser of this frame.

## `private bool _qScreenWeb`

The flag selects browser commands or local media commands during playback synchronization.

## `private bool _qScreenOpened`

This tracks whether media has been opened so property changes can trigger a reload safely.

## `private QScreen(FrameworkElement surface)`

Builds both players over the frame, then subscribes the stage's size and the switch's click.
Deferred loading and lifetime cleanup are then tied to the frame's visual lifecycle.
The markup carries no event attribute, so every handler is wired here.

## `internal string? QScreenFilm`

Which hosted film the page plays, or null for a film the browser's own element plays.

## `internal TimeSpan QScreenFrom`

Where play begins and where it returns to.

## `internal TimeSpan? QScreenUntil`

Where play returns to the start, or `null` for a film played out.

## `internal double QScreenVolume`

How loud to play, taken from the one slider the program keeps.
A film and a pronunciation are both the program speaking, so one slider answers for both.

## `internal bool QScreenPlaying`

Whether the film should be running, held by the row.
It survives the Screen being taken down and put back.

## `private Uri? QScreenAddress`

Where the film is read from, as the row already resolved it through the engine.
It is null while the location names no place.

## `private Border QScreenStage`

The named parts of the frame are pulled by contract ID on each read.

## `internal static void QScreenIntroduce(FrameworkElement surface)`

Builds the driver for a realized frame the first time a row fills it.
A later fill of the same frame finds its driver standing and builds nothing.
The row calls it before setting any value, so even a row of defaults gets a wired switch.

## `internal static QScreen? QScreenFind(DependencyObject node)`

The driver of this element, or null when the element is not a realized frame.

## `internal void QScreenNoticeRefine()`

A localized notice replaces the browser surface when a video cannot be displayed.

## `private static void QScreenSourceRefine(DependencyObject holder, DependencyPropertyChangedEventArgs e)`

Every change to what is played waits half a second before anything is built.
The location field updates on each keystroke.
A half-typed address would otherwise open a browser page per letter.
A Screen nobody has asked to play ignores the change, since there is no player to hand it to.

## `private static void QScreenPlayingRefine(DependencyObject holder, DependencyPropertyChangedEventArgs e)`

The callback mirrors the playing state into the switch.
The first ask for play opens the player, and every later ask is relayed to the one standing.
A row put back into the tree while marked playing opens on load instead, so this waits for the tree.

## `private static void QScreenVolumeRefine(DependencyObject holder, DependencyPropertyChangedEventArgs e)`

Volume changes are applied immediately so both playback paths remain in sync.
The level is given to whichever player is standing.
The page is sent a culture independent command rather than rebuilt.
Reloading a film to turn it down would lose where it was.
A page still loading is skipped, since the level it will be built with is the current one anyway.

## `private void QScreenSizeRefine(object sender, SizeChangedEventArgs e)`

The height follows the width at sixteen to nine.
Only a width change is answered, because the answer is a height change and answering that would not end.

## `private void QScreenOpenRefine(object sender, RoutedEventArgs e)`

A Screen entering the tree opens only when its row already says it is playing.
That is the row returning after the card was taken down, not a card being read.

## `private void QScreenPendingRefine(object? sender, EventArgs e)`

The timer callback opens only the latest pending source after earlier requests have settled.

## `private void QScreenSwitchRefine(object sender, RoutedEventArgs e)`

User toggles update the playing property rather than bypassing its callback.
The two way binding carries the change back to the row.

## `private void QScreenDropRefine(object sender, RoutedEventArgs e)`

Unloading stops pending work and disposes browser resources to release the media surface.

## `private void QScreenRefine()`

The one place the two players are chosen between.
A file address is the machine's own player and anything else is the browser page.
Whichever was standing is stopped first, because a Screen shows one film at a time.
The Screen remembers a player was opened.
Later changes of address reach it, and an unopened one stays idle.

## `private void QScreenSyncRefine()`

Which player is running decides who is told to play or pause.
The flag is kept here rather than asked of either player, since only this file starts them.

## `private void QScreenStopRefine()`

Stopping clears visible failure state and halts both playback paths before a reload.
