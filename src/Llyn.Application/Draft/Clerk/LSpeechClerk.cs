using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Application;

public static class LSpeechClerk
{
    public static IReadOnlyList<LSpeechDraft> LSpeechParse(IReadOnlyList<LSpeechDraft> held, string? typed)
    {
        ArgumentNullException.ThrowIfNull(held);

        string name = (typed ?? string.Empty).Trim();
        if (name.Length == 0 || LSpeechHeldCheck(held, name))
        {
            return held;
        }

        return [.. held, LSpeechDraft.LSpeechDraftCreate(name)];
    }

    public static string? LSpeechPendingRead(IReadOnlyList<LSpeechDraft> held, string? typed)
    {
        return LSpeechParse(held, typed).Count > held.Count ? (typed ?? string.Empty).Trim() : null;
    }

    public static IReadOnlyList<LSpeechDraft> LSpeechChipRead(IReadOnlyList<LSpeechDraft> shown, string? pending)
    {
        ArgumentNullException.ThrowIfNull(shown);

        bool appended = pending is not null
            && shown.Count > 0
            && shown[^1].LSpeechDraftValue <= 0
            && string.Equals(shown[^1].LSpeechDraftName, pending, StringComparison.Ordinal);
        return LSpeechNormalize(appended ? shown.Take(shown.Count - 1).ToList() : shown);
    }

    public static bool LSpeechTypedCheck(string? typed)
    {
        return (typed ?? string.Empty).Trim().Length > 0;
    }

    public static IReadOnlyList<LSpeechDraft>? LSpeechAdd(
        IReadOnlyList<LSpeechDraft> held, string? name, Func<string, LSpeechValue?> declare)
    {
        ArgumentNullException.ThrowIfNull(held);
        ArgumentNullException.ThrowIfNull(declare);

        string typed = (name ?? string.Empty).Trim();
        if (typed.Length == 0)
        {
            return null;
        }

        if (LSpeechHeldCheck(held, typed))
        {
            return held;
        }

        LSpeechValue? value = declare(typed);
        return
        [
            .. held,
            value is null
                ? LSpeechDraft.LSpeechDraftCreate(typed)
                : LSpeechDraft.LSpeechDraftCreate(value.LSpeechValueId, value.LSpeechValueName),
        ];
    }

    public static IReadOnlyList<LSpeechDraft> LSpeechRemove(IReadOnlyList<LSpeechDraft> held, string? name)
    {
        ArgumentNullException.ThrowIfNull(held);

        List<LSpeechDraft> kept = new(held.Count);
        foreach (LSpeechDraft speech in held)
        {
            if (!string.Equals(speech.LSpeechDraftName, name, StringComparison.Ordinal))
            {
                kept.Add(speech);
            }
        }

        return kept;
    }

    public static (IReadOnlyList<LSpeechDraft> LSpeechHeld, string LSpeechTyped) LSpeechSettle(
        IReadOnlyList<LSpeechDraft> shown, IReadOnlyList<LSpeechDraft> held, string typed)
    {
        ArgumentNullException.ThrowIfNull(shown);
        ArgumentNullException.ThrowIfNull(held);
        ArgumentNullException.ThrowIfNull(typed);

        if (LSpeechMatch(shown, LSpeechParse(held, typed)))
        {
            return (held, typed);
        }

        return (LSpeechNormalize(shown), string.Empty);
    }

    private static bool LSpeechHeldCheck(IReadOnlyList<LSpeechDraft> held, string name)
    {
        foreach (LSpeechDraft speech in held)
        {
            if (string.Equals(speech.LSpeechDraftName, name, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static bool LSpeechMatch(IReadOnlyList<LSpeechDraft> shown, IReadOnlyList<LSpeechDraft> held)
    {
        if (shown.Count != held.Count)
        {
            return false;
        }

        for (int index = 0; index < shown.Count; index++)
        {
            if (shown[index].LSpeechDraftValue != held[index].LSpeechDraftValue
                || !string.Equals(
                    shown[index].LSpeechDraftName, held[index].LSpeechDraftName, StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    private static IReadOnlyList<LSpeechDraft> LSpeechNormalize(IReadOnlyList<LSpeechDraft> shown)
    {
        List<LSpeechDraft> kept = new(shown.Count);
        foreach (LSpeechDraft speech in shown)
        {
            string name = speech.LSpeechDraftName.Trim();
            if (name.Length == 0 || LSpeechHeldCheck(kept, name))
            {
                continue;
            }

            kept.Add(speech.LSpeechDraftValue > 0
                ? LSpeechDraft.LSpeechDraftCreate(speech.LSpeechDraftValue, name)
                : LSpeechDraft.LSpeechDraftCreate(name));
        }

        return kept;
    }
}
