using System;
using System.Collections.Generic;

namespace Llyn.Core;

public static partial class LMarkup
{
    internal enum LMarkupTokenKind
    {
        LMarkupTokenText,

        LMarkupTokenEnter,

        LMarkupTokenLeave,
    }

    internal readonly record struct LMarkupToken(
        LMarkupTokenKind LMarkupTokenKind,
        string LMarkupTokenName,
        IReadOnlyDictionary<string, string> LMarkupTokenMark,
        string LMarkupTokenText,
        bool LMarkupTokenEmpty)
    {
        internal string? LMarkupTokenRead(string mark)
        {
            return LMarkupTokenMark.TryGetValue(mark, out string? value) ? value : null;
        }
    }

    internal static readonly IReadOnlyDictionary<string, string> LMarkupMarkEmpty =
        new Dictionary<string, string>(StringComparer.Ordinal);

    private static readonly HashSet<string> LMarkupBlockList =
        new HashSet<string>(StringComparer.Ordinal)
        {
            "llyn",
            "catalog",
            "entry",
            "sense",
            "collocation",
            "source",
            "example",
            "situation",
            "register",
            "video",
            "inflection",
            "pronunciation",
            "relation",
        };

    private static readonly HashSet<string> LMarkupMarkList =
        new HashSet<string>(StringComparer.Ordinal)
        {
            "id",
            "ref",
            "src",
            "par",
            "dep",
            "lang",
            "role",
            "local",
            "pos",
            "entry",
            "sense",
            "type",
            "label",
            "labels",
            "level",
            "system",
            "builtin",
            "source",
            "onset",
            "medial",
            "nucleus",
            "coda",
            "orthography",
            "tone",
            "tone-local",
            "tone-points",
        };

    public static IReadOnlyList<LEntryDraft> LMarkupRead(string text)
    {
        List<LEntryDraft> drafts = new List<LEntryDraft>();

        foreach (LMarkupEntry entry in LMarkupEntryRead(text).LMarkupDocumentEntry)
        {
            drafts.Add(entry.LMarkupEntryDraft);
        }

        return drafts;
    }

    internal static IReadOnlyList<LMarkupToken> LMarkupScan(string text)
    {
        List<LMarkupToken> tokens = new List<LMarkupToken>();
        Stack<(string Name, int Start)> blocks = new Stack<(string, int)>();
        int position = 0;

        while (position < text.Length)
        {
            if (text[position] != '<')
            {
                position++;
                continue;
            }

            int start = position;
            position++;

            if (position < text.Length && text[position] == '/')
            {
                position++;
                string closed = LMarkupNameRead(text, ref position, start);
                LMarkupTailRead(text, ref position, start, closed);

                if (blocks.Count == 0 || !string.Equals(blocks.Peek().Name, closed, StringComparison.Ordinal))
                {
                    throw new FormatException(
                        $"Unmatched closing tag '</{closed}>' at character {start}.");
                }

                blocks.Pop();
                tokens.Add(new LMarkupToken(
                    LMarkupTokenKind.LMarkupTokenLeave, closed, LMarkupMarkEmpty, string.Empty, false));
                continue;
            }

            string name = LMarkupNameRead(text, ref position, start);
            (IReadOnlyDictionary<string, string> mark, bool closing) =
                LMarkupHeadRead(text, ref position, start, name);

            if (LMarkupBlockList.Contains(name))
            {
                tokens.Add(new LMarkupToken(
                    LMarkupTokenKind.LMarkupTokenEnter, name, mark, string.Empty, closing));

                if (closing)
                {
                    tokens.Add(new LMarkupToken(
                        LMarkupTokenKind.LMarkupTokenLeave, name, LMarkupMarkEmpty, string.Empty, false));
                }
                else
                {
                    blocks.Push((name, start));
                }

                continue;
            }

            if (closing)
            {
                tokens.Add(new LMarkupToken(
                    LMarkupTokenKind.LMarkupTokenText, name, mark, string.Empty, true));
                continue;
            }

            string inner = LMarkupTextRead(text, ref position, start, name);
            tokens.Add(new LMarkupToken(
                LMarkupTokenKind.LMarkupTokenText, name, mark, inner, inner.Length == 0));
        }

        if (blocks.Count > 0)
        {
            (string open, int opened) = blocks.Peek();
            throw new FormatException($"Unclosed tag '<{open}>' at character {opened}.");
        }

        return tokens;
    }

