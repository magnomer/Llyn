# LWarrant.cs
Hash: `836b51c37d0f22a2`

## `public interface LWarrant`

The port that protects the Joplin token at rest for the current user.
Protection depends on the operating system, so a platform twin implements it.
Core stays free of platform code and only ever sees the hidden form.

## `string LWarrantHide(string token);`

Returns a protected, printable form of `token` that settings can store as text.
It refuses a blank token, since there is nothing worth protecting.
It can throw when the platform cannot protect data, for example when the user profile is not loaded.

## `string? LWarrantRestore(string hidden);`

Returns the token that `hidden` protects.
It answers null when the token cannot be recovered, for another user, another machine or damaged text.
On another machine a roaming Windows profile may still recover it there.
A null tells the caller to ask Joplin for a fresh token.
