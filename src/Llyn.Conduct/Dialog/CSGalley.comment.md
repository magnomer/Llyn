# CSGalley.cs
Hash: `ce974bb1a933f340`

## `public sealed class CSGalley : IDisposable`

The print window's proof draw gates over one galley, the preview the canvas shows.
`CSPress` builds it over the same session and hands it out as `CSPressGalley`.
The setting each draw answers comes from `CSPress`, which owns the page and the running flag.
A failure goes through `CLedger.LLedgerFailureShow` as `Print.RenderFailed`.

It holds three roles apart, each in a type that owns its own state.
The waits a person causes by typing or scrolling live in two `CSGalleyDelay` values.
The sight window and its delivered pages live in one `CSGalleyWindow`, replaced whole.
The zoom lives in one `CSLens`, which learns each proof's paper and answers the resolution a sight needs.
This class keeps only the gates, the events and the closed flag.
Continuations resume on the caller's context, since the galley is bound to the thread that opened it.
So every event is raised on that thread, and the driver marshals nothing for it.
Engine order stays below, where the newest draw wins and a replaced proof's page answers null.

## `private readonly LGalleyRelay _csGalleyEngine;`

The proof relay the proof and its pages are drawn through, owned and disposed by `CPortrait.LPortraitPrint`.

## `private readonly CEnvoy _csGalleyEnvoy;`

The port a draw failure is shown through.

## `private readonly LSettingsPort _csGalleySettings;`

The port a failure notice is read through, the same one the print gate opened with.

## `private readonly CSPress _csGalleyPress;`

The press gate that answers the setting a draw carries.
It is read through its public setting gate, never through its fields.

## `private CSGalleyWindow _csGalleyWindow`

The sight window of the current proof with the pages delivered in it.
`LSGalleyWindowSet` is its one writer, so every page it stops holding is announced as dropped.
It starts empty, with no pages and no window.

## `private bool _csGalleyClosed`

Set by `Dispose`, after which no wait starts and no event fires.

## `internal CSGalley(LGalleyRelay relay, CEnvoy envoy, LSettingsPort settings, CSPress press)`

Built only by `CSPress`, so both gates share one relay and the session behind it.

## `public event Action<CSPressState>? CSGalleyStateChanged;`

Raised when a deferred redraw finishes with a proof or a failure.
A superseded redraw raises nothing, since a newer one is on its way.
The direct gate `CSGalleyDraw` raises nothing, since its caller already holds the answer.

## `public event Action<CSPressPage>? CSGalleyPageChanged;`

Raised once per page a sight delivers, with its picture.
A superseded page answer and a failed one raise nothing.
Also raised with a null picture for each delivered page the galley stops holding, in page order.
A page drops when a sight's window leaves it or a finished draw starts a new proof.
A page asked again at another resolution is not dropped, since its new picture replaces the old.
So a driver's picture cache has this one event as its only writer.

## `public CSLens CSGalleyLens { get; }`

The zoom of this preview, one per galley.
The driver hands it raw zoom input and paints what it answers.
The galley feeds it each proof's paper size and asks it for a sight's resolution.

## `internal CSGalleyDelay CSGalleyEditDelay { get; }`

The wait after a stale edit before the deferred redraw, 250 milliseconds in production.
Tests set its time, and the sight delay's, through `CSPress.CSPressGalley`.

## `internal CSGalleyDelay CSGalleySightDelay { get; }`

The wait after a sight before its page requests, 150 milliseconds in production.

## `public async Task<CSPressState?> CSGalleyDraw()`

Draws a proof of the current layout through one relay call, and the latest request wins.
It is the one awaitable draw, which the retry button and the deferred redraw both call.
It answers null when a newer draw superseded this one, so the driver drops that answer.
A failed draw shows `Print.RenderFailed` through the ledger.
It also answers that key with no pages, which is the window's retry veil, not a second notice.
That answer's setting is not stale, since the failure schedules no redraw, so only the failure shows.
Either answer carries the setting read after the draw, since the page count changes the printed pages.
Either answer also starts a new window with the next turn and the new page count.
Either answer carries the lens after it learned the proof's paper, zero after a failure.
A fitting lens refits there, so the zoom follows a paper or orientation change.
Every page delivered from the old proof is announced as dropped before the answer returns.

## `public async Task CSGalleySightSet(int page, int first, int last, double scale)`

Takes the raw sight of one scroll, the page a reader is on, the visible pages and the monitor scale.
The lens turns the scale and its zoom into a resolution in dots per inch.
It answers null while the proof's default suffices, so no pixel width crosses the cut.
It first hands `page` to `CSPress.LSPressPageSelect`, which ignores a negative or unchanged page.
So the current page has one owner, and the driver keeps no copy to compare against.
Then it moves the window before any wait, announcing the delivered pages it leaves as dropped.
The window is the visible range and one page either side, clipped to the proof.
A negative first page means nothing is visible.
Then the window stays as it was and nothing is requested.

After the sight wait it requests each page of the window not yet held at that resolution.
An answer is delivered only while the window still wants it, so no page lands outside the window.
A newer sight stops the older one before its next request.
The first failed page ends the pass and shows `Print.RenderFailed` once.
A superseded page answers null and stays silent.
The task ends when the requests are answered, a page fails, the wait is cancelled or a new proof arrives.
It never faults.

## `public void Dispose()`

Closes the galley when the window closes, before the session is disposed.
It cancels both waits, and an answer still in flight raises nothing.
A second call does nothing more.

## `internal CSPressSetting LSGalleyProofDefer(CSPressSetting setting)`

Called only by `CSPress.LSPressSettingRaise`, with each setting before the press raises it.
A stale setting restarts the redraw wait, so quick edits end in one redraw of the newest setting.
A fresh setting leaves a pending wait alone, so a redraw already due still runs.
It answers `setting` unchanged, though its one caller ignores the answer.

## `private async Task LSGalleyProofDraw()`

Waits the edit delay, draws through `CSGalleyDraw`, and raises the state event with the answer.
A cancelled wait ends quietly, since a newer edit or the close replaced it.

## `private void LSGalleyWindowSet(CSGalleyWindow window)`

Replaces the window whole and announces each page the old one held and the new one does not.
After close it only stores the window, so no event fires.
