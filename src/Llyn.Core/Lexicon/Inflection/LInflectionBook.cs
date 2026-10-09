using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Llyn.Core;

public sealed record LInflectionBook(
    IReadOnlyList<LInflectionKind> LInflectionBookKinds,
    IReadOnlyList<LInflectionStem> LInflectionBookStems,
    IReadOnlyList<LInflectionRule> LInflectionBookRules,
    IReadOnlyList<LInflectionRule> LInflectionBookFolds,
    LInflectionLayout? LInflectionBookLayout,
    string LInflectionBookStamp)
{
    public string? LInflectionBookResolve(string headword, IReadOnlyList<long> codes) =>
        LInflectionBookDivide(headword, codes, out _);

    public string? LInflectionBookDivide(string headword, IReadOnlyList<long> codes, out int? root)
    {
        ArgumentNullException.ThrowIfNull(headword);
        ArgumentNullException.ThrowIfNull(codes);

        root = null;
        IReadOnlyList<LInflectionKind> kinds = LInflectionBookRead(headword);
        if (kinds.Count == 0)
        {
            return null;
        }

        List<long> cell = codes.Order().ToList();
        foreach (LInflectionStem stem in LInflectionBookStems)
        {
            foreach (LInflectionEnding ending in stem.LInflectionStemEndings)
            {
                if (!stem.LInflectionStemValues.Concat(ending.LInflectionEndingValues).Order().SequenceEqual(cell))
                {
                    continue;
                }

                LInflectionKind? kind = kinds.FirstOrDefault(
                    candidate => stem.LInflectionStemTemplates.ContainsKey(candidate.LInflectionKindName));
                if (kind is null)
                {
                    return null;
                }

                string text = kind.LInflectionKindResolve(
                    headword, stem.LInflectionStemTemplates[kind.LInflectionKindName]) + ending.LInflectionEndingText;
                int boundary = text.IndexOf('·', StringComparison.Ordinal);
                bool marked = boundary >= 0;
                foreach (LInflectionRule rule in LInflectionBookRules)
                {
                    if (rule.LInflectionRuleKind is not null
                        && !kinds.Any(
                            member => string.Equals(
                                member.LInflectionKindName, rule.LInflectionRuleKind, StringComparison.Ordinal)))
                    {
                        continue;
                    }

                    string before = text;
                    text = rule.LInflectionRuleResolve(text);
                    int index = text.IndexOf('·', StringComparison.Ordinal);
                    if (!marked || index >= 0)
                    {
                        boundary = index;
                        continue;
                    }

                    int output = 0;
                    int last = 0;
                    int? moved = null;
                    foreach (Match match in rule.LInflectionRuleScan(before))
                    {
                        if (match.Index > boundary)
                        {
                            break;
                        }

                        output += match.Index - last;
                        if (boundary < match.Index + match.Length)
                        {
                            moved = output;
                            break;
                        }

                        output += match.Result(rule.LInflectionRuleReplacement).Length;
                        last = match.Index + match.Length;
                    }

                    boundary = moved ?? output + boundary - last;
                }

                root = marked ? Math.Clamp(boundary, 0, text.Length) : null;
                return text;
            }
        }

        return null;
    }

    private IReadOnlyList<LInflectionKind> LInflectionBookRead(string headword) =>
        LInflectionBookKinds.Where(kind => kind.LInflectionKindCheck(headword)).ToList();
}
