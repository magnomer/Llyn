# LEngineEstablishment.cs

## `public sealed partial class LEngine`

What the status bar asks the engine, answered in one call.
The bar holds no copy of any of it and asks again on every bulletin that could move a number.

## `public LEstablishment LEngineEstablishmentRead()`

Counts the held drafts that differ from their origin, the stored entries and the database bytes.
Only drafts this engine holds are counted.
A leftover from an earlier launch is not this window's unsaved work.
Each held draft is measured as the leave dialog measures it, so the bar and the dialog cannot disagree.
The size is the main database file alone.
The write-ahead log beside it grows and empties on its own schedule.
A number that jumps on a checkpoint reads as a fault.
A file that is gone answers zero rather than throwing, since the bar has nowhere to show a failure.
