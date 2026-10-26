using System;
using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CStep(
    CCardDraft? CStepCard,
    Func<bool, bool>? CStepFold,
    IReadOnlyDictionary<CCaretTrail, long?>? CStepCaret);
