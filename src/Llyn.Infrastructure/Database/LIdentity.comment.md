# LIdentity.cs

## `public static class LIdentity`

Generates the opaque random identifiers that Entry, Example, Tag, and Situation carry.
An id is a fixed-length string of lowercase letters and digits, for example `176f86dr4k2q`.
It is drawn from a cryptographic source and never derived from any visible content.
So an id reveals nothing about the row it names.
Two rows with identical text still get distinct ids.

The length is what keeps "distinct" true without a retry loop.
Twelve symbols over an alphabet of thirty-six is about 4.7e18 identifiers.
That leaves the chance of any collision negligible across every row a workspace will hold.
A shorter id would eventually surface as a primary-key violation in the middle of a write.
There is nothing useful to do about it there.

## `public static string LIdentityCreate()`

Returns a new opaque identifier.
Distinct across calls with overwhelming probability.
