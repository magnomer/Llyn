# LPortraitClerkPress.cs
Hash: `92076f126dea5fac`

## `public sealed class LPortraitClerkPress`

A composed page sent out: saved in a format or printed through the press.
It composes no page itself, so the entry page and the catalog pages share it.

## `public LPortraitClerkPress(LRig rig)`

Reads the portrait port and the press out of `rig`.

## `public async Task LPortraitClerkExport(LPortraitPage portrait, string path, LPortraitMedium format)`

A PDF goes through the press as rendered sheet HTML, every other format through the portrait port.

## `public Task LPortraitClerkPrint(LPortraitPage page, LPressTicket ticket)`

The page rendered as sheet HTML and handed to the press with the ticket.

## `public static LPressTicket LPortraitTicketCreate(string printer, double? width, double? height, bool landscape, int copies, bool collated, LPressSide side, LPressInk ink)`

The ticket a print dialog's answer stands for.
The sheet size arrives in inches, since the driver that measured it converts its own unit.
A dialog that named no usable sheet size prints on the local sheet.
A width or height that is missing or not above zero is no usable size.

## `public static IReadOnlyList<(LPortraitMedium, string, bool)> LPortraitMediumRead()`

Every format an entry can be exported to, in the order a chooser offers them.
Each row carries the file suffix the format is written under, and whether it is the default.
HTML is the default.
