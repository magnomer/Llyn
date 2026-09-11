# LIdentity.cs

## `public static class LIdentity`

Issues the temporary ids a draft row carries before the database has given it a real one.

A row that has never been stored has no id, yet the draft holding it still needs to name it.
Something has to say which sentence a chip stands for while the user is still editing.
So the shell issues a temporary id, and every link inside the draft is made through that id exactly as it would be through a real one.

Temporary ids are negative and real ids are positive, because SQLite counts rows from one.
That makes the two impossible to confuse and removes the need for a second kind of key beside the id.
A row whose id is still negative has never been written; a row whose id is positive has.

The issuer lives here and not in the UI, because the UI holds no ground truth.
The draft is held below the UI, ids are issued below the UI, and saving sends that held draft down to the database rather than reading it back off the widgets.

## `public static long LIdentityCreate()`

Issues the next temporary id.

Counts downward, so ids are distinct for the life of the process and never stray into the range the database assigns.
Interlocked, because drafts are edited from the UI thread while the shell works on others.

## `public static bool LIdentityTemporary(long id)`

Reports whether `id` names a row the database has never stored.

Zero counts as temporary, because it is what a record carries before anything has been issued at all.
