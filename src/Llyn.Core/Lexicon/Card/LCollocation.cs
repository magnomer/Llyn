using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public sealed record LCollocation(
    long LCollocationId,
    long LCollocationEntryId,
    int LCollocationPosition,
    LStateValue LCollocationTitle,
    LStateValue LCollocationExpression,
    LStateValue LCollocationMeaning)
{
    public LStateValue LCollocationTitle { get; init; } = LCollocationTitle ?? LStateValue.LStateValueUnspecified;

    public LStateValue LCollocationExpression { get; init; } =
        LCollocationExpression ?? LStateValue.LStateValueUnspecified;

    public LStateValue LCollocationMeaning { get; init; } = LCollocationMeaning ?? LStateValue.LStateValueUnspecified;

    public static long? LCollocationHomeRead(IReadOnlyList<LCardDraft> meanings, LCardDraft collocation, bool nested)
    {
        ArgumentNullException.ThrowIfNull(meanings);
        ArgumentNullException.ThrowIfNull(collocation);

        foreach (long sense in collocation.LCardDraftSense)
        {
            if (LMeaning.LMeaningPathRead(meanings, sense) is not null)
            {
                return nested ? sense : LMeaning.LMeaningRootRead(meanings, sense);
            }
        }

        return null;
    }

    public static IReadOnlyList<long> LCollocationStubRead(
        IReadOnlyList<LCardDraft> meanings, LCardDraft collocation, bool nested)
    {
        long? home = LCollocationHomeRead(meanings, collocation, nested);
        List<long> stubs = [];
        foreach (long sense in collocation.LCardDraftSense)
        {
            if (LMeaning.LMeaningPathRead(meanings, sense) is null)
            {
                continue;
            }

            long lifted = nested ? sense : LMeaning.LMeaningRootRead(meanings, sense);
            if (lifted != home && !stubs.Contains(lifted))
            {
                stubs.Add(lifted);
            }
        }

        return stubs;
    }

    public static IReadOnlyList<LCardDraft> LCollocationOtherRead(
        IReadOnlyList<LCardDraft> meanings, IReadOnlyList<LCardDraft> collocations)
    {
        ArgumentNullException.ThrowIfNull(collocations);

        List<LCardDraft> others = [];
        foreach (LCardDraft collocation in collocations)
        {
            if (LCollocationHomeRead(meanings, collocation, true) is null)
            {
                others.Add(collocation);
            }
        }

        return others;
    }

    public static LStateValue LCollocationPhraseRead(LCardDraft collocation)
    {
        ArgumentNullException.ThrowIfNull(collocation);

        LStateValue expression = collocation.LCardDraftExpression;
        return expression.LStateValueUncertain || expression.LStateValueShow().Length > 0
            ? expression
            : collocation.LCardDraftTitle;
    }

    public static IReadOnlyList<string> LCollocationLanguageRead(IReadOnlyList<LTranslationTarget> targets)
    {
        ArgumentNullException.ThrowIfNull(targets);

        return targets
            .Select(static target => target.LTranslationTargetLanguage)
            .Where(static language => language.Length > 0)
            .Distinct(StringComparer.Ordinal)
            .ToList();
    }

    public static int LCollocationExampleRead(LCardDraft collocation)
    {
        ArgumentNullException.ThrowIfNull(collocation);

        return collocation.LCardDraftSentence.Count(static sentence => !sentence.LSentenceDraftEmpty);
    }

    public static IReadOnlyList<string> LCollocationAlsoRead(IReadOnlyList<LCardDraft> meanings, LCardDraft collocation)
    {
        List<string> paths = [];
        foreach (long sense in LCollocationStubRead(meanings, collocation, true))
        {
            if (LMeaning.LMeaningPathRead(meanings, sense) is string path)
            {
                paths.Add(path);
            }
        }

        return paths;
    }
}
