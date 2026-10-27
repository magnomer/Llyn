# Outpost

This folder holds the port and the records for the one-way push of entries to Joplin.
Joplin is an outside program reached over its local Data API, so Core names neither the client nor the address.
Llyn's copy always wins, and a manifest kept by the entrance lets the next push send only what changed.

## `LOutpost`

The port to Joplin's local Data API, which receives Llyn's entries one way.
The entrance in Infrastructure is the only implementation, so Core never learns the protocol.
It offers two calls, one to pair with Joplin and one to push everything.
Both take a cancellation last, so a slow or stalled Joplin never holds a push open.
A refusal reaches the caller as an `LRefusal`.
Every other cause of an unreachable Joplin reads the same to the user.

### `LOutpostAttach`

Asks Joplin for a token and answers it once the user accepts the request inside Joplin.
The port is the stored one, which is tried before any other.
The fault sink receives every caught failure before it is turned into a refusal.
A Joplin that answers on no port refuses, since no request can reach it.
A rejection refuses with `LRefusalWarrant`, and running out of polls refuses with `LRefusalPending`.

### `LOutpostSend`

Pushes one cargo and answers the receipt of what happened.
Llyn owns every note it writes, so an edit made inside Joplin is replaced.
A note gone from the workspace is trashed.
A note is trashed only when it is proven to be Llyn's.
Llyn must never act on a Joplin item it did not create.
The user can still recover a note trashed by mistake, since trashing is not deleting.
The push stops on a refusal or a cancellation, and a failed note does not stop it.

## `LOutpostCargo`

Everything one push needs, gathered by the application clerk so the outpost holds no engine state.
It carries the port, the token, the top notebook title, the realm text, the entries and the language names.
The page, sound and lookup functions let the outpost read pages while it sends.
They cover each entry's page and each language's reconstruction side.
Reading is lazy, so a failed read costs one entry or one language and not the whole push.
The fault sink receives every caught failure.
The realm binds the push to the workspace that wrote the manifest.

## `LOutpostNote`

One Joplin note as Llyn pushes it.
A stable id lets a later push overwrite the same note instead of adding a copy.
The id is 32 lowercase hex characters, the only form Joplin resolves.
The body is Markdown.

## `LParcel`

One attachment pushed to Joplin as a resource that notes can link to.
Its id derives from the file content, so a changed image gets a new id.
The note body that references the id changes too, so the note is pushed again.
That is why a push may skip an id Joplin already holds.
The id is fixed, so a note body can name it before the upload happens.

## `LReceipt`

The tally of one push to Joplin, shown to the user when it ends.
A failed note does not stop the push, so failures are listed rather than raised.
A reconstruction note whose title itself failed is listed under its language name.
The saved count never includes the style note.

## `LWarrant`

The port that protects the Joplin token at rest for the current user.
Protection depends on the operating system, so a platform twin implements it.
Core stays free of platform protection code.

## `LWarrantHide`

Returns a protected, printable form of the token that settings can store as text.
It refuses a blank token, since there is nothing worth protecting.
It can throw when the platform cannot protect data, for example when the user profile is not loaded.

## `LWarrantRestore`

Returns the token that the hidden form protects.
It answers null when the token cannot be recovered, for another user, another machine or damaged text.
On another machine a roaming Windows profile may still recover it there.
A null tells the caller to ask Joplin for a fresh token.

## `LWarrantAnswer`

Joplin's reply to one poll of a pending token request.
The token is present only when the request was accepted.

## `LWarrantState`

Where a token request stands inside Joplin.
Waiting means the user has not answered yet, so the caller polls again.
Accepted means polling stops and the token is read.
Rejected means polling stops.
An unknown or expired ticket is reported as rejected, so polling never runs forever.

## Stamp

Commit: `1f3467bbe5c6`
- `LOutpost.cs` `21ccdda98ef1879f`
- `LOutpostCargo.cs` `afaf1af63f2d4749`
- `LOutpostNote.cs` `54e940cc97fb43e6`
- `LParcel.cs` `1ad3b2f7ad138c3e`
- `LReceipt.cs` `a2abbd39386ba268`
- `LWarrant.cs` `836b51c37d0f22a2`
- `LWarrantAnswer.cs` `62ad2b48ebd61fe0`
- `LWarrantState.cs` `615afc9002387e31`
