using System;
using System.Text.RegularExpressions;
using Llyn.Core;

namespace Llyn.Infrastructure;

internal sealed class LFanqieShape
{
    private const string LFanqieShapeToken = "{word}";

    private static readonly TimeSpan LFanqieShapePatience = TimeSpan.FromSeconds(2);

    private LFanqieShape(
        Regex marker, Regex? split, Regex? head, Regex? column, Regex? rounded, Regex? line, Regex? spelling)
    {
        LFanqieShapeMarker = marker;
        LFanqieShapeSplit = split;
        LFanqieShapeHead = head;
        LFanqieShapeColumn = column;
        LFanqieShapeRounded = rounded;
        LFanqieShapeLine = line;
        LFanqieShapeSpelling = spelling;
    }

    internal Regex LFanqieShapeMarker { get; }

    internal Regex? LFanqieShapeSplit { get; }

    internal Regex? LFanqieShapeHead { get; }

    internal Regex? LFanqieShapeColumn { get; }

    internal Regex? LFanqieShapeRounded { get; }

    internal Regex? LFanqieShapeLine { get; }

    internal Regex? LFanqieShapeSpelling { get; }

    internal static LFanqieShape LFanqieShapeCreate(LFanqieBook book, string character)
    {
        ArgumentNullException.ThrowIfNull(book);
        ArgumentException.ThrowIfNullOrWhiteSpace(character);

        return new LFanqieShape(
            LFanqieRegexCreate(book.LFanqieBookPattern, character)!,
            LFanqieRegexCreate(book.LFanqieBookSplit, character),
            LFanqieRegexCreate(book.LFanqieBookHead, character),
            LFanqieRegexCreate(book.LFanqieBookColumn, character),
            LFanqieRegexCreate(book.LFanqieBookRounded, character),
            LFanqieRegexCreate(book.LFanqieBookLine, character),
            LFanqieRegexCreate(book.LFanqieBookSpelling, character));
    }

    private static Regex? LFanqieRegexCreate(string? pattern, string character)
    {
        if (string.IsNullOrEmpty(pattern))
        {
            return null;
        }

        return new Regex(
            pattern.Replace(LFanqieShapeToken, Regex.Escape(character), StringComparison.Ordinal),
            RegexOptions.CultureInvariant | RegexOptions.Singleline,
            LFanqieShapePatience);
    }
}