    internal static LCardDraft LMarkupCardRead(IReadOnlyList<LMarkupToken> tokens, int position)
    {
        LMarkupToken? title = null;
        LMarkupToken? expression = null;
        LMarkupToken? meaning = null;
        LMarkupToken? synonym = null;
        List<string> tags = new List<string>();
        List<LStateValue> images = new List<LStateValue>();
        List<LVideoDraft> videos = new List<LVideoDraft>();
        List<LExampleDraft> examples = new List<LExampleDraft>();
        List<LSituationDraft> situations = new List<LSituationDraft>();
        List<LRegisterDraft> registers = new List<LRegisterDraft>();

        foreach (LMarkupToken token in LMarkupLeafRead(tokens))
        {
            switch (token.LMarkupTokenName)
            {
                case "title":
                    title = token;
                    break;
                case "expression":
                    expression = token;
                    break;
                case "meaning":
                    meaning = token;
                    break;
                case "synonym":
                    synonym = token;
                    break;
                case "tag":
                    LMarkupTagRead(tags, token);
                    break;
                case "image":
                    images.Add(LMarkupStateRead(token));
                    break;
                case "video":
                    videos.Add(LVideoDraft.LVideoDraftCreate(LMarkupStateRead(token)));
                    break;
                case "example":
                    examples.Add(new LExampleDraft(
                        LMarkupStateRead(token),
                        string.Empty,
                        LMarkupStateRead(token.LMarkupTokenRead("src")),
                        LMarkupStateRead(token.LMarkupTokenRead("par")),
                        LMarkupStateRead(token.LMarkupTokenRead("dep"))));
                    break;
                case "situation":
                    situations.Add(new LSituationDraft(LMarkupStateRead(token), string.Empty));
                    break;
                case "register":
                    registers.Add(new LRegisterDraft(LMarkupStateRead(token), string.Empty));
                    break;
                default:
                    break;
            }
        }

        return new LCardDraft(
            LMarkupStateRead(title),
            LMarkupStateRead(expression),
            LMarkupStateRead(meaning),
            examples,
            situations,
            registers,
            [],
            LMarkupStateRead(synonym).LStateValueShow(),
            tags,
            images,
            videos,
            position);
    }

    private static IEnumerable<LMarkupToken> LMarkupLeafRead(IReadOnlyList<LMarkupToken> tokens)
    {
        int depth = 0;

        foreach (LMarkupToken token in tokens)
        {
            switch (token.LMarkupTokenKind)
            {
                case LMarkupTokenKind.LMarkupTokenEnter:
                    depth++;
                    break;
                case LMarkupTokenKind.LMarkupTokenLeave:
                    depth--;
                    break;
                default:
                    if (depth == 1)
                    {
                        yield return token;
                    }

                    break;
            }
        }
    }

    private static void LMarkupTagRead(List<string> tags, LMarkupToken token)
    {
        if (token.LMarkupTokenEmpty)
        {
            return;
        }

        tags.Add(token.LMarkupTokenText);
    }

    private static LStateValue LMarkupStateRead(LMarkupToken? token)
    {
        if (token is null)
        {
            return LStateValue.LStateValueUnspecified;
        }

        return token.Value.LMarkupTokenEmpty
            ? LStateValue.LStateValueUnknown
            : LStateValue.LStateValueCreate(token.Value.LMarkupTokenText);
    }

