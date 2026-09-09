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

The span is enforced wherever the film is playing.
A file is watched by a clock that sends it back to the start when it passes the end.
A page is given the span before it loads, so the site's own player never plays outside it.
An unreadable span is no span at all, and the film plays whole.

The Screen fills the width it is given and takes the height that keeps a film's shape.
A card is read at the width of the window.
A film boxed to a corner of it is smaller than the reader asked for.

The Screen owns the switch beneath it rather than the row that holds the Screen.
Play is a thing the surface does.
A row that had to relay it would know how the surface works.

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

### `private void PScreenVolumeApply()`

The level is given to whichever player is standing.
The page is told rather than rebuilt, because reloading a film to turn it down would lose where it was.
A page still loading is skipped, since the level it will be built with is the current one anyway.

### `private void PScreenSizeHandle(object sender, SizeChangedEventArgs e)`

The height follows the width at sixteen to nine.
Only a width change is answered, because the answer is a height change and answering that would not end.

### `private async Task PScreenPageShow()`

The browser environment is made once for the whole program and shared by every Screen.
It is started with autoplay allowed.
The user has already asked by the time play is called, and the page cannot see that.

A machine without the browser runtime is told so rather than shown an empty box.
The stored environment is dropped on that failure so a later attempt is a real attempt.

A page that finished loading after the row moved on is discarded.
The paper it was built for is compared, because typing quickly starts more loads than it finishes.

### `private WebView2CompositionControl PScreenBrowserCreate()`

The composition-hosted browser is used rather than the ordinary one.
The ordinary control is a window of its own, and a window of its own is drawn by the system over everything the program draws.
Such a player ignored the scroll region it sat in and covered the entry list, the command row and the title bar.
No draw order, opacity or arrangement reaches a separate window, so the fix could only be to stop it being one.
The composition control draws into the program's own surface and is therefore clipped, layered and moved like every other element.
It builds its surface through the Windows projection, so the shell is targeted at a Windows-versioned framework to carry it.
Lowering that target again removes the projection and the player then fails the moment a card holding one is drawn.

### `private void PScreenPaperHandle(object? sender, CoreWebView2WebResourceRequestedEventArgs e)`

The page is answered from memory rather than written to a file.
It still needs a real address to be answered at.
The embedded player refuses to run for a page that has no origin.

### `internal static string? PScreenFilmRead(Uri address)`

The film id inside a YouTube address, or `null` when the address is not one.
Every shape that site hands out is read, since a user pastes whichever one they were given.
An id holding anything but letters, digits, dashes and underscores is refused, because it is written straight into the page.
