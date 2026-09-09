using System;
using System.Collections.Generic;

namespace Llyn.Core;

public static partial class LMarkup
{
    public enum LMarkupRowKind
    {
        LMarkupRowAuthor,

        LMarkupRowSource,

        LMarkupRowExample,

        LMarkupRowSituation,

        LMarkupRowRegister,

        LMarkupRowImage,

        LMarkupRowVideo,

        LMarkupRowEntry,

        LMarkupRowSense,
    }

    public sealed record LMarkupCatalog(
        IReadOnlyDictionary<string, LMarkupRowKind> LMarkupCatalogRow,
        IReadOnlyDictionary<string, LAuthor> LMarkupCatalogAuthor,
        IReadOnlyDictionary<string, LMarkupReference> LMarkupCatalogSource,
        IReadOnlyDictionary<string, LExample> LMarkupCatalogExample,
        IReadOnlyDictionary<string, LSituation> LMarkupCatalogSituation,
        IReadOnlyDictionary<string, LRegister> LMarkupCatalogRegister,
        IReadOnlyDictionary<string, LImage> LMarkupCatalogImage,
        IReadOnlyDictionary<string, LVideo> LMarkupCatalogVideo)
    {
        public static LMarkupCatalog LMarkupCatalogCreate()
        {
            return new LMarkupCatalog(
                new Dictionary<string, LMarkupRowKind>(StringComparer.Ordinal),
                new Dictionary<string, LAuthor>(StringComparer.Ordinal),
                new Dictionary<string, LMarkupReference>(StringComparer.Ordinal),
                new Dictionary<string, LExample>(StringComparer.Ordinal),
                new Dictionary<string, LSituation>(StringComparer.Ordinal),
                new Dictionary<string, LRegister>(StringComparer.Ordinal),
                new Dictionary<string, LImage>(StringComparer.Ordinal),
                new Dictionary<string, LVideo>(StringComparer.Ordinal));
        }
    }

    public static string LMarkupRowFormat(LMarkupRowKind kind)
    {
        return kind switch
        {
            LMarkupRowKind.LMarkupRowAuthor => "author",
            LMarkupRowKind.LMarkupRowSource => "source",
            LMarkupRowKind.LMarkupRowExample => "example",
            LMarkupRowKind.LMarkupRowSituation => "situation",
            LMarkupRowKind.LMarkupRowRegister => "register",
            LMarkupRowKind.LMarkupRowImage => "image",
            LMarkupRowKind.LMarkupRowVideo => "video",
            LMarkupRowKind.LMarkupRowEntry => "entry",
            _ => "sense",
        };
    }

    public readonly record struct LMarkupReference(
        string LMarkupReferenceId,
        LReference LMarkupReferenceValue,
        IReadOnlyList<string> LMarkupReferenceAuthor);

    internal static LMarkupReference LMarkupReferenceRead(IReadOnlyList<LMarkupToken> tokens)
    {
        string? named = tokens.Count > 0 && tokens[0].LMarkupTokenKind == LMarkupTokenKind.LMarkupTokenEnter
            ? tokens[0].LMarkupTokenRead("id")
            : null;

        if (string.IsNullOrEmpty(named))
        {
            throw new FormatException("A source is declared without an id.");
        }

        string id = named;

        LMarkupToken? title = null;
        List<LMarkupToken> authors = new List<LMarkupToken>();
        LMarkupToken? year = null;
        LMarkupToken? url = null;
        LMarkupToken? kind = null;
        LMarkupToken? note = null;

        foreach (LMarkupToken token in LMarkupLeafRead(tokens))
        {
            switch (token.LMarkupTokenName)
            {
                case "title":
                    title = token;
                    break;
                case "author":
                    authors.Add(token);
                    break;
                case "year":
                    year = token;
                    break;
                case "url":
                    url = token;
                    break;
                case "kind":
                    kind = token;
                    break;
                case "note":
                    note = token;
                    break;
                default:
                    break;
            }
        }

        List<string> names = new List<string>();
        foreach (LMarkupToken author in authors)
        {
            string? cited = author.LMarkupTokenRead("ref");
            if (!string.IsNullOrEmpty(cited))
            {
                names.Add(cited);
            }
        }

        LState credited = names.Count > 0
            ? LState.LStateSpecified
            : authors.Count > 0
                ? LState.LStateUnknown
                : LState.LStateUnspecified;

        LReference reference = new LReference(
            id,
            LMarkupStateRead(title),
            LMarkupStateRead(year),
            LMarkupKindRead(kind),
            LMarkupStateRead(note),
            LMarkupStateRead(url),
            credited);

        return new LMarkupReference(id, reference, names);
    }

