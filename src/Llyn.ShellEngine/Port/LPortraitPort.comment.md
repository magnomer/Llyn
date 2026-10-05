# LPortraitPort.cs
Hash: `967882659869ba12`

## `public interface LPortraitPort`

The slice of the engine a panel sees when it prints, exports or imports.
Printing and exporting portray the record a vista chose, so the caller hands the vista and never the id.
The Joplin push and attach each run as one call.
The markup import runs as one call, which reads the cargo and stores it under declared intakes.
`LPortraitOutlet` implements it today, and a portrait clerk takes it over when the parts are dismantled.

## `Task LEnginePortraitPrint(LVista? vista, LPortraitLabel label, LPressTicket ticket);`

Prints the entry the vista has chosen, and does nothing when the vista holds no entry.

## `Task LEnginePortraitPrint(LVista? vista, LPortraitLegend legend, LPressTicket ticket);`

Prints the Example, Situation or Source a catalog vista has chosen.
Any other subject is a caller mistake and throws.

## `Task LEnginePortraitExport(LVista? vista, string path, LPortraitMedium format, LPortraitLabel label);`

Exports the entry the vista has chosen to `path`, and does nothing when the vista holds no entry.

## `Task<IReadOnlyList<LMarkupOmission>?> LEngineMarkupStart(string path, Func< IReadOnlyList<LMarkupEntry>, IReadOnlyList<IReadOnlyList<LMarkupTarget>>, IReadOnlyList<LMarkupIntake>?> declare)`

Reads the markup file, asks `declare` for the intakes, then stores the cargo.
The read and the store run on a worker thread, while `declare` runs back on the caller's thread.
A `declare` answering null stores nothing and answers null.
Otherwise it answers what the read and the import left behind.

## `Task<LReceipt> LEngineCourierSend(LPortraitLabel label, CancellationToken cancellation);`

Pushes every entry into Joplin one way, worded with `label`.
The receipt counts what was saved, kept, removed and failed.

## `Task LEngineCourierAttach(CancellationToken cancellation);`

Asks Joplin to grant access and keeps the hidden token in the settings.
The caller never sees the raw token, so it cannot leak into a view.

## `static LPressTicket LEngineTicketRead(string printer, double? width, double? height, bool landscape, int copies, bool collated, LPressSide side, LPressInk ink)`

The engine ticket a print dialog's answer stands for, with the local sheet when no size was named.
The statics build the records a print or export takes from plain values, so Conduct builds no engine record.

## `static LPortraitLabel LEngineLabelRead(IReadOnlyList<string> words)`

The engine label an entry page is worded with, word by word in the record's order.

## `static IReadOnlyList<(LPortraitMedium, string, bool)> LEngineMediumRead()`

The formats an export offers, each with its file suffix and whether it is the default.

## `static IReadOnlyList<string> LEngineKindRead()`

The stored word of every Source kind, which a legend is worded by.

## `static LMarkupIntake LEngineIntakeRead(int index, LMarkupMode mode, long target)`

The engine intake one declared row stands for, built from plain values through the intake clerk.
A new entry carries no target, which the Core factory enforces.

## `static LPortraitLegend LEngineLegendRead(IReadOnlyList<string> words, IReadOnlyDictionary<string, string> kinds)`

The engine legend a catalog page is worded with, and each Source kind by its stored word.
