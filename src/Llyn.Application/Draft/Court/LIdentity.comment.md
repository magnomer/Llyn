# LIdentity.cs

## `public sealed class LIdentity`

Issues the temporary ids a draft row carries before the database has given it a real one.

A row that has never been stored has no id, yet the draft holding it still needs to name it.
Something has to say which sentence a chip stands for while the user is still editing.
So the application ring issues a temporary id.
Every link inside the draft is made through that id exactly as it would be through a real one.

Temporary ids are negative and real ids are positive, because SQLite counts rows from one.
That makes the two impossible to confuse and removes the need for a second kind of key beside the id.
A row whose id is still negative has never been written.
A row whose id is positive has.

The issuer lives here and not in the UI, because the UI holds no ground truth.
The draft is held below the UI, and ids are issued below the UI.
Saving sends that held draft down to the database rather than reading it back off the widgets.

The issuer is an instance the engine owns, one per open workspace.
It counts from the workspace row rather than from a process-wide static.
Drafts, court links, and claims are files named by the id they were issued.
A counter that restarted at zero with the process would name a new draft after a file still on disk.
So the floor lives in the workspace database and every issue lowers it there before the id is used.

## `public LIdentity(LWorkspaceVault workspaces, IReadOnlySet<long>? retired = null)`

Binds the issuer to the workspace vault whose floor it lowers.
The retired ids are ones a tenure still holds from a workspace the engine has left.
Each workspace counts from its own floor, so the next one would otherwise issue those ids again.

## `public long LIdentityFloor`

The lowest id issued so far, read from the workspace row.

## `public long LIdentityCreate()`

Issues the next temporary id.

Lowers the floor in the database and returns the new floor, in one statement.
The write commits before the id is handed out.
A crash after the call therefore cannot bring the id round again.
Two engines open on one workspace lower the same row, so neither can issue what the other already holds.
A retired id is skipped, so a draft of this workspace never shares an id with a stale tenure.

## `public static void LIdentityRecord(Dictionary<long, long> identity, long draftId, long rowId)`

Records under `draftId` the real `rowId` a store just gave a draft row.
A `draftId` that is not negative names a stored row already and is not recorded.
The map is what the commit hands back so the held draft can be renumbered onto real ids.
