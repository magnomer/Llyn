# LGalleyClerk.cs
Hash: `a7d1b05795d66092`

## `public sealed class LGalleyClerk : IAsyncDisposable`

The print session's hold on the board and on the proof it drew last.
ShellEngine may not name the Core board or proof, so it reaches both through this clerk.
`LPortraitClerkPress.LPortraitClerkOpen` builds one around a freshly opened board.

The clerk is the single owner of the latest-wins rule.
Every proof draw takes a generation, and a draw that returns after a newer one started drops its own proof.
The proof and the setting it was drawn from change together, so the held proof always matches its setting.

There is no turn chain.
The board twin already serialises every board call, and the generation orders the results.
So the clerk relies on a board that runs one call at a time.

There is no `ConfigureAwait(false)` here.
The board is bound to the thread that opened it.

## `public const int LGalleyClerkResolution = 300;`

The resolution in dots per inch a page is drawn at when the caller names none.
The clerk owns it, so Conduct's lens asks for more detail only when the zoom needs it.
300 is print quality, sharp on screen up to about three times actual size.

## `private readonly Dictionary<LPressProof, int> _lGalleyClerkReader`

The page draws in flight, counted per proof.
A replaced proof with readers stays open until its last reader ends, and that reader disposes it.
It also serves as the lock over the held proof and the counts.

## `internal LGalleyClerk(LPressBoard board)`

Takes ownership of the board, which the clerk disposes.

## `public int? LGalleyClerkCount`

The page count of the held proof, or null before a draw and after dispose.

## `public bool LGalleyClerkCheck(LPressSetting setting)`

Whether the held proof still shows `setting`.
Only the layout counts, through `LPressSetting.LPressSettingMatch`.
Nothing held never matches, so the session draws.

## `public async Task<bool> LGalleyClerkDraw(LPressSetting setting)`

Draws a proof of `setting` and holds it, answering false when a newer draw superseded this one.
The generation is taken before the up-to-date check.
So a draw that finds the held proof current still supersedes any older draw in flight.
A superseded proof is disposed here, and the held one is never touched by it.
A draw after dispose throws `ObjectDisposedException`.

## `public async Task<LPressImage?> LGalleyClerkDraw(int page, int? resolution = null)`

One page of the held proof as a PNG at `resolution` dots per inch.
A null resolution draws at `LGalleyClerkResolution`.
It answers null when no proof is held, or when a newer proof replaced this one during the draw.
That is latest-wins, not a failure, so it never throws for it.
A page outside the proof, or a resolution below one, still throws.

The proof is counted as read for the whole draw.
A draw or dispose that replaces it then leaves the disposing to this reader.

## `public LPressPaper? LGalleyClerkRead(int page)`

The size of one page of the held proof, in inches.
It answers null when no proof is held or the proof has no such page.

## `public async Task<LPressStatus> LGalleyClerkPrint(LPressSetting setting, string? file)`

Prints `setting` on its printer, or exports it as PDF to `file` when one is given.
An export answers done, since only printing reports a status.

## `public async Task LGalleyClerkShow()`

Opens the system print dialog over the board.

## `public async ValueTask DisposeAsync()`

Ends every draw in flight by taking a new generation, so each disposes its own proof when it returns.
It drops the held proof unless a page draw still reads it, then disposes the board.
The board twin waits for its call in flight before it closes.

## `private void LGalleyClerkSet(LPressProof? proof, LPressSetting? setting)`

Replaces the held proof and its setting as one step.
The replaced proof is disposed at once when no page draw reads it.
