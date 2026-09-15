using System;
using System.Collections.Generic;
using System.Text;

namespace Llyn.Core;

public static class LMarkdown
{
    public static string LMarkdownNormalize(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        string[] lines = text.Replace("\r\n", "\n", StringComparison.Ordinal).Replace('\r', '\n').Split('\n');
        StringBuilder canon = new StringBuilder(text.Length);

        foreach (string line in lines)
        {
            canon.Append(line.TrimEnd()).Append('\n');
        }

        return canon.ToString().Trim('\n');
    }

    public static IReadOnlyList<LMarkdownBlock> LMarkdownParse(string? text)
    {
        List<LMarkdownBlock> blocks = new List<LMarkdownBlock>();
        string canon = LMarkdownNormalize(text);

        if (canon.Length == 0)
        {
            return blocks;
        }

        string[] lines = canon.Split('\n');
        int place = 0;

        while (place < lines.Length)
        {
            string line = lines[place];
            string bare = line.TrimStart();

            if (bare.Length == 0)
            {
                place++;
            }
            else if (bare.StartsWith("```", StringComparison.Ordinal))
            {
                place = LMarkdownCodeParse(blocks, lines, place);
            }
            else if (LMarkdownRuleCheck(bare))
            {
                blocks.Add(new LMarkdownBlock(LMarkdownKind.LMarkdownKindRule, 0, [], string.Empty));
                place++;
            }
            else if (LMarkdownHeadingParse(blocks, bare))
            {
                place++;
            }
            else if (bare[0] == '>')
            {
                place = LMarkdownQuoteParse(blocks, lines, place);
            }
            else if (LMarkdownItemCheck(line, out _, out _, out _))
            {
                place = LMarkdownItemParse(blocks, lines, place);
            }
            else
            {
                place = LMarkdownParagraphParse(blocks, lines, place);
            }
        }

        return blocks;
    }

    private static int LMarkdownCodeParse(List<LMarkdownBlock> blocks, string[] lines, int place)
    {
        StringBuilder body = new StringBuilder();
        int next = place + 1;

        while (next < lines.Length && !lines[next].TrimStart().StartsWith("```", StringComparison.Ordinal))
        {
            if (body.Length > 0)
            {
                body.Append('\n');
            }

            body.Append(lines[next]);
            next++;
        }

        blocks.Add(new LMarkdownBlock(LMarkdownKind.LMarkdownKindCode, 0, [], body.ToString()));
        return Math.Min(next + 1, lines.Length);
    }

    private static bool LMarkdownRuleCheck(string bare)
    {
        string packed = bare.Replace(" ", string.Empty, StringComparison.Ordinal);

        if (packed.Length < 3)
        {
            return false;
        }

        char mark = packed[0];

        if (mark is not ('-' or '*' or '_'))
        {
            return false;
        }

        foreach (char letter in packed)
        {
            if (letter != mark)
            {
                return false;
            }
        }

        return true;
    }

    private static bool LMarkdownHeadingParse(List<LMarkdownBlock> blocks, string bare)
    {
        int level = 0;

        while (level < bare.Length && level < 6 && bare[level] == '#')
        {
            level++;
        }

        if (level == 0 || (level < bare.Length && bare[level] != ' '))
        {
            return false;
        }

        string body = bare[level..].Trim().TrimEnd('#').TrimEnd();
        blocks.Add(new LMarkdownBlock(
            LMarkdownKind.LMarkdownKindHeading, level, LMarkdownInline.LMarkdownInlineParse(body), string.Empty));
        return true;
    }

    private static int LMarkdownQuoteParse(List<LMarkdownBlock> blocks, string[] lines, int place)
    {
        StringBuilder body = new StringBuilder();
        int next = place;

        while (next < lines.Length && lines[next].TrimStart().StartsWith('>'))
        {
            string inner = lines[next].TrimStart()[1..];

            if (inner.StartsWith(' '))
            {
                inner = inner[1..];
            }

            if (body.Length > 0)
            {
                body.Append('\n');
            }

            body.Append(inner);
            next++;
        }

        blocks.Add(new LMarkdownBlock(
            LMarkdownKind.LMarkdownKindQuote, 0, LMarkdownInline.LMarkdownInlineParse(body.ToString()), string.Empty));
        return next;
    }

    private static bool LMarkdownItemCheck(string line, out LMarkdownKind kind, out int level, out string body)
    {
        int indent = line.Length - line.TrimStart().Length;
        string bare = line[indent..];
        kind = LMarkdownKind.LMarkdownKindBullet;
        level = indent / 2;
        body = string.Empty;

        if (bare.Length >= 2 && bare[0] is '-' or '*' or '+' && bare[1] == ' ')
        {
            body = bare[2..].TrimStart();
            return true;
        }

        int digits = 0;

        while (digits < bare.Length && char.IsAsciiDigit(bare[digits]))
        {
            digits++;
        }

        if (digits > 0 && digits + 1 < bare.Length && bare[digits] is '.' or ')' && bare[digits + 1] == ' ')
        {
            kind = LMarkdownKind.LMarkdownKindNumber;
            body = bare[(digits + 2)..].TrimStart();
            return true;
        }

        return false;
    }

    private static int LMarkdownItemParse(List<LMarkdownBlock> blocks, string[] lines, int place)
    {
        LMarkdownItemCheck(lines[place], out LMarkdownKind kind, out int level, out string body);
        StringBuilder text = new StringBuilder(body);
        int next = place + 1;

        while (next < lines.Length && LMarkdownContinueCheck(lines[next]))
        {
            text.Append('\n').Append(lines[next].Trim());
            next++;
        }

        blocks.Add(new LMarkdownBlock(
            kind, level, LMarkdownInline.LMarkdownInlineParse(text.ToString()), string.Empty));
        return next;
    }

    private static bool LMarkdownContinueCheck(string line)
    {
        string bare = line.TrimStart();

        return bare.Length > 0
            && bare.Length < line.Length
            && bare[0] != '>'
            && bare[0] != '#'
            && !bare.StartsWith("```", StringComparison.Ordinal)
            && !LMarkdownRuleCheck(bare)
            && !LMarkdownItemCheck(line, out _, out _, out _);
    }

    private static int LMarkdownParagraphParse(List<LMarkdownBlock> blocks, string[] lines, int place)
    {
        StringBuilder body = new StringBuilder(lines[place].Trim());
        int next = place + 1;

        while (next < lines.Length && LMarkdownPlainCheck(lines[next]))
        {
            body.Append('\n').Append(lines[next].Trim());
            next++;
        }

        blocks.Add(new LMarkdownBlock(
            LMarkdownKind.LMarkdownKindParagraph,
            0,
            LMarkdownInline.LMarkdownInlineParse(body.ToString()),
            string.Empty));
        return next;
    }

    private static bool LMarkdownPlainCheck(string line)
    {
        string bare = line.TrimStart();

        return bare.Length > 0
            && bare[0] != '>'
            && !bare.StartsWith("```", StringComparison.Ordinal)
            && !LMarkdownRuleCheck(bare)
            && !LMarkdownItemCheck(line, out _, out _, out _)
            && !(bare[0] == '#' && bare.TrimStart('#').StartsWith(' '));
    }
}
