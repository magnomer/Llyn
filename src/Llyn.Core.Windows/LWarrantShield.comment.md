# LWarrantShield.cs
Hash: `fae5bf15c3da86b0`

## `public sealed class LWarrantShield : LWarrant`

The Windows adapter that protects the Joplin token with DPAPI for the current user.
Only the same user on the same machine can read the stored text back.

## `private static readonly byte[] LWarrantShieldSalt = Encoding.UTF8.GetBytes("Llyn.Warrant");`

A fixed entropy that separates Llyn's protected tokens from other programs' data.
It does not stop a process running as the same user.

## `public string LWarrantHide(string token)`

Refuses a blank token, since there is nothing worth protecting.
The result is Base64 so settings can keep it as plain text.
A `CryptographicException` from Protect escapes, so the caller reports it.

## `public string? LWarrantRestore(string hidden)`

Answers null for every unrecoverable input instead of throwing.
A caller then asks Joplin for a fresh token rather than failing on a stale setting.
