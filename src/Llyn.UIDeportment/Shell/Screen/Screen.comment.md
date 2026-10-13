# Screen

The surface one Video plays on, shared by the form and the reading view.
A video row means the same thing in both modes, so one Screen answers for both.
It is split by player so a reader who wants one reads one file.
The Veneer's `PScreen` frame is realized by each video row template, and Deportment pulls its parts by contract ID.

## `QScreen`

Holds the surface itself and the choice between the two players.
One driver is built per realized frame and held weakly, so a dropped frame frees its entry.
The row's values reach the frame as attached properties, and the driver reads them back.
The frame is the Veneer's and the driver is no element, so every property is attached to the frame.
The players are written by logic, so they stay in Deportment beside it.
A row introduces the frame before setting any value, so even a row of defaults gets a wired switch.
The markup carries no event attribute, so every handler is wired in code.

### Choosing the player

A file on this machine goes to the machine's own player, which opens it directly and costs nothing.
Any other address goes to an embedded browser page.
A page address is not a film and no local player can resolve one.
A YouTube address plays through YouTube's embedded player, the only way its terms allow.
The row hands over the hosted film id the atelier read, beside the address.
A portrait sheet names films by the same engine rule.
It is the one place the players are chosen between.
Whichever player was standing is stopped first, because a Screen shows one film at a time.
The Screen remembers that a player was opened.
Later changes of address reach it, and an unopened Screen stays idle.
The running player decides who is told to play or pause.
That flag is kept here and not asked of either player, since only this file starts them.

### Nothing opens until play

A card lists every film of every meaning.
Opening each would start a player or a browser page for films nobody watches.
So the stage stands empty with its switch until the switch is pressed.
Only then is a player chosen and given the address.
The Screen owns the switch beneath it, because play is a thing the surface does.
A row that had to relay it would know how the surface works.
A Screen entering the tree opens only when its row already says it is playing.
That is the row returning after the card was taken down, not a card being read.

### Span

The span is enforced wherever the film plays.
A file is watched by a clock that sends it back to the start when it passes the end.
A page is given the span before it loads, so the site's own player never plays outside it.
An unknown span is no span at all, and the film plays whole.
Start and end are attached properties.

### Changes

The film id has no callback, because the row sets it before the address, whose change reloads the Screen.
Every change to what is played waits `QScreenDelay`, 500 ms, before anything is built.
The location field updates on each keystroke, and a half-typed address would open a browser page per letter.
A timer coalesces the changes and opens only the latest.
A Screen nobody has asked to play ignores the change, since there is no player to hand it to.
The playing property binds two ways by default, so the row's flag follows the switch.
The row's flag survives the Screen being taken down as the card scrolls.
User toggles go through the playing property and never bypass its callback.
The first ask for play opens the player, and every later ask is relayed to the one standing.
A row put back into the tree while marked playing opens on load, so the callback waits for the tree.
Volume changes apply at once to whichever player stands.
The page is sent a culture-independent command and not rebuilt, because reloading a film would lose its place.
A page still loading is skipped, since the level it will be built with is current anyway.
One slider answers for films and pronunciations, because both are the program speaking.
Unloading stops pending work and disposes browser resources.
A failed video shows a localized notice in place of the browser surface.

### Size

The Screen fills the width it is given and takes the height that keeps sixteen to nine.
A card is read at the window's width.
A film boxed to a corner would be smaller than the reader asked.
Only a width change is answered, because the answer is a height change and answering that would never end.

## `QScreenFile`

The machine's own player, used for a film that is a file on this machine.
A local file is opened directly and costs the program nothing to serve.
It reads the row's values from the `QScreen` that built it and raises that notice on failure.
The span clock is made where it is read, so the player owns its own parts.
Its tick looks at the film's position every 250 ms.
The framework's player cannot be told to stop at a moment, so the position is watched instead.
The film is sent back to the start once it passes the end the row carries.
An end at or before the start is no span at all and is ignored.
An opened film takes the level and the start, and waits paused unless the row is playing.
A player with no film is left alone, because there is nothing to run.
A film that ends starts again from the start of its span.
A film that will not open hides the player and shows the notice.
Silencing takes the film down and drops the file.
So a Screen shown again does not start on the last one.

## `QScreenBrowser`

The embedded browser a web film is played on.
The browser, its environment and the messages sent to the page live here together.
It reads the row values from the `QScreen` that built it and raises that notice on failure.
It builds nothing at construction, since a browser costs a process.
The browser is built on first play and dropped when the frame leaves the tree.
A page kept alive behind a closed card would hold its own process for nothing.
The page load is not waited on, because the surface must stay answerable while it runs.
The page is answered from memory and not written to a file.
It still needs a real address, `QScreenHost` on a host the program owns.
The embedded player refuses a page with no origin.

### Environment

The browser environment is the window's, made once and shared by every Screen under it.
It is found through the `Tag` of the loaded window above the Screen.
A Screen with no window above it is told so and not shown an empty box.
So is a machine without the browser runtime.
The window drops its environment on that failure, so a later attempt is a real attempt.
A page that finished loading after the row moved on is discarded.
The paper it was built for is compared, because fast typing starts more loads than finish.

### Composition control

The composition-hosted browser is used and not the ordinary one.
The ordinary control is a window of its own, drawn by the system over everything the program draws.
Such a player ignored its scroll region and covered the entry list, the command row and the title bar.
No draw order, opacity or arrangement reaches a separate window, so the fix was to stop it being one.
The composition control is clipped, layered and moved like every other element.
It builds its surface through the Windows projection.
So the shell is targeted at a Windows-versioned framework to carry it.
Lowering that target removes the projection, and the player then fails once a card holding one is drawn.

### Messages

Everything reaches the page through one send, whether play, pause or a level.
A browser not yet standing is skipped, since the page it will build carries the current state.
A message to a closed page is dropped, because no film is left to tell.
WebView2 reports that as an `InvalidOperationException` around a `COMException` with `QScreenClosed`.
Only that code marks a message with no one left to receive it.
A call from the wrong thread raises the same exception type around another inner one.
That is a programming fault and not a failure the user meets, so it propagates.
Blanking hides the page and navigates to an empty address, so a film cannot keep playing unseen.
The browser itself is kept, because building one costs far more than navigating one.

## `QScreenPaper`

The page an embedded browser is given, written for the address at hand.
A plain film address plays in the browser's own video element.
A YouTube address plays in the site's own player, which is driven by its API and not by an element.
So the two players are written separately.
The span, the level and whether to start are written into the page before it loads.
A page told afterwards would already have begun playing from the wrong place.
The film id is the one the row set on the Screen.
Without one, the video element plays the address.
The address is text the program did not choose and lands inside an attribute, so it is HTML-encoded.
The film id reaches a script literal, and the film check has already limited it to safe characters.
Every page shares one frame: black, unscrolled, and filled edge to edge by the film.

## Stamp

Commit: `c2f335c331fe`
- `QScreen.cs` `cdde399312a28b63`
- `QScreenBrowser.cs` `af60edbb3eadff2f`
- `QScreenFile.cs` `3465d9ec841d0d87`
- `QScreenPaper.cs` `19b30d60a425f0b3`
