# QScreenPaper.cs

## `internal static class QScreenPaper`

The page an embedded browser is given, written for the address at hand.
A plain film address is played by the browser's own video element.
A YouTube address is played by the site's own player, which is the only way its terms allow.
It reads the row values from the `QScreen` whose browser asks for the page.

## `internal static string QScreenPaperBuild(QScreen screen, Uri address)`

The span, the level and whether to start are written into the page before it loads.
A page told afterwards would already have begun playing from the wrong place.
The film id is the one the row set on the Screen.
Without one, the browser's own video element plays the address.
The two players are written separately, because the site's player is driven by its API rather than by an element.
The address and the film id are HTML-encoded before they are written into the page.
An address is text the program did not choose, and it lands inside an attribute.
The film id reaches a script literal, and the film check has already limited it to safe characters.

## `private static string QScreenPaperFormat(string body)`

The frame every page shares: black, unscrolled, and filled edge to edge by the film.
