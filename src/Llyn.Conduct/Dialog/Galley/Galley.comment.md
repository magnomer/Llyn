# Galley

This folder holds the print preview over one galley.
It carries the draws, waits, page pictures, zoom, proof state and the page range the galley holds.

## Print enums and records

- `CSPressRange` maps member for member to `LPressScope`, which the galley holds.
- `CSPressPage` is one change to a page of the proof, a drawn picture or a drop.
  A single record for both keeps one handler as the only writer of the driver's page cache.
  It carries no size, since the canvas sizes each sheet from the paper in `CSPressState`.
  A dropped page frees its picture, so a later visit asks for it again.
- `CSPressState` is the outcome of one proof draw, from which the canvas lays out its placeholder sheets.
  The count and size are zero after a failure.
  The canvas sizes sheets at 96 to the inch, so it computes no unit itself.
  A fitting lens refits to the new paper, so the driver paints the zoom with the sheets.

## `CSGalley`

The proof draw gates over one galley, the preview the canvas shows.
`CSPress` builds it over the same session and hands it out as `CSPressGalley`.
The setting each draw answers comes from `CSPress`, which owns the page and the running flag.
A failure goes through the ledger as `Print.RenderFailed`.
Continuations resume on the caller's context, since the galley is bound to the thread that opened it.
So every event is raised on that thread, and the driver marshals nothing for it.
Engine order stays below, where the newest draw wins and a replaced proof's page answers null.
The proof relay is owned and disposed by `CPortrait.LPortraitPrint`.

It holds three roles apart, each in a type that owns its own state.
- Two `CSGalleyDelay` values hold the waits a person causes by typing or scrolling.
- One `CSGalleyWindow` holds the sight window and its delivered pages, replaced whole.
- One `CSLens` holds the zoom, learns each proof's paper and answers the resolution a sight needs.

`CSGalleyStateChanged` is raised when a deferred redraw finishes with a proof or a failure.
A superseded redraw raises nothing, since a newer one is on its way.
The direct gate `CSGalleyDraw` raises nothing, since its caller already holds the answer.
`CSGalleyPageChanged` is raised once per delivered page with its picture.
It is raised with a null picture for each delivered page the galley stops holding, in page order.
A page drops when a sight's window leaves it or a finished draw starts a new proof.
A page asked again at another resolution is not dropped, since its new picture replaces the old.
So a driver's picture cache has this one event as its only writer.
`LSGalleyWindowSet` is the window's one writer, so every page it stops holding is announced as dropped.
After close it only stores the window, so no event fires.

The edit delay is 250 milliseconds in production, and the sight delay is 150.
Tests set both through `CSPress.CSPressGalley`.

`CSGalleyDraw` draws a proof through one relay call, and the latest request wins.
It is the one awaitable draw, which the retry button and the deferred redraw both call.
It answers null when a newer draw superseded this one, so the driver drops that answer.
A failed draw shows `Print.RenderFailed` and answers that key with no pages.
That answer is the window's retry veil, not a second notice, and its setting is not stale.
Either answer carries the setting read after the draw, since the page count changes the printed pages.
Either answer starts a new window with the next turn and the new page count.
Either answer carries the lens after it learned the proof's paper, zero after a failure.
Every page delivered from the old proof is announced as dropped before the answer returns.

`CSGalleySightSet` takes the raw sight of one scroll: the page, the visible pages and the monitor scale.
The lens turns the scale and its zoom into a resolution, null while the default suffices.
So no pixel width crosses the cut.
It first hands the page to `CSPress.LSPressPageSelect`, so the current page has one owner.
Then it moves the window before any wait, announcing the pages it leaves as dropped.
The window is the visible range and one page either side, clipped to the proof.
A negative first page means nothing is visible, so the window stays and nothing is requested.
After the sight wait it requests each page of the window not yet held at that resolution.
An answer is delivered only while the window still wants it, so no page lands outside the window.
A newer sight stops the older one before its next request.
The first failed page ends the pass and shows `Print.RenderFailed` once.
A superseded page answers null and stays silent.
The task never faults.

`LSGalleyProofDefer` is called only by `CSPress.LSPressSettingRaise`.
A stale setting restarts the redraw wait, so quick edits end in one redraw of the newest setting.
A fresh setting leaves a pending wait alone, so a redraw already due still runs.
`LSGalleyProofDraw` ends quietly when its wait is cancelled by a newer edit or the close.
`Dispose` cancels both waits, so an answer still in flight raises nothing.

