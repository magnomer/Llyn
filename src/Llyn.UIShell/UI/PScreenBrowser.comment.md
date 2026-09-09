# PScreenBrowser.cs

## `public partial class PScreen`

The embedded browser a web film is played on.
A web address is not a file and the machine's own player cannot resolve one.
The browser, its environment, and the messages sent to the page live here together.

## `private void PScreenPageShow(Uri address)`

Builds the page for this address and puts the browser in front of the reader.
The load itself is not waited on, because the surface must stay answerable while it runs.

## `private async Task PScreenPageLoad()`

The browser environment is made once for the whole program and shared by every Screen.
It is started with autoplay allowed.
The user has already asked by the time play is called, and the page cannot see that.

A machine without the browser runtime is told so rather than shown an empty box.
The stored environment is dropped on that failure so a later attempt is a real attempt.

A page that finished loading after the row moved on is discarded.
The paper it was built for is compared, because typing quickly starts more loads than it finishes.

## `private WebView2CompositionControl PScreenBrowserCreate()`

The composition-hosted browser is used rather than the ordinary one.
The ordinary control is a window of its own, and a window of its own is drawn by the system over everything the program draws.
Such a player ignored the scroll region it sat in and covered the entry list, the command row and the title bar.
No draw order, opacity or arrangement reaches a separate window, so the fix could only be to stop it being one.
The composition control draws into the program's own surface and is therefore clipped, layered and moved like every other element.
It builds its surface through the Windows projection, so the shell is targeted at a Windows-versioned framework to carry it.
Lowering that target again removes the projection and the player then fails the moment a card holding one is drawn.

## `private void PScreenPageSend(string order)`

The one way anything reaches the page, whether that is play, pause or a level.
A browser not yet standing is silently skipped, since the page it will build carries the current state anyway.
A refused message is dropped, because a film that would not be told is not worth a failed program.

## `private void PScreenPageStop()`

Hides the page and sends the browser to an empty address, so a film cannot keep playing unseen.
The browser itself is kept, because building one costs far more than navigating one.

## `private void PScreenBrowserDispose()`

The browser is released when the Screen leaves the tree.
A page kept alive behind a closed card would hold its own process for nothing.

## Inline notes

### `private void PScreenPaperHandle(object? sender, CoreWebView2WebResourceRequestedEventArgs e)`

The page is answered from memory rather than written to a file.
It still needs a real address to be answered at.
The embedded player refuses to run for a page that has no origin.
