# LBulletinRoster.cs
Hash: `089b003a16e92876`

## `internal sealed class LBulletinRoster`

One ordered list of bulletin observers, shared by the engine, a tenure and a vista.
Each entry may name a subject and an id, and a missing one matches every bulletin.
The roster takes no lock, so each owner keeps its own gate around every call.
Owners copy the list under their gate and dispatch outside it.
So an observer can attach or detach while a dispatch runs without disturbing it.

## `internal LBulletinRoster(Func<LBulletin, bool>? filter)`

The optional filter is asked for every entry a bulletin would reach.
It is asked again per entry, so state read by it counts as of that call.
A vista passes one to reach its chosen observers only for the chosen row.

## `internal void LBulletinRosterAttach(LSubject? subject, Action<LBulletin> observer, long? id)`

Appends the entry, so dispatch reaches observers in the order they were attached.
A null subject or id matches every bulletin on that field.

## `internal bool LBulletinRosterCheck(Action<LBulletin> observer)`

Whether any entry holds `observer`, so the engine can refuse a second attach.

## `internal void LBulletinRosterDetach(Action<LBulletin> observer)`

Removes the first entry holding `observer`, and does nothing when none does.

## `internal void LBulletinRosterClear()`

Drops every entry, so nothing attached before is reached again.

## `internal (LSubject?, Action<LBulletin>, long?)[] LBulletinRosterRead()`

A copy of the entries, taken under the owner's gate before a dispatch.

## `internal void LBulletinRosterDispatch(LBulletin bulletin, (LSubject?, Action<LBulletin>, long?)[] snapshot)`

Calls each snapshot entry whose subject and id match the bulletin and whose filter agrees.
Entries are reached in snapshot order, so an earlier observer may change state a later one reads.
