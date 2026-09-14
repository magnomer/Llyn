# LSheetPlate.cs

## `public static class LSheetPlate`

Renders picture and video plates, whether a card or a page section holds them.

## `public static void LSheetPlateAppend(StringBuilder page, LPortraitCard card)`

The card's own plates, handed on to the list form below.

## `public static void LSheetPlateAppend(StringBuilder page, IReadOnlyList<LPortraitMedia> images, IReadOnlyList<LPortraitMedia> videos)`

A local picture is embedded, so the page keeps its images after it is moved or mailed.
A picture that cannot be found takes a plain plate, so the layout keeps its place without a broken mark.
A video cannot play in a document, so it becomes a poster frame over a link.
A hosted video lays the host's own thumbnail over the plain plate, which shows through if the host is unreachable.

## `private static string? LSheetAddressRead(string location)`

The address an image tag can carry, or null when there is none worth writing.
Only a readable local file qualifies, and it becomes a data address the page carries within itself.
A web address is dropped.
The page would otherwise fetch from a host the user's entry named, wherever it is opened.
A local path that does not read is dropped too.
From a page written elsewhere it would point at nothing.
