# LPortraitPort.cs

## `public interface LPortraitPort`

The slice of the engine a panel sees when it prints, exports or imports.
Printing and exporting portray the record a vista chose, so the caller hands the vista and never the id.
The markup import runs as one call, which reads the cargo and stores it under declared intakes.
`LEngine` implements it today, and a portrait clerk takes it over when the parts are dismantled.

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
