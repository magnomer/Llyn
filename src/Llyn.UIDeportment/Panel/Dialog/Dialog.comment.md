# Dialog

This folder holds the coinage, customs and leave modal drivers, plus their presentation values.
Each driver pulls a fresh window by contract ID and attaches its look independently of the owner window.
Named parts are pulled from that dialog window, keeping the drivers independent of its markup structure.

## `QSCoinage`

Only the mint handler sets a successful dialog result, keeping dismissal distinct from accepted wording.

### `QSCoinageShow`

The field takes focus on open, so the user types straight away.
Accepted wording returns unchanged, leaving any normalization to the caller.
Dismissal returns null instead of treating the current field text as accepted.

### `QSCoinageWordingRefine`

The mint button follows `CSCoinage`'s wording check, keeping acceptance rules outside the dialog.

## `QSCustoms`

Each row's mutable mode and target live in the supplied `CSCustoms` gate rather than its presentation item.
The dialog calls that gate for readiness and row state, keeping intake decisions outside presentation.

### `QSCustomsChoice`

Contract IDs map the markup choices to modes, so the selected markup item need not carry a Conduct enum.

### `QSCustoms`

Each presentation row carries the supplied candidate ids, so the dialog performs no candidate search.
Initial accept availability comes from the gate's readiness check.

### `QSCustomsConsult`

The dialog returns acceptance rather than a new row list, leaving the supplied gate as the owner of selections.

### `QSCustomsOmissionConsult`

The omission list uses line and text pairs of its own, keeping Conduct records outside generated template items.
Declaration default and cancel behavior is disabled before showing the omission face.

### `QSCustomsRowRefine`

One gate read supplies each row's state.
After a selection change, every generated row is refilled because another row's state may also differ.
The loss wording uses the gate's resource key and ready counts, avoiding local loss decisions.

### `QSCustomsModeApply`

The handler is off while the fill selects, so a fill never writes back into the gate.

### `QSCustomsTargetApply`

The handler is off while the fill selects.
Candidate items are replaced only when the supplied list instance differs.
The selected candidate's template is filled after a loaded-priority dispatch, when its parts can be found.

### `QSCustomsRefine`

The returned readiness updates the accept button before requesting a full item refill.

## `QSCustomsItem`

The row retains its zero-based index for gate addressing and displays a one-based number.
Its candidates are supplied unchanged, keeping candidate discovery outside this value.
Mode and selected target stay in `CSCustoms`, so this item holds no user-editable selection.

## `QSLeave`

The initial answer is stay, so dismissing without a button preserves that answer.
Button handlers choose an answer before closing the window, keeping the result independent of `ShowDialog`'s return value.

## `QSLeaveAnswer`

The three explicit choices distinguish storing, discarding and staying without overloading a boolean dialog result.

## Stamp

Commit: `c2f335c331fe`
- `QSCoinage.cs` `24970800a080e78f`
- `QSCustoms.cs` `625c39ff992690e9`
- `QSCustomsItem.cs` `5bff43c2c4aa264d`
- `QSLeave.cs` `3e8093a00a96af04`
- `QSLeaveAnswer.cs` `e5e3cd9c62e29f69`
