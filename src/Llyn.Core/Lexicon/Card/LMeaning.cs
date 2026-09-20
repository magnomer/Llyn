using System;
using System.Collections.Generic;

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

}

