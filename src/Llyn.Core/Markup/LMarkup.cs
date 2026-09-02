using System;
using System.Collections.Generic;

namespace Llyn.Core;

public static class LMarkup
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
        string? LMarkupTokenSource,
        string LMarkupTokenText,
        bool LMarkupTokenEmpty);

    internal readonly record struct LMarkupReference(
        string LMarkupReferenceId,
        LReference LMarkupReferenceValue,
        LStateValue LMarkupReferenceAuthor);

    private static readonly HashSet<string> LMarkupBlockList =
        new HashSet<string>(StringComparer.Ordinal) { "entry", "sense", "collocation", "source" };

    public static IReadOnlyList<LEntryDraft> LMarkupRead(string text)
    {
        return new List<LEntryDraft>();
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
                    LMarkupTokenKind.LMarkupTokenLeave, closed, null, string.Empty, false));
                continue;
            }

            string name = LMarkupNameRead(text, ref position, start);
            (string? source, bool closing) = LMarkupHeadRead(text, ref position, start, name);

            if (LMarkupBlockList.Contains(name))
            {
                tokens.Add(new LMarkupToken(
                    LMarkupTokenKind.LMarkupTokenEnter, name, source, string.Empty, closing));

                if (closing)
                {
                    tokens.Add(new LMarkupToken(
                        LMarkupTokenKind.LMarkupTokenLeave, name, null, string.Empty, false));
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
                    LMarkupTokenKind.LMarkupTokenText, name, source, string.Empty, true));
                continue;
            }

            string inner = LMarkupTextRead(text, ref position, start, name);
            tokens.Add(new LMarkupToken(
                LMarkupTokenKind.LMarkupTokenText, name, source, inner, inner.Length == 0));
        }

        if (blocks.Count > 0)
        {
            (string open, int opened) = blocks.Peek();
            throw new FormatException($"Unclosed tag '<{open}>' at character {opened}.");
        }

        return tokens;
    }

    internal static LCardDraft LMarkupCardRead(IReadOnlyList<LMarkupToken> tokens)
    {
        LMarkupToken? title = null;
        LMarkupToken? expression = null;
        LMarkupToken? meaning = null;
        LMarkupToken? synonym = null;
        List<LStateValue> tags = new List<LStateValue>();
        List<LStateValue> images = new List<LStateValue>();
        List<LExampleDraft> examples = new List<LExampleDraft>();
        List<LSituationDraft> situations = new List<LSituationDraft>();

        foreach (LMarkupToken token in tokens)
        {
            if (token.LMarkupTokenKind != LMarkupTokenKind.LMarkupTokenText)
            {
                continue;
            }

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
                    tags.Add(LMarkupStateRead(token));
                    break;
                case "image":
                    images.Add(LMarkupStateRead(token));
                    break;
                case "example":
                    examples.Add(new LExampleDraft(
                        LMarkupStateRead(token),
                        string.Empty,
                        LMarkupStateRead(token.LMarkupTokenSource)));
                    break;
                case "situation":
                    situations.Add(new LSituationDraft(
                        LMarkupStateRead(token),
                        string.Empty,
                        LMarkupStateRead(token.LMarkupTokenSource)));
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
            LMarkupStateRead(synonym).LStateValueShow(),
            tags,
            images);
    }

    internal static LMarkupReference LMarkupReferenceRead(IReadOnlyList<LMarkupToken> tokens)
    {
        string id = tokens.Count > 0 && tokens[0].LMarkupTokenKind == LMarkupTokenKind.LMarkupTokenEnter
            ? tokens[0].LMarkupTokenSource ?? string.Empty
            : string.Empty;

        LMarkupToken? title = null;
        LMarkupToken? author = null;
        LMarkupToken? year = null;
        LMarkupToken? url = null;
        LMarkupToken? program = null;
        LMarkupToken? channel = null;

        foreach (LMarkupToken token in tokens)
        {
            if (token.LMarkupTokenKind != LMarkupTokenKind.LMarkupTokenText)
            {
                continue;
            }

            switch (token.LMarkupTokenName)
            {
                case "title":
                    title = token;
                    break;
                case "author":
                    author = token;
                    break;
                case "year":
                    year = token;
                    break;
                case "url":
                    url = token;
                    break;
                case "program":
                    program = token;
                    break;
                case "channel":
                    channel = token;
                    break;
                default:
                    break;
            }
        }

        LStateValue credited = LMarkupStateRead(author);
        LReference reference = new LReference(
            id,
            LMarkupStateRead(title),
            LMarkupStateRead(program),
            LMarkupStateRead(channel),
            LMarkupStateRead(year),
            LMarkupStateRead(url),
            credited.LStateValueState);

        return new LMarkupReference(id, reference, credited);
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

    private static (string? Source, bool Closing) LMarkupHeadRead(
        string text, ref int position, int start, string name)
    {
        string? source = null;

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
                return (source, false);
            }

            if (text[position] == '/')
            {
                position++;
                if (position >= text.Length || text[position] != '>')
                {
                    throw new FormatException($"Unclosed tag '<{name}' at character {start}.");
                }

                position++;
                return (source, true);
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

            if (string.Equals(attribute, "src", StringComparison.Ordinal) ||
                string.Equals(attribute, "id", StringComparison.Ordinal))
            {
                source = value;
            }
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
