using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Llyn.Core;

public static class LInflectionDifference
{
    public static IReadOnlyList<LInflectionMark> LInflectionDifferenceScan(
        IReadOnlyList<LInflectionRule> folds,
        string predicted,
        string actual)
    {
        ArgumentNullException.ThrowIfNull(folds);
        ArgumentNullException.ThrowIfNull(predicted);
        ArgumentNullException.ThrowIfNull(actual);

        IReadOnlyList<LInflectionLetter> left = LInflectionDifferenceResolve(folds, predicted);
        IReadOnlyList<LInflectionLetter> right = LInflectionDifferenceResolve(folds, actual);
        int[,] common = LInflectionDifferenceCreate(left, right);
        List<LInflectionMark> marks = [];
        int p = 0;
        int a = 0;
        while (a < right.Count)
        {
            if (p < left.Count && left[p].LInflectionLetterValue == right[a].LInflectionLetterValue)
            {
                p++;
                a++;
            }
            else if (p < left.Count && common[p + 1, a] >= common[p, a + 1])
            {
                p++;
            }
            else
            {
                int offset = right[a].LInflectionLetterOffset;
                int end = right[a].LInflectionLetterEnd;
                if (end > offset)
                {
                    if (marks.Count > 0 && offset <= marks[^1].LInflectionMarkOffset + marks[^1].LInflectionMarkLength)
                    {
                        LInflectionMark last = marks[^1];
                        int stop = Math.Max(end, last.LInflectionMarkOffset + last.LInflectionMarkLength);
                        marks[^1] = last with { LInflectionMarkLength = stop - last.LInflectionMarkOffset };
                    }
                    else
                    {
                        marks.Add(new LInflectionMark(offset, end - offset));
                    }
                }

                a++;
            }
        }

        return marks;
    }

    public static int LInflectionDifferenceDivide(
        IReadOnlyList<LInflectionRule> folds,
        string predicted,
        int root,
        string actual)
    {
        ArgumentNullException.ThrowIfNull(folds);
        ArgumentNullException.ThrowIfNull(predicted);
        ArgumentNullException.ThrowIfNull(actual);

        IReadOnlyList<LInflectionLetter> left = LInflectionDifferenceResolve(folds, predicted);
        IReadOnlyList<LInflectionLetter> right = LInflectionDifferenceResolve(folds, actual);
        int[,] common = LInflectionDifferenceCreate(left, right);
        int p = 0;
        int a = 0;
        while (a < right.Count)
        {
            if (p < left.Count && left[p].LInflectionLetterValue == right[a].LInflectionLetterValue)
            {
                if (left[p].LInflectionLetterOffset >= root)
                {
                    return right[a].LInflectionLetterOffset;
                }

                p++;
                a++;
            }
            else if (p < left.Count && common[p + 1, a] >= common[p, a + 1])
            {
                p++;
            }
            else
            {
                a++;
            }
        }

        return actual.Length;
    }

    private static int[,] LInflectionDifferenceCreate(
        IReadOnlyList<LInflectionLetter> left,
        IReadOnlyList<LInflectionLetter> right)
    {
        int[,] common = new int[left.Count + 1, right.Count + 1];
        for (int i = left.Count - 1; i >= 0; i--)
        {
            for (int j = right.Count - 1; j >= 0; j--)
            {
                common[i, j] = left[i].LInflectionLetterValue == right[j].LInflectionLetterValue
                    ? common[i + 1, j + 1] + 1
                    : Math.Max(common[i + 1, j], common[i, j + 1]);
            }
        }

        return common;
    }

    private static IReadOnlyList<LInflectionLetter> LInflectionDifferenceResolve(
        IReadOnlyList<LInflectionRule> folds,
        string text)
    {
        List<LInflectionLetter> letters = [];
        for (int index = 0; index < text.Length; index++)
        {
            letters.Add(new LInflectionLetter(char.ToLowerInvariant(text[index]), index, index + 1));
        }

        foreach (LInflectionRule fold in folds)
        {
            string current = string.Concat(letters.ConvertAll(static letter => letter.LInflectionLetterValue));
            List<LInflectionLetter> next = [];
            int cursor = 0;
            foreach (Match match in fold.LInflectionRuleScan(current))
            {
                next.AddRange(letters.GetRange(cursor, match.Index - cursor));
                int offset = match.Index < letters.Count ? letters[match.Index].LInflectionLetterOffset : text.Length;
                int end = match.Length > 0 ? letters[match.Index + match.Length - 1].LInflectionLetterEnd : offset;
                foreach (char letter in match.Result(fold.LInflectionRuleReplacement))
                {
                    next.Add(new LInflectionLetter(letter, offset, end));
                }

                cursor = match.Index + match.Length;
            }

            next.AddRange(letters.GetRange(cursor, letters.Count - cursor));
            letters = next;
        }

        return letters;
    }
}
