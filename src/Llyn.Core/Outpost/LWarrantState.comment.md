# LWarrantState.cs
Hash: `615afc9002387e31`

## `public enum LWarrantState`

Where a token request stands inside Joplin.

## `LWarrantStateWaiting,`

The user has not answered yet, so the caller polls again.

## `LWarrantStateAccepted,`

The user granted the request, so polling stops and the token is read.

## `LWarrantStateRejected`

The user refused the request, so polling stops.
An unknown or expired ticket is reported as rejected, so polling never runs forever.
