# CImprint.cs
Hash: `4649fce23c8999d5`

## `public sealed class CImprint`

The source editor: the desk over one Source draft, its credit rows and the byline under them.
The stated fields go down raw, and the draft notice writes back what the engine holds.
The credit rows are the engine's, and the only credit state kept here is where the one blank row sits.
Every request is built by the desk's quill, so the editor names no request.
`CShelf` builds it over the atelier's ports.

## `internal CImprint(LDraftPort drafts, LEntryPort entries, LSettingsPort settings, CEnvoy envoy, CLedgerNoticed noticed, Action<Action> marshal)`

Builds the desk under the `Source` scope, and the byline over the same draft port.
It takes the atelier's repaint memory only to hand it to the byline, which has no other way to it.
The desk's prepared draft is announced as the reference notice.
The desk hears its tenure and draft notices through `marshal`, so no driver wires them.

## `public event Action? CImprintChanged;`

The credit rows moved, so the driver reads them again.

## `public event Action<CReference>? CImprintReferenceChanged;`

The held Source's fields, ready to write, raised on every draft notice.

## `public event Action? CImprintFocused;`

A blank row opened, so the driver moves the caret into it.

## `public event Action? CImprintReverted;`

The typed credit is dropped, so the driver writes the row's held name back into its field.

## `public CDesk CImprintDesk { get; }`

The desk that holds the Source draft and offers its quill.
The shelf reads its storable state and finish through it, and the byline reads its draft id.

## `public CByline CImprintByline { get; }`

The popup of Authors a typed credit may already name.

## `public bool CImprintHeld => CImprintDesk.CDeskHeld;`

Whether a draft is held, so every credit gate and the byline do nothing without one.

## `internal void LImprintOpen(long? id)`

Starts the desk over the Source, or over a fresh one when `id` is null.
The blank row and the byline start closed.

## `internal void LImprintCancel()`

Drops the held draft and announces the rows, then closes the byline.

## `internal void LImprintSave()`

Stores the draft only when it changed, so an unchanged save keeps the draft held.

## `public CReference CImprintEmptyRead()`

The fields of a blank Source, written while no draft is held.

## `public string CImprintTallyRead()`

The citation line of the stored Source, worded by the engine.
A fresh Source has no uses yet.

## `public void CImprintTitleSet(string text)`

Hands the typed title to the quill, which defers it.
The year, address and note gates do the same for their own fields.
While the desk fills its fields from a notice, no quill is offered and nothing is sent.

## `public void CImprintKindSet(string? tag)`

Hands the picked menu tag to the quill, which sends the kind at once.

## `public static IReadOnlyList<CReferenceKind> CImprintKindRead()`

The kind menu the driver builds once, each option's tag and key already decided by the engine.
It is static, since the menu is the same for every Source and needs no desk.
Every tag appears once, since the read drops a repeated tag and keeps its first kind.
So a driver can pair each option to its tag without meeting two radios for one kind.
It hands the engine's static kind list to `LImprintKindRead`, which holds that rule.

## `internal static IReadOnlyList<CReferenceKind> LImprintKindRead(IReadOnlyList<(string LReferenceKindTag, string LReferenceKindKey)> rows)`

Maps the kind rows into menu options, in their order, keeping the first kind of each tag.
It takes the rows as a parameter, so a test can feed rows the static source never gives.

## `public IReadOnlyList<CAuthorRow> CImprintCreditRead()`

The credit rows of the held draft, with the one blank row already placed among them.
While a draft is held, the blank row is opened on an empty list and kept within the rows.
The blank row has id `0`, an empty name, its own place and no move.
The blank row alone carries `CAuthorRowBlank`, and every engine row carries it false.
The driver tests that mark only to paint the row.
The add, remove and move gates never take that mark back from the driver.
They decide blankness from Conduct's own marker, the place the blank row was inserted.
Id `0` is the engine's word for no Author, so no stored credit carries it.

## `public void CImprintAuthorAdd(int? position, long? id)`

The add button of a credit row opens a blank row beneath it and asks for the caret.
The blank row's own add button only asks for the caret.
A row with id `0` is the blank row only at the place Conduct keeps for it.
An id `0` anywhere else names no row, so nothing happens.

## `public void CImprintAuthorRemove(long? id)`

The remove button closes the byline, then closes the blank row or sends the credit's removal.
Id `0` is the blank row, closed only while Conduct's marker says one is open.
Id `0` is never sent to the engine.

## `public void CImprintAuthorMove(int? position, long? id, int step)`

The earlier or later button sends the credit one place along by `step`.
The blank row has no place in the draft, so it never moves.
Id `0` names the blank row or no row, so it is never sent to the engine.

## `public bool CImprintAuthorFinish(int? position, long? id, string? text, long? chosen)`

Enter in a credit field, answering whether it took the key.
With the byline offering a lit row, the lit Author takes the field's place.
Otherwise the typed name is credited, and a blank name reverts the field.

## `public bool CImprintAuthorCancel(int? position, long? id)`

Escape in a credit field, answering whether it took the key.
An offered byline only closes, and with none the field reverts.

## `internal void LImprintAuthorInsert(int at, long author, long picked)`

Credits a picked Author in place of the row's former credit.
The blank row closes before the quill sends, since the new credit repaints the rows at once.
When the quill sends nothing, the blank row comes back and the field reverts.

## `private void LImprintCreditCommit(int at, long author, string? text)`

Credits a typed name in place of the row's former credit, in one quill call.
A name the quill refuses keeps the blank row and reverts the field, as a picked Author does.

## `private void LImprintBlankSet()`

Opens the blank row on an empty list, and pulls it back within the rows when they shrank.

## `private void LImprintDraftShow(LDraft draft)`

Announces the rows and the fields of a prepared draft, passed unread to the engine's edit sheet.

## `private static CReference LImprintReferenceRead(LImprint sheet)`

Copies the engine's edit sheet into the record the driver writes.
