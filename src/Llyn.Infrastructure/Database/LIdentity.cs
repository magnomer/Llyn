using System.Security.Cryptography;

namespace Llyn.Infrastructure;

/// <summary>
/// Generates the opaque random identifiers that Entry, Example, Tag, and Situation carry. An id is a
/// fixed-length string of lowercase letters and digits (for example <c>176f86dr4k2q</c>) drawn from a
/// cryptographic source, never derived from any visible content — so an id reveals nothing about the
/// row it names and two rows with identical text still get distinct ids.
/// <para>
/// The length is what keeps "distinct" true without a retry loop. Twelve symbols over an alphabet of
/// thirty-six is about 4.7e18 identifiers, which leaves the chance of any collision negligible across
/// every row a workspace will ever hold; a shorter id would eventually surface as a primary-key
/// violation in the middle of a write, where there is nothing useful to do about it.
/// </para>
/// </summary>
public static class LIdentity
{
    private const int LIdentityLength = 12;
    private const string LIdentityAlphabet = "0123456789abcdefghijklmnopqrstuvwxyz";

    /// <summary>Returns a new opaque identifier. Distinct across calls with overwhelming probability.</summary>
    public static string LIdentityCreate()
    {
        return RandomNumberGenerator.GetString(LIdentityAlphabet, LIdentityLength);
    }
}
