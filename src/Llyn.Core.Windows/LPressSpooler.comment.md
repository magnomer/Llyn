# LPressSpooler.cs
Hash: `b4c79568a7583a71`

## `public sealed class LPressSpooler : LPressQueue`

Reads the installed printers and what each one can do, as plain Core records.
It is Core's Windows twin of `LPressQueue`, so no layer above sees System.Printing.
Only Host names it, and hands it to the rig as `LRigQueue`.

## `public Task<IReadOnlyList<LPressPrinter>> LPressQueueRead()`

Runs the whole read on one fresh background STA thread and answers when it ends.
System.Printing objects must stay on the thread that made them.
A dead network queue can block for seconds, so the read never runs on the caller's thread.
Both local queues and connected network queues are listed.
The default printer is marked by its full name, which is also the name the browser prints to.
A machine without a default printer still lists its printers, just with none marked.
A failed enumeration faults the answer with its own exception.
The print window reports it and still offers Save as PDF.
The continuation runs asynchronously, so the caller never resumes on the worker thread.

## `private static LPressPrinter LPressSpoolerScan(PrintQueue queue, bool marked)`

Builds one printer record from the queue's reported capabilities.
A queue that cannot report them is kept as unknown, with empty lists and no copy limit.
So a broken driver still offers its printer with the standard sheets and printer defaults.
Sheet sizes arrive in 1/96 inch and are handed on in inches, as the driver lists them.
Sheets without a size are dropped, since a missing size has no Core value.
Turning sheets upright, dropping repeats and naming standard sheets is a rule, so Application does it.
`LPortraitClerkPress.LPortraitPrinterRead` applies it to the whole list.
Monochrome counts as gray, since the browser offers only one non-colour mode.
Collation is offered only when the driver reports collated output.
