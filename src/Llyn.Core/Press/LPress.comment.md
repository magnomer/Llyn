# LPress.cs
Hash: `334e2ac6b6f36510`

## `public interface LPress`

The printing surface an export hands a rendered page to.
Printing needs a browser engine, which is a platform concern rather than a logic one.
Declaring the need here lets the engine print without knowing what draws.
The host supplies the implementation at startup, as it does for any other platform port.
The printer list is a separate port, `LPressQueue`, since it needs no browser.

## `Task LPressSave(string html, string path)`

Prints `html` into a PDF file at `path`.

## `Task<LPressBoard> LPressBoardOpen(string html)`

Loads `html` into a board kept open for one print session.
The caller owns the board and disposes it when the session ends.
It raises when the page cannot be opened, so the shell can say so.
It is called on the thread that will use the board, because the browser engine is bound to it.
