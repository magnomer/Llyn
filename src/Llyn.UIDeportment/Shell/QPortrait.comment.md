# QPortrait.cs

## `internal static class QPortrait`

The print and export shapes of every panel, turned from Conduct copies into the engine's own.
It holds no state and names no WPF type, so job44 sinks it into Conduct.

## `internal static LPortraitMedium QPortraitMediumRead(CPortraitMedium medium)`

The engine format a driver's export choice stands for, cast member for member.

## `internal static LPressTicket QPortraitTicketRead(CPressTicket ticket)`

The engine ticket a driver's print dialog answer stands for.
Side and ink cast member for member.

## `private static LPressPaper QPortraitPaperRead(double? width, double? height)`

The sheet the dialog named, or the local sheet when it named no usable size.
The local sheet is the engine's rule, so the driver only says it has no size.

## `internal static LPortraitLabel QPortraitLabelRead(CPortraitLabel label)`

The engine label a driver's localized words stand for, copied word for word.

## `internal static Func<CPortraitLabel, CPressTicket, Task> QPortraitPressCreate(`

The same wrap for a print that takes the labels too.

## `internal static Func<string, CPortraitMedium, CPortraitLabel, Task> QPortraitPortraitCreate(`

The same wrap for an export, turning the format and the labels into the engine's own.