    private static LReferenceKind LMarkupKindRead(LMarkupToken? token)
    {
        if (token is null)
        {
            return LReferenceKind.LReferenceKindUnspecified;
        }

        return token.Value.LMarkupTokenEmpty
            ? LReferenceKind.LReferenceKindUnknown
            : LReference.LReferenceKindParse(token.Value.LMarkupTokenText);
    }

    internal static LMarkupCatalog LMarkupCatalogRead(
        IReadOnlyList<LMarkupToken> tokens, int first, int last)
    {
        Dictionary<string, LMarkupRowKind> rows =
            new Dictionary<string, LMarkupRowKind>(StringComparer.Ordinal);
        Dictionary<string, LAuthor> authors = new Dictionary<string, LAuthor>(StringComparer.Ordinal);
        Dictionary<string, LMarkupReference> sources =
            new Dictionary<string, LMarkupReference>(StringComparer.Ordinal);
        Dictionary<string, LExample> examples = new Dictionary<string, LExample>(StringComparer.Ordinal);
        Dictionary<string, LSituation> situations = new Dictionary<string, LSituation>(StringComparer.Ordinal);
        Dictionary<string, LRegister> registers = new Dictionary<string, LRegister>(StringComparer.Ordinal);
        Dictionary<string, LImage> images = new Dictionary<string, LImage>(StringComparer.Ordinal);
        Dictionary<string, LVideo> videos = new Dictionary<string, LVideo>(StringComparer.Ordinal);

        for (int position = first + 1; position < last; position++)
        {
            LMarkupToken token = tokens[position];

            if (token.LMarkupTokenKind == LMarkupTokenKind.LMarkupTokenEnter)
            {
                int close = LMarkupBlockFind(tokens, position);
                IReadOnlyList<LMarkupToken> block = LMarkupBlockRead(tokens, position, close);
                position = close;

                switch (token.LMarkupTokenName)
                {
                    case "source":
                        LMarkupReference source = LMarkupReferenceRead(block);
                        sources[LMarkupRowAdd(rows, source.LMarkupReferenceId, LMarkupRowKind.LMarkupRowSource)] =
                            source;
                        break;
                    case "example":
                        examples[LMarkupRowAdd(rows, token, LMarkupRowKind.LMarkupRowExample)] =
                            LMarkupExampleRead(token, block);
                        break;
                    case "situation":
                        situations[LMarkupRowAdd(rows, token, LMarkupRowKind.LMarkupRowSituation)] =
                            LMarkupSituationRead(block);
                        break;
                    case "register":
                        registers[LMarkupRowAdd(rows, token, LMarkupRowKind.LMarkupRowRegister)] =
                            LMarkupRegisterRead(token, block);
                        break;
                    case "video":
                        videos[LMarkupRowAdd(rows, token, LMarkupRowKind.LMarkupRowVideo)] =
                            LMarkupVideoRead(block);
                        break;
                    default:
                        break;
                }

                continue;
            }

            if (token.LMarkupTokenKind != LMarkupTokenKind.LMarkupTokenText)
            {
                continue;
            }

            switch (token.LMarkupTokenName)
            {
                case "author":
                    authors[LMarkupRowAdd(rows, token, LMarkupRowKind.LMarkupRowAuthor)] = new LAuthor(
                        string.Empty,
                        token.LMarkupTokenEmpty ? string.Empty : token.LMarkupTokenText);
                    break;
                case "image":
                    images[LMarkupRowAdd(rows, token, LMarkupRowKind.LMarkupRowImage)] =
                        new LImage(string.Empty, LMarkupStateRead(token));
                    break;
                default:
                    break;
            }
        }

        return new LMarkupCatalog(
            rows, authors, sources, examples, situations, registers, images, videos);
    }

