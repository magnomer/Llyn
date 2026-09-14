using System.Collections.Generic;
using System.Text;

namespace Llyn.Core;

public sealed record LContour(string LContourText, IReadOnlyList<int> LContourLevels)
{
    public const int LContourFloor = 1;

    public const int LContourCeiling = 5;

    private const string LContourSuperscript = "⁰¹²³⁴⁵⁶⁷⁸⁹";

    private const string LContourLetters = "˩˨˧˦˥";

    private const string LContourJoiners = "⁻-";

    private const string LContourBreaks = " .|‖‿\t";

    private const string LContourFences = "/[]()";

    public static IReadOnlyList<LContour> LContourParse(string ipa)
    {
        List<LContour> syllables = [];
        if (string.IsNullOrWhiteSpace(ipa))
        {
            return syllables;
        }

        StringBuilder segments = new();
        StringBuilder marks = new();
        foreach (char symbol in ipa)
        {
            bool tone = LContourMarkCheck(symbol) || (marks.Length > 0 && LContourJoiners.Contains(symbol));
            if (tone)
            {
                marks.Append(symbol);
                continue;
            }

            if (marks.Length > 0 || LContourBreaks.Contains(symbol))
            {
                LContourAppend(syllables, segments, marks);
            }

            if (!LContourBreaks.Contains(symbol) && !LContourFences.Contains(symbol))
            {
                segments.Append(symbol);
            }
        }

        LContourAppend(syllables, segments, marks);
        return syllables;
    }

    public static bool LContourToneCheck(IReadOnlyList<LContour> syllables)
    {
        foreach (LContour syllable in syllables)
        {
            if (syllable.LContourLevels.Count > 0)
            {
                return true;
            }
        }

        return false;
    }

    private static void LContourAppend(List<LContour> syllables, StringBuilder segments, StringBuilder marks)
    {
        if (segments.Length == 0 && marks.Length == 0)
        {
            return;
        }

        syllables.Add(new LContour(
            string.Concat(segments.ToString(), marks.ToString()),
            LContourLevelRead(marks.ToString())));
        segments.Clear();
        marks.Clear();
    }

    private static IReadOnlyList<int> LContourLevelRead(string marks)
    {
        string surface = marks.TrimEnd(LContourJoiners.ToCharArray());
        foreach (char joiner in LContourJoiners)
        {
            int cut = surface.LastIndexOf(joiner);
            if (cut >= 0)
            {
                surface = surface[(cut + 1)..];
            }
        }

        List<int> levels = [];
        foreach (char symbol in surface)
        {
            int level = LContourLevelResolve(symbol);
            if (level < LContourFloor || level > LContourCeiling)
            {
                return [];
            }

            levels.Add(level);
        }

        return levels;
    }

    private static bool LContourMarkCheck(char symbol)
    {
        return char.IsAsciiDigit(symbol) || LContourSuperscript.Contains(symbol) || LContourLetters.Contains(symbol);
    }

    private static int LContourLevelResolve(char symbol)
    {
        if (char.IsAsciiDigit(symbol))
        {
            return symbol - '0';
        }

        int superscript = LContourSuperscript.IndexOf(symbol);
        if (superscript >= 0)
        {
            return superscript;
        }

        return LContourLetters.IndexOf(symbol) + 1;
    }
}
