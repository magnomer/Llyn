# PScreenPaper.cs

## `public partial class PScreen`

The page an embedded browser is given, written for the address at hand.
A plain film address is played by the browser's own video element.
A YouTube address is played by the site's own player, which is the only way its terms allow.

## `private string PScreenPaperBuild(Uri address)`

The span, the level and whether to start are written into the page before it loads.
A page told afterwards would already have begun playing from the wrong place.
The two players are written separately, because the site's player is driven by its API rather than by an element.

## `private static string PScreenPaperFormat(string body)`

The frame every page shares: black, unscrolled, and filled edge to edge by the film.

## `private static string PScreenTextFormat(string text)`

What a user typed is escaped before it is written into the page.
An address is text the program did not choose, and it lands inside both markup and script.