    private static LExample LMarkupExampleRead(LMarkupToken token, IReadOnlyList<LMarkupToken> block)
    {
        LMarkupToken? text = null;
        LMarkupToken? translation = null;

        foreach (LMarkupToken leaf in LMarkupLeafRead(block))
        {
            switch (leaf.LMarkupTokenName)
            {
                case "text":
                    text = leaf;
                    break;
                case "trans":
                    translation = leaf;
                    break;
                default:
                    break;
            }
        }

        return new LExample(
            string.Empty,
            token.LMarkupTokenRead("lang") ?? string.Empty,
            LMarkupStateRead(text),
            LMarkupStateRead(translation),
            LMarkupStateRead(token.LMarkupTokenRead("src")));
    }

    private static LSituation LMarkupSituationRead(IReadOnlyList<LMarkupToken> block)
    {
        LMarkupToken? title = null;
        LMarkupToken? description = null;
        LMarkupToken? kind = null;

        foreach (LMarkupToken leaf in LMarkupLeafRead(block))
        {
            switch (leaf.LMarkupTokenName)
            {
                case "title":
                    title = leaf;
                    break;
                case "description":
                    description = leaf;
                    break;
                case "kind":
                    kind = leaf;
                    break;
                default:
                    break;
            }
        }

        return new LSituation(
            string.Empty,
            LMarkupStateRead(title),
            LMarkupStateRead(description),
            LMarkupStateRead(kind));
    }

    private static LRegister LMarkupRegisterRead(LMarkupToken token, IReadOnlyList<LMarkupToken> block)
    {
        LMarkupToken? name = null;

        foreach (LMarkupToken leaf in LMarkupLeafRead(block))
        {
            if (string.Equals(leaf.LMarkupTokenName, "name", StringComparison.Ordinal))
            {
                name = leaf;
            }
        }

        return new LRegister(
            string.Empty,
            LMarkupStateRead(name),
            token.LMarkupTokenRead("lang") ?? string.Empty,
            string.Equals(token.LMarkupTokenRead("builtin"), "yes", StringComparison.Ordinal));
    }

    private static LVideo LMarkupVideoRead(IReadOnlyList<LMarkupToken> block)
    {
        LMarkupToken? location = null;
        LMarkupToken? span = null;

        foreach (LMarkupToken leaf in LMarkupLeafRead(block))
        {
            switch (leaf.LMarkupTokenName)
            {
                case "location":
                    location = leaf;
                    break;
                case "span":
                    span = leaf;
                    break;
                default:
                    break;
            }
        }

        return new LVideo(string.Empty, LMarkupStateRead(location), LMarkupStateRead(span));
    }

    private static string LMarkupRowAdd(
        IDictionary<string, LMarkupRowKind> rows, LMarkupToken token, LMarkupRowKind kind)
    {
        return LMarkupRowAdd(rows, token.LMarkupTokenRead("id"), kind);
    }

    private static string LMarkupRowAdd(
        IDictionary<string, LMarkupRowKind> rows, string? named, LMarkupRowKind kind)
    {
        string key = LMarkupKeyValidate(named, LMarkupRowFormat(kind));
        if (rows.ContainsKey(key))
        {
            throw new FormatException($"The key '{key}' is declared twice.");
        }

        rows[key] = kind;
        return key;
    }
}
