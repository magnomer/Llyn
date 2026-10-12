using System;
using System.Collections.Generic;
using System.Globalization;

namespace Llyn.Core;

public sealed record LMeaning(
    long LMeaningId,
    long LMeaningEntryId,
    long? LMeaningParentId,
    int LMeaningPosition,
    LStateValue LMeaningTitle,
    LStateValue LMeaningDefinition)
{
    public LStateValue LMeaningTitle { get; init; } = LMeaningTitle ?? LStateValue.LStateValueUnspecified;

    public LStateValue LMeaningDefinition { get; init; } = LMeaningDefinition ?? LStateValue.LStateValueUnspecified;

    public string LMeaningName => LMeaningTitle.LStateValueShown ?? LMeaningDefinition.LStateValueShown ?? string.Empty;

    public static string? LMeaningPathRead(IReadOnlyList<LCardDraft> meanings, long meaningId)
    {
        ArgumentNullException.ThrowIfNull(meanings);

        if (meaningId == 0)
        {
            return null;
        }

        Stack<(IReadOnlyList<LCardDraft>, string)> pending = new();
        pending.Push((meanings, string.Empty));
        while (pending.Count > 0)
        {
            (IReadOnlyList<LCardDraft> level, string prefix) = pending.Pop();
            for (int index = 0; index < level.Count; index++)
            {
                string path = prefix + (index + 1).ToString(CultureInfo.InvariantCulture);
                if (level[index].LCardDraftId == meaningId)
                {
                    return path;
                }

                pending.Push((level[index].LCardDraftChild, path + "."));
            }
        }

        return null;
    }

    public static long LMeaningPathFind(IReadOnlyList<LCardDraft> meanings, string path)
    {
        ArgumentNullException.ThrowIfNull(meanings);
        ArgumentNullException.ThrowIfNull(path);

        IReadOnlyList<LCardDraft> level = meanings;
        LCardDraft? found = null;
        foreach (string step in path.Split('.'))
        {
            if (!int.TryParse(step, NumberStyles.None, CultureInfo.InvariantCulture, out int rank)
                || rank < 1
                || rank > level.Count)
            {
                return 0;
            }

            found = level[rank - 1];
            level = found.LCardDraftChild;
        }

        return found?.LCardDraftId ?? 0;
    }

    public static long LMeaningRootRead(IReadOnlyList<LCardDraft> meanings, long meaningId)
    {
        string? path = LMeaningPathRead(meanings, meaningId);
        return path is null ? 0 : LMeaningPathFind(meanings, path.Split('.')[0]);
    }

    public static IReadOnlyList<LMeaning> LMeaningDraftRead(long entryId, IReadOnlyList<LCardDraft> meanings)
    {
        ArgumentNullException.ThrowIfNull(meanings);

        List<LMeaning> rows = [];
        Stack<(IReadOnlyList<LCardDraft>, long?)> pending = new();
        pending.Push((meanings, null));
        while (pending.Count > 0)
        {
            (IReadOnlyList<LCardDraft> level, long? parent) = pending.Pop();
            for (int index = 0; index < level.Count; index++)
            {
                LCardDraft card = level[index];
                rows.Add(new LMeaning(
                    card.LCardDraftId, entryId, parent, index, card.LCardDraftTitle, card.LCardDraftMeaning));
                pending.Push((card.LCardDraftChild, card.LCardDraftId));
            }
        }

        return rows;
    }
}
