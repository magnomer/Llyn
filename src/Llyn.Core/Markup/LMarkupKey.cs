using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Llyn.Core;

public static partial class LMarkup
{
    private const int LMarkupKeyWord = 5;

    private const int LMarkupKeyLength = 40;

    private static readonly IReadOnlyDictionary<string, LMarkupRowKind> LMarkupCitationList =
        new Dictionary<string, LMarkupRowKind>(StringComparer.Ordinal)
        {
            ["author"] = LMarkupRowKind.LMarkupRowAuthor,
            ["use"] = LMarkupRowKind.LMarkupRowExample,
            ["situation"] = LMarkupRowKind.LMarkupRowSituation,
            ["register"] = LMarkupRowKind.LMarkupRowRegister,
            ["image"] = LMarkupRowKind.LMarkupRowImage,
            ["video"] = LMarkupRowKind.LMarkupRowVideo,
        };

    internal static string LMarkupKeyValidate(string? named, string kind)
    {
        if (named is null)
        {
            throw new FormatException($"A {kind} is declared without a key.");
        }

        if (named.Length == 0)
        {
            throw new FormatException($"A {kind} is declared with an empty key.");
        }

        foreach (char letter in named)
        {
            if (!LMarkupLetterCheck(letter))
            {
                throw new FormatException($"The {kind} key '{named}' is not a key.");
            }
        }

        return named;
    }

    internal static IReadOnlyDictionary<string, LMarkupRowKind> LMarkupKeyRead(
        IReadOnlyList<LMarkupToken> tokens, LMarkupCatalog catalog)
    {
        Dictionary<string, LMarkupRowKind> keys =
            new Dictionary<string, LMarkupRowKind>(catalog.LMarkupCatalogRow, StringComparer.Ordinal);

        foreach (LMarkupToken token in tokens)
        {
            if (token.LMarkupTokenKind != LMarkupTokenKind.LMarkupTokenEnter)
            {
                continue;
            }

            bool entry = string.Equals(token.LMarkupTokenName, "entry", StringComparison.Ordinal);
            if (!entry && !string.Equals(token.LMarkupTokenName, "sense", StringComparison.Ordinal))
            {
                continue;
            }

            LMarkupRowKind kind = entry
                ? LMarkupRowKind.LMarkupRowEntry
                : LMarkupRowKind.LMarkupRowSense;

            string? named = token.LMarkupTokenRead("id");
            if (named is null)
            {
                continue;
            }

            LMarkupRowAdd(keys, named, kind);
        }

        return keys;
    }

    internal static void LMarkupCitationValidate(
        IReadOnlyList<LMarkupToken> tokens, IReadOnlyDictionary<string, LMarkupRowKind> keys)
    {
        foreach (LMarkupToken token in tokens)
        {
            if (token.LMarkupTokenKind == LMarkupTokenKind.LMarkupTokenLeave)
            {
                continue;
            }

            if (LMarkupCitationList.TryGetValue(token.LMarkupTokenName, out LMarkupRowKind cited))
            {
                LMarkupCitationValidate(token.LMarkupTokenRead("ref"), cited, keys);
            }

            LMarkupCitationValidate(token.LMarkupTokenRead("src"), LMarkupRowKind.LMarkupRowSource, keys);
            LMarkupCitationValidate(token.LMarkupTokenRead("entry"), LMarkupRowKind.LMarkupRowEntry, keys);
            LMarkupCitationValidate(token.LMarkupTokenRead("sense"), LMarkupRowKind.LMarkupRowSense, keys);
        }
    }

    private static void LMarkupCitationValidate(
        string? cited, LMarkupRowKind wanted, IReadOnlyDictionary<string, LMarkupRowKind> keys)
    {
        if (string.IsNullOrEmpty(cited))
        {
            return;
        }

        if (!keys.TryGetValue(cited, out LMarkupRowKind found))
        {
            throw new FormatException($"The citation '{cited}' names no row the document declares.");
        }

        if (found != wanted)
        {
            throw new FormatException(
                $"The citation '{cited}' names a {LMarkupRowFormat(found)} "
                + $"where a {LMarkupRowFormat(wanted)} was wanted.");
        }
    }

    public static string LMarkupKeyCreate(string? seed, LMarkupRowKind kind, ISet<string> taken)
    {
        ArgumentNullException.ThrowIfNull(taken);

        string stem = LMarkupKeyFormat(seed);
        if (stem.Length == 0)
        {
            stem = LMarkupRowFormat(kind);
        }

        if (taken.Add(stem))
        {
            return stem;
        }

        for (int suffix = 2; ; suffix++)
        {
            string named = stem + "-" + suffix.ToString(CultureInfo.InvariantCulture);
            if (taken.Add(named))
            {
                return named;
            }
        }
    }

    private static string LMarkupKeyFormat(string? seed)
    {
        if (string.IsNullOrWhiteSpace(seed))
        {
            return string.Empty;
        }

        StringBuilder stem = new StringBuilder();
        int words = 0;
        bool broken = true;

        foreach (char letter in seed)
        {
            if (char.IsLetterOrDigit(letter))
            {
                if (broken)
                {
                    words++;
                    if (words > LMarkupKeyWord)
                    {
                        break;
                    }

                    if (stem.Length > 0)
                    {
                        stem.Append('-');
                    }

                    broken = false;
                }

                stem.Append(char.ToLowerInvariant(letter));
                if (stem.Length >= LMarkupKeyLength)
                {
                    break;
                }

                continue;
            }

            broken = true;
        }

        return stem.ToString();
    }
}
