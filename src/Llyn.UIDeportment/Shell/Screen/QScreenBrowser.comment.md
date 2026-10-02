# QScreenBrowser.cs

## `internal sealed class QScreenBrowser`

The embedded browser a web film is played on.
A web address is not a file and the machine's own player cannot resolve one.
The browser, its environment, and the messages sent to the page live here together.
It reads the row values from the `QScreen` that built it, and raises that notice on failure.

## `private const string QScreenHost = "https://llyn.video/screen"`

The address the page is answered at, on a host the program owns.

## `private const int QScreenClosed = unchecked((int)0x8007139F)`

The invalid-state code WebView2 answers once its page is closed.
Only this code marks a message with no one left to receive it.
A call from the wrong thread raises the same exception type around another inner one.
That is a programming fault, not a failure the user meets, so it propagates.

## `private readonly QScreen _qScreenBrowserDriver`

The driver whose values the page is built from.

## `private readonly FrameworkElement _qScreenBrowserSurface`

The realized frame whose page slot holds the browser and whose window shares the environment.

## `private bool _qScreenReady`

Whether the browser's settings and page answer are wired, which happens once per browser.

## `private WebView2CompositionControl? _qScreenBrowser`

The browser, built on first play and dropped when the frame leaves the tree.

## `private string _qScreenPaper = string.Empty`

The page last built, answered from memory and compared to drop a stale load.

## `internal QScreenBrowser(QScreen driver, FrameworkElement surface)`

Keeps the driver and the frame and builds nothing, since a browser costs a process.

## `private Grid QScreenPage`

The frame's empty slot the browser is put into, pulled by contract ID on each read.

## `internal void QScreenPageRefine(Uri address)`

Builds the page for this address and puts the browser in front of the reader.
The load itself is not waited on, because the surface must stay answerable while it runs.

## `private async Task QScreenLoadRefine()`

The browser environment is the window's, made once and shared by every Screen under it.
The host is found through the `Tag` of the loaded window above the Screen.
A Screen with no window above it is told so rather than shown an empty box.
So is a machine without the browser runtime.
The window drops its environment on that failure so a later attempt is a real attempt.

A page that finished loading after the row moved on is discarded.
The paper it was built for is compared, because typing quickly starts more loads than it finishes.

## `private WebView2CompositionControl QScreenBrowserCreate()`

The composition-hosted browser is used rather than the ordinary one.
The ordinary control is a window of its own.
A window of its own is drawn by the system over everything the program draws.
Such a player ignored the scroll region it sat in.
It covered the entry list, the command row and the title bar.
No draw order, opacity or arrangement reaches a separate window.
The fix could only be to stop it being one.
The composition control draws into the program's own surface and is therefore clipped, layered and moved like every other element.
It builds its surface through the Windows projection, so the shell is targeted at a Windows-versioned framework to carry it.
Lowering that target again removes the projection and the player then fails the moment a card holding one is drawn.

## `internal void QScreenPageSend(string order)`

The one way anything reaches the page, whether that is play, pause or a level.
A browser not yet standing is silently skipped, since the page it will build carries the current state anyway.
A message to a page already closed is dropped, because no film is left to tell.
The WebView2 wrapper reports that case as an `InvalidOperationException` around the closed state's `COMException`.
The only other refusal is a call from the wrong thread, a programming fault left to propagate.

## `internal void QScreenBlankRefine()`

Hides the page and sends the browser to an empty address, so a film cannot keep playing unseen.
The browser itself is kept, because building one costs far more than navigating one.

## `internal void QScreenBrowserDispose()`

The browser is released when the Screen leaves the tree.
A page kept alive behind a closed card would hold its own process for nothing.

## Inline notes

### `private void QScreenPaperRefine(object? sender, CoreWebView2WebResourceRequestedEventArgs e)`

The page is answered from memory rather than written to a file.
It still needs a real address to be answered at.
The embedded player refuses to run for a page that has no origin.
