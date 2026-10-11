# TPressWindowDelay.cs
Hash: `04061a6e817809cb`

## `public sealed class TPressWindowDelay`

Covers the galley's deferred redraw and its page window.
Each fact opens a session through `TInterfaceConductPortrait.TPressWindowOpen`, which pins the wait to `Timeout.Infinite`.
A fact sets the wait to zero through `TPressDelaySet` before the request it checks.
So nothing fires by the clock, and every event comes from the request under test.

A stale edit raises one state event carrying the drawn setting.
An edit left pending is replaced by a newer one, so one redraw renders the newer scale.
A failed deferred redraw shows `Print.RenderFailed` through the ledger and raises the failure state.

A sight raises one page event per page of its window, each a PNG with its page index.
A sight before any draw raises nothing and shows nothing, since no proof is held.
The window is the visible range and one page either side.
A repeated sight at the same resolution requests nothing, since those pages were delivered.
The same sight after a zoom past the default detail delivers its pages again and drops none.
A redraw forgets the delivered pages, so the same sight requests them again.
A sight that fails on its first page shows `Print.RenderFailed` once and delivers nothing.
After the galley is disposed, neither an edit nor a sight raises an event or reaches the board.

A sight also carries the page a reader is on, and a new page becomes the current page.
The same page again raises no setting, so a scroll within one page stays quiet.
A sight that moves the window raises a drop event for each delivered page it leaves.
A redraw raises a drop event for every delivered page, in page order.
