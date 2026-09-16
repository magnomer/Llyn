# PScreen.xaml.cs

## `public partial class PScreen : UserControl`

The surface one Video plays on, used by the form and the reading view alike.
A video row means the same thing in both modes.
One Screen answers for both rather than two players drifting apart.

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

## `public string PScreenLocation { get; set; }`

Where the film is read from, taken as the user is still typing it.

## `public TimeSpan PScreenFrom { get; set; }`

Where play begins and where it returns to.

## `public TimeSpan? PScreenUntil { get; set; }`

Where play returns to the start, or `null` for a film played out.

## `public bool PScreenPlaying { get; set; }`

Whether the film should be running, held by the row.
It survives the Screen being taken down and put back.

## `public double PScreenVolume { get; set; }`

How loud to play, taken from the one slider the program keeps.
A film and a pronunciation are both the program speaking, so one slider answers for both.

## Inline notes

### `private static void PScreenSourceHandle(DependencyObject holder, DependencyPropertyChangedEventArgs e)`

Every change to what is played waits half a second before anything is built.
The location field updates on each keystroke, and a half-typed address would otherwise open a browser page per letter.
A Screen nobody has asked to play ignores the change, since there is no player to hand it to.

### `private static void PScreenPlayingHandle(DependencyObject holder, DependencyPropertyChangedEventArgs e)`

The first ask for play opens the player, and every later ask is relayed to the one standing.
A row put back into the tree while marked playing opens on load instead, so this waits for the tree.

### `private void PScreenOpenHandle(object sender, RoutedEventArgs e)`

A Screen entering the tree opens only when its row already says it is playing.
That is the row returning after the card was taken down, not a card being read.

### `private void PScreenVolumeApply()`

The level is given to whichever player is standing.
The page is told rather than rebuilt, because reloading a film to turn it down would lose where it was.
A page still loading is skipped, since the level it will be built with is the current one anyway.

### `private void PScreenSizeHandle(object sender, SizeChangedEventArgs e)`

The height follows the width at sixteen to nine.
Only a width change is answered, because the answer is a height change and answering that would not end.

### `private void PScreenShow()`

The one place the two players are chosen between.
A file address is the machine's own player and anything else is the browser page.
Whichever was standing is stopped first, because a Screen shows one film at a time.
The Screen remembers a player was opened, so later changes of address reach it and an unopened one stays idle.

### `private void PScreenSync()`

Which player is running decides who is told to play or pause.
The flag is kept here rather than asked of either player, since only this file starts them.
