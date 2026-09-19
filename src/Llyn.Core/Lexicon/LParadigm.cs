using System;
using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LParadigm(
    long LParadigmSpeechCode,
    IReadOnlyList<long> LParadigmMorphology,
    IReadOnlyList<LParadigmRule>? LParadigmRegular = null,
    IReadOnlyList<long>? LParadigmExcept = null)
{
    public IReadOnlyList<LParadigmRule> LParadigmRegular { get; init; } = LParadigmRegular ?? [];

    public IReadOnlyList<long> LParadigmExcept { get; init; } = LParadigmExcept ?? [];

    public static string LParadigmLanguageRead(IReadOnlyList<LParadigmSlot> slots)
    {
        ArgumentNullException.ThrowIfNull(slots);

        return slots.Count == 0 ? string.Empty : slots[0].LParadigmSlotSpeech.LSpeechValueLanguage;
    }
}