    private static LStateValue LMarkupStateRead(string? source)
    {
        if (source is null)
        {
            return LStateValue.LStateValueUnspecified;
        }

        return source.Length == 0
            ? LStateValue.LStateValueUnknown
            : LStateValue.LStateValueCreate(source);
    }

    private static string LMarkupNameRead(string text, ref int position, int start)
    {
        int first = position;
        while (position < text.Length && LMarkupLetterCheck(text[position]))
        {
            position++;
        }

        if (position == first)
        {
            throw new FormatException($"Stray '<' at character {start}.");
        }

        return text[first..position];
    }

    private static (IReadOnlyDictionary<string, string> Mark, bool Closing)
        LMarkupHeadRead(string text, ref int position, int start, string name)
    {
        Dictionary<string, string>? mark = null;

        while (true)
        {
            while (position < text.Length && char.IsWhiteSpace(text[position]))
            {
                position++;
            }

            if (position >= text.Length)
            {
                throw new FormatException($"Unclosed tag '<{name}' at character {start}.");
            }

            if (text[position] == '>')
            {
                position++;
                return (mark ?? LMarkupMarkEmpty, false);
            }

            if (text[position] == '/')
            {
                position++;
                if (position >= text.Length || text[position] != '>')
                {
                    throw new FormatException($"Unclosed tag '<{name}' at character {start}.");
                }

                position++;
                return (mark ?? LMarkupMarkEmpty, true);
            }

            int first = position;
            while (position < text.Length && LMarkupLetterCheck(text[position]))
            {
                position++;
            }

            if (position == first)
            {
                throw new FormatException(
                    $"Malformed attribute in tag '<{name}' at character {position}.");
            }

            string attribute = text[first..position];
            string value = LMarkupValueRead(text, ref position, name);

            if (!LMarkupMarkList.Contains(attribute))
            {
                continue;
            }

            mark ??= new Dictionary<string, string>(StringComparer.Ordinal);
            mark[attribute] = value;
        }
    }

    private static string LMarkupValueRead(string text, ref int position, string name)
    {
        while (position < text.Length && char.IsWhiteSpace(text[position]))
        {
            position++;
        }

        if (position >= text.Length || text[position] != '=')
        {
            throw new FormatException(
                $"Attribute without a value in tag '<{name}' at character {position}.");
        }

        position++;
        while (position < text.Length && char.IsWhiteSpace(text[position]))
        {
            position++;
        }

        if (position >= text.Length || (text[position] != '"' && text[position] != '\''))
        {
            throw new FormatException(
                $"Unquoted attribute value in tag '<{name}' at character {position}.");
        }

        char quote = text[position];
        position++;
        int first = position;
        while (position < text.Length && text[position] != quote)
        {
            position++;
        }

        if (position >= text.Length)
        {
            throw new FormatException(
                $"Unterminated attribute value in tag '<{name}' at character {first}.");
        }

        string value = text[first..position];
        position++;
        return value;
    }

    private static string LMarkupTextRead(string text, ref int position, int start, string name)
    {
        string tail = $"</{name}>";
        int close = text.IndexOf(tail, position, StringComparison.Ordinal);
        if (close < 0)
        {
            throw new FormatException($"Unclosed tag '<{name}>' at character {start}.");
        }

        string inner = text[position..close];
        position = close + tail.Length;
        return inner.Trim();
    }

    private static void LMarkupTailRead(string text, ref int position, int start, string name)
    {
        while (position < text.Length && char.IsWhiteSpace(text[position]))
        {
            position++;
        }

        if (position >= text.Length || text[position] != '>')
        {
            throw new FormatException($"Unclosed tag '</{name}' at character {start}.");
        }

        position++;
    }

    private static bool LMarkupLetterCheck(char value)
    {
        return char.IsLetterOrDigit(value) || value == '-' || value == '_';
    }
}
