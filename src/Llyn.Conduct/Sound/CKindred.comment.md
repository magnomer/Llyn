# CKindred.cs
Hash: `78ceb564cfeb62e7`

## `public sealed class CKindred`

The reflex block of the entry an editor holds, as the editor shows or edits it.
It is split from `CTimbre` by concern, since the reflex rows have their own ports, quill and lookup.
The editor builds it over its desk, its display, its reflex, draft and settings ports, and its envoy.
So it keeps no copy.

## `internal CKindred(CDesk desk, LReflexPort reflexes, LDisplay display, LDraftPort drafts, LSettingsPort settings, CEnvoy envoy)`

Takes `envoy` and `settings`, so a refused anchor read shows its notice.
It starts the reflex lookup whenever the desk prepares a draft.

## `public event Action? CKindredChanged;`

The held entry's reflexes changed, raised on the driver's thread.

## `public bool CKindredPending`

Whether the held entry's reflex lookup is still running, so the reflex block shows its loading line.

## `public CTimbreReflex CKindredRead()`

The held draft's reflex block, ready to paint, as the reading view reads its own.
Every row shows, the blank ones included, each resolved by the shared reflex scan.
The anchor labels are read against the draft's headword through the shared anchor map.
It hands the map this class's envoy and settings and the display's repaint memory.
So a refused anchor read shows `Display.AnchorFailed` once until the user acts.
The fold comes from the editor's display, which shares it with every reading view.
An empty desk answers no rows, no anchor and no fetching line.
A refused scan is not caught here.

## `private void LKindredStart(LDraft _)`

Starts the reflex lookup whenever the desk prepares a draft, before the editor's bulletin repaints.
So the first paint of a stored entry without reflexes already shows the fetching line.
A reflex quill over the held tenure decides whether a lookup is due.
A failure is not caught here, so it reaches the desk's draft load and shows its `LoadFailed` notice.

## `public void CKindredRebuild()`

The user asked to look the held entry's reflexes up again.

## `public void CKindredAdd(long? reflex)`

The user pressed the plus of reflex row `reflex`, or of the header when it is null.
Null means no row is pressed, so the new row lands after all rows.
Conduct maps null to the engine's id 0, so the driver never sends a magic id.
The engine places the new row below the pressed one, in its language and kind.

## `public void CKindredRemove(long reflex)`

The user pressed the minus of reflex row `reflex`.

## `public void CKindredToggle(long reflex)`

The user pressed the star of reflex row `reflex`.
The engine flips the main mark the draft holds, so a row it no longer holds sends nothing.

## `public CReflexTyped CKindredSet(long reflex, CReflexField field, string text)`

The user typed `text` into the cell `field` of reflex row `reflex`.
Each cell has its own engine member, and the engine defers every one.
The answer is the row as it now reads, which the driver keeps in place of its own copy.
It finds the row in the held draft before the write and copies it through `CReflex.CReflexTypedApply`.
A taken edit answers the typed text, since the deferred draft has not caught up yet.
A desk that fills its view takes no edit, and answers the row as the draft holds it.
A row the held draft lacks answers no row.
A typed language answers every row's lead, with the typed language standing in for the row's stored one.
Other cells answer no lead, and neither does an edit not taken.
Only the edit in hand is overlaid, so another row's edit still deferred reads as stored.
The lead compares the raw typed language, as the scan compares the stored one.

## `private CReflex? LKindredFind(long reflex)`

The row `reflex` as the held draft reads, or null for a row it lacks or an empty desk.
It scans the rows alone, so a keystroke reads no anchor.

## `private IReadOnlyList<CReflex> LKindredRowRead(LEntryDraft content)`

The rows of `content`, each resolved by the shared reflex scan.
`CKindredRead` and `LKindredFind` both read their rows here.

## `private static IReadOnlyList<CReflexHead> LKindredLeadRead(IReadOnlyList<LReflexDraft> typed)`

Marks the rows the engine answered by the lead rule `CReflex.LReflexLeadRead`, the one the scan uses.

## `internal void LKindredObserverAttach(Action<Action> marshal)`

Hears the reflex subject of the held entry.

## `private long? LKindredEntry`

The stored entry the held draft stands on, or null for a fresh draft or an empty desk.

## `private LTenure? LKindredTenure`

The held draft's tenure for a write, or null while the desk fills its view.
So a row the render writes raises no request.

## `internal LQuillReflex? LKindredQuill`

The reflex row edits over the writing tenure and the editor's reflex port, or null while the desk fills.
It is internal so `CSoundingAnchor` sends its anchor edit through the same quill.
