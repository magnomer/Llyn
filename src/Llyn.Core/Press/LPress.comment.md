# LPress.cs

## `public interface LPress`

The printing surface an export hands a rendered page to.
Printing needs a browser engine, which is a platform concern rather than a logic one.
Declaring the need here lets the engine print without knowing what draws.
The shell supplies the implementation at startup, as it does for any other platform port.

## `Task LPressSave(string html, string path)`

Prints `html` into a PDF file at `path`.

## `Task LPressPrint(string html, LPressTicket ticket)`

Prints `html` on the printer the ticket names.
It raises when the printer cannot be reached, so the shell can say so.
