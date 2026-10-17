# Pager

The console's paged text with links, kept as numbers so it knows no words.

## `BPager`

Pages of lines, where a line is either plain or a link to another page.
The pages are not copied in, so the pager asks the target function at each read.
It holds the page, the top line, the lit link and a trail of places to go back to.
The constructor throws for a null function, a negative page or a view that is not positive.
It does not check the page against the pages.
It declares its own key rows in the help focus, which help reads to list them.

## `BPagerObserve`

Returns true when the pager took the key.
Up, Down, PageUp, PageDown, Home and End move the top line, clamped so the view stays on the page.
Tab and Shift+Tab step the lit link around, and the top moves to keep that link in view.
Enter follows the lit link, and it is taken but does nothing on a page with no link.
Backspace returns to the last place on the trail, and does nothing when the trail is empty.
Every other key returns false, and a key with extra modifiers is not matched.

## `BPagerShow`

Goes to a page and pushes the place it left.
The new page starts at its top with the first link lit.
Showing the page already open changes nothing and adds nothing to the trail.
A page outside the pages throws.

## Stamp

Commit: `2883be178842`
- `BPager.cs` `e5f828e47d02f990`