## `CSGalleyDelay`

One wait a person causes, by typing or scrolling, before the galley acts.
The wait is interaction with no medium, so it lives in Conduct.
A newer start cancels the older wait, so only the last request in a burst goes on.
The time is read when a wait starts, so a change applies to the next start.
Tests set zero, which runs the work at once, and `Timeout.Infinite` keeps a wait pending until cancelled.
`LSGalleyDelayStart` answers null when a newer start or `Dispose` cancelled the wait.
The token stays live after the wait, so a long pass can stop once a newer start cancels it.

## `CSGalleyLeaf` and `CSGalleyWindow`

`CSGalleyLeaf` is one delivered page picture keyed by page and resolution.
Its value equality lets a set find a picture by its two values.

`CSGalleyWindow` is immutable and replaced whole, so its state has one writer.
The galley compares old and new windows to announce dropped pages.
- The turn counts finished draws, and a page answer begun under an older turn is dropped.
- The count is zero before any draw and after a failed one, so a sight then requests nothing.
- The high page is -1 before any valid sight on the current proof.
- Held pictures are at most one per page and never outside the window.

`LSGalleyWindowMove` clips the visible range plus one page either side and keeps only held pictures inside the bounds.
A held picture at another resolution stays until its replacement arrives, so the sheet never blanks.
`LSGalleyWindowCheck` tells whether a page is in bounds, at the window's resolution, and not yet held.
The galley asks it before a request and again before it delivers.
`LSGalleyWindowAdd` replaces any other picture of the same page.

## `CSLens`

The print preview's zoom and its rules.
Typed or wheeled zoom input is raw, so the driver hands it here and paints the answer.
Nothing about zoom goes below Conduct, so its gates call no engine member.
`CSGalley` builds one per galley and feeds it each proof, so the zoom lives as long as its galley.
The factor and fit mode are one immutable grade, replaced only through `LSLensFactorSet`.
The view area and the proof's paper size with its default resolution are two more immutable records.
The window opens in width fit at a factor of 1.

`CSLensLadder` lists the zoom percents the step gate walks.
Its first and last rungs are also the clamp every factor obeys.
`CSLensTextSet` drops the culture's percent sign and reads a whole number in the current culture.
A number sets a free factor clamped to the ladder.
Text that does not read refits in the current mode, so the box shows the zoom again.
`CSLensLadderMove` steps one rung and frees the factor from fitting.
A positive delta steps up and any other steps down, so the driver hands a raw wheel delta unread.
A factor between rungs steps to the nearest rung in that direction.
Past either end it stays on the end rung.
`CSLensModeSet` stays at the old factor without a paper or a laid-out view.
`CSLensAreaSet` refits in the current mode, and a free factor stays.
`LSLensPaperSet` takes zeros after a failure and refits, so a new paper or orientation keeps the chosen fit.
`LSLensResolutionRead` caps the shown width at twice the view width, matching what the driver decodes.
It answers null while the proof's default resolution already covers that width.
No pixel width leaves this method, only a document resolution.
`LSLensFitRead` answers the current factor for a free mode, a missing paper or an empty view.

`CSLensMode` has three members.
- Width fills the view width and is the mode a new print window opens in.
- Page fits the whole page, the smaller of the width and height ratios.
- Free keeps the factor as typed or stepped, whatever the view does.

`CSLensState` is the answer every lens gate returns.
The factor is between 0.1 and 4.
The text is the factor as a whole percent in the current culture.
The driver also derives the decoded picture width from the factor, under the same cap the lens uses.

## Stamp

Commit: `c2f335c331fe`
- `CSGalley.cs` `ce974bb1a933f340`
- `CSGalleyDelay.cs` `0a9301b2e0feefb8`
- `CSGalleyLeaf.cs` `41ecbf1e5422afda`
- `CSGalleyWindow.cs` `b2e9a298de0b6e59`
- `CSLens.cs` `f7d46996ac6ededb`
- `CSLensMode.cs` `940bc2ab91d73e54`
- `CSLensState.cs` `05c2a8a8c597edd3`
- `CSPressPage.cs` `072a1a25bb06b306`
- `CSPressRange.cs` `841942edac1c4b77`
- `CSPressState.cs` `79da20a85e5cd51c`
