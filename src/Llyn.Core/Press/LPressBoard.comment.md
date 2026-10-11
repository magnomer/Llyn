# LPressBoard.cs
Hash: `0729118eba7b3453`

## `public interface LPressBoard : IAsyncDisposable`

One sheet held loaded in a browser for the life of a print session.
Loading the sheet once spares every later preview the navigation and the wait for pictures and fonts.
The engine keeps the board and hands each call a setting, so the board itself remembers no choice.
Disposing it closes the browser, so a closed session leaves nothing open behind it.

## `Task<LPressProof> LPressBoardDraw(LPressSetting setting);`

Lays the whole sheet out under `setting` and answers the resulting pages.
The page range is never applied here, so page numbers stay stable while the range changes.
Only one draw or print may run at a time, because the browser refuses a second.

## `Task<LPressStatus> LPressBoardPrint(LPressSetting setting);`

Sends the sheet to the printer `setting` names, with every choice it holds.
An offline or unknown printer answers a status rather than an exception, so the session can say so.

## `Task LPressBoardExport(LPressSetting setting, string path);`

Writes the sheet under `setting` into a PDF file at `path`.
It raises when the file cannot be written, so the caller can report it.

## `Task LPressBoardShow();`

Opens the system print dialog on the same loaded sheet.
It keeps driver-only features such as trays and stapling within reach.
