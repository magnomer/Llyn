# Press

This folder holds the print ports and the records of one print session.
Printing needs a browser engine and a spooler, which are platform concerns, so Core declares only the need.
The host supplies the implementations at startup, as it does for any other platform port.
All sizes are in inches, the unit the browser engine prints in.

## `LPress`

The printing surface an export hands a rendered page to.
Declaring the need here lets the engine print without knowing what draws.
The printer list is a separate port, `LPressQueue`, since it needs no browser.

## `LPressSave`

Prints the html into a PDF file at the path.

## `LPressBoardOpen`

Loads the html into a board kept open for one print session.
The caller owns the board and disposes it when the session ends.
It raises when the page cannot be opened, so the shell can say so.
It is called on the thread that will use the board, because the browser engine is bound to it.

## `LPressBoard`

One sheet held loaded in a browser for the life of a print session.
Loading the sheet once spares every later preview the navigation and the wait for pictures and fonts.
The engine keeps the board and hands each call a setting, so the board itself remembers no choice.
Disposing it closes the browser, so a closed session leaves nothing open behind it.

## `LPressBoardDraw`

Lays the whole sheet out under the setting and answers the resulting pages.
The page range is never applied here, so page numbers stay stable while the range changes.
Only one draw or print may run at a time, because the browser refuses a second.

## `LPressBoardPrint`

Sends the sheet to the printer the setting names, with every choice it holds.
An offline or unknown printer answers a status rather than an exception, so the session can say so.

## `LPressBoardExport`

Writes the sheet under the setting into a PDF file.
It raises when the file cannot be written, so the caller can report it.

## `LPressBoardShow`

Opens the system print dialog on the same loaded sheet.
It keeps driver-only features such as trays and stapling within reach.

## `LPressProof`

The pages of one layout, kept so each page can be drawn when it scrolls into view.
A new setting that changes layout yields a new proof.
The old one is disposed once no page draw still reads it.
Pages are counted from zero here, as a list of drawn pages is.
The count is known without drawing any page.

## `LPressProofRead`

The size of a page as laid out, in inches.
The canvas shapes its sheets from the first page's size rather than from the setting.
So rounding never makes a sheet and its page differ.

## `LPressProofDraw`

Draws a page as a picture at a resolution in dots per inch of its own paper.
The resolution is a document unit, so no screen geometry reaches Core.
The picture's pixel size lives in the PNG itself.

## `LPressImage`

One drawn page, carried as encoded PNG bytes so no drawing framework crosses into the logic.
The pixel size is not repeated beside the bytes.
A view lays a page out from the proof's paper size and reads pixels only from the PNG.

## `LPressQueue`

The printer list a print window offers beside Save as PDF.
Listing printers asks the operating system's spooler.
It is its own port, apart from `LPress`, because listing printers needs no browser.

## `LPressQueueRead`

Lists the installed printers with what each reported it can do.
A printer that reports nothing is still listed, marked unknown.
With no printer installed the list is empty, and saving as PDF remains.
A spooler that cannot be read raises, so the caller can tell the user why no printer shows.

## `LPressPrinter`

One installed printer and what it reported it can do.
A printer that is offline or has a broken driver may report nothing.
It is still listed, marked unknown, so the reader can choose it and print with its defaults.
Empty paper, side and colour lists mean unknown.
The copies limit is null when the printer states none.

## `LPressStatus`

How a print request ended.
An unavailable printer is common and expected, so it is an answer rather than an exception.
Done means the job was handed to the printer.
Offline means the printer was unknown or unavailable.
Error means the job failed for another reason.

## `LPressVault`

The port for the print setting kept under the workspace root, so the next print window opens with it.
`LPressFile` in Infrastructure is its adapter over the keep file and the JSON the setting is written in.
Core sees only the `LPressSetting` record and never the text.

## `LPressVaultRead`

The kept setting, or nothing while none was ever written.
Text that is not a setting also reads as nothing, so the window always opens.
A file that exists but cannot be read raises `LVaultFault`.

## `LPressVaultSave`

Replaces the kept setting whole, exactly as given.
The caller decides what is worth keeping, such as an emptied page range.
A file that cannot be written raises `LVaultFault`.

## Stamp

Commit: `c2f335c331fe`
- `LPress.cs` `334e2ac6b6f36510`
- `LPressBoard.cs` `0729118eba7b3453`
- `LPressImage.cs` `3d69582d14ebd9f9`
- `LPressPrinter.cs` `17b4f1555fe5e279`
- `LPressProof.cs` `d0f82653948a3c31`
- `LPressQueue.cs` `88309db3989d2040`
- `LPressStatus.cs` `8d7ae2d160666638`
- `LPressVault.cs` `ebd676ab2466657b`
