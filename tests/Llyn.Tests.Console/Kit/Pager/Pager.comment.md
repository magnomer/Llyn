# Pager

Tests of the paged text with links.

## `TPager`

Tab lights the next link and moves the top to keep it in view.
Enter follows a link and Backspace restores the page, the top and the lit link.
End puts the top at the last full view.
Later keys move it by a view or a line, clamped.
Escape is not taken, and Shift with Down is not taken either.
Showing the open page twice adds no trail, so one Backspace leaves the trail empty.

## Stamp

Commit: `2883be178842`
- `TPager.cs` `841883f6ca21f872`
