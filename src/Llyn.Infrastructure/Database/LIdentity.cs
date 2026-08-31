using System.Security.Cryptography;

namespace Llyn.Infrastructure;

/// <summary>
/// Generates the opaque random identifiers that Entry, Example, Tag, and Situation carry. An id is a
/// fixed-length string of lowercase letters and digits (for example <c>176f86dr</c>) drawn from a
/// cryptographic source, never derived from any visible content — so an id reveals nothing about the
/// row it names and two rows with identical text still get distinct ids.
/// </summary>
public static class LIdentity
{
    private const int LIdentityLength = 8;
    private const string LIdentityAlphabet = "0123456789abcdefghijklmnopqrstuvwxyz";

    /// <summary>Returns a new opaque identifier. Distinct across calls with overwhelming probability.</summary>
    public static string LIdentityCreate()
    {
        return RandomNumberGenerator.GetString(LIdentityAlphabet, LIdentityLength);
    }
}
