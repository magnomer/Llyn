# CImprint.cs

## `public sealed class CImprint`

The source editor: the desk over one Source draft, its credit rows and the byline under them.
The stated fields go down raw, and the draft notice writes back what the engine holds.
The credit rows are the engine's, and the only credit state kept here is where the one blank row sits.
Every request is built by the desk's quill, so the editor names no request.
The sources panel builds it over its ports until `CShelf` does.

## `internal CImprint(LDraftPort drafts, LEntryPort entries, CEnvoy envoy)`

Builds the desk under the `Source` scope, and the byline over the same draft port.
The desk's prepared draft is announced as the reference notice.

## `public event Action? CImprintChanged;`

The credit rows moved, so the driver reads them again.

## `public event Action<CReference>? CImprintReferenceChanged;`

The held Source's fields, ready to write, raised on every draft notice.

## `public event Action? CImprintFocused;`

A blank row opened, so the driver moves the caret into it.

## `public event Action? CImprintReverted;`

The typed credit is dropped, so the driver writes the row's held name back into its field.

## `public CByline CImprintByline { get; }`

The popup of Authors a typed credit may already name.

## `public int CImprintBlankAt`

The place of the one blank credit row, or `-1` when none is open.

## `public void CImprintOpen(long? id)`

Starts the desk over the Source, or over a fresh one when `id` is null.
The blank row and the byline start closed.

## `public void CImprintCancel()`

Drops the held draft and closes the byline, then announces both.

## `public void CImprintSave()`

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

## `public IReadOnlyList<CAuthorRow> CImprintCreditRead()`

The credit rows of the held draft.
While a draft is held, the blank row is opened on an empty list and kept within the rows.

## `public void CImprintAuthorAdd(int? position, long? id)`

The add button of a credit row opens a blank row beneath it and asks for the caret.
The blank row's own add button only asks for the caret.

## `public void CImprintAuthorRemove(long? id)`

The remove button closes the byline, then closes the blank row or sends the credit's removal.

## `public void CImprintAuthorMove(int? position, long? id, int step)`

The earlier or later button sends the credit one place along by `step`.
The blank row has no place in the draft, so it never moves.

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
