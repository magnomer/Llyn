using System;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LQuillSituation
{
    private readonly LTenure _lQuillSituationTenure;

    public LQuillSituation(LTenure tenure)
    {
        ArgumentNullException.ThrowIfNull(tenure);

        _lQuillSituationTenure = tenure;
    }

    public void LQuillTitleSet(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        LSituation? held = LQuillSituationRead();
        _lQuillSituationTenure.LTenureRequestDefer(new LRequestSituationTitle(
            _lQuillSituationTenure.LTenureId,
            held?.LSituationId ?? 0,
            LQuillWrittenRead(text, held?.LSituationTitle)));
    }

    public void LQuillKindSet(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        LSituation? held = LQuillSituationRead();
        _lQuillSituationTenure.LTenureRequestDefer(new LRequestSituationKind(
            _lQuillSituationTenure.LTenureId,
            held?.LSituationId ?? 0,
            LQuillWrittenRead(text, held?.LSituationKind)));
    }

    public void LQuillDescriptionSet(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        LSituation? held = LQuillSituationRead();
        _lQuillSituationTenure.LTenureRequestDefer(new LRequestSituationDescription(
            _lQuillSituationTenure.LTenureId,
            held?.LSituationId ?? 0,
            LQuillWrittenRead(text, held?.LSituationDescription)));
    }

    private LSituation? LQuillSituationRead()
    {
        return _lQuillSituationTenure.LTenureRead()?.LDraftSituation;
    }

    private static LStateWritten LQuillWrittenRead(string text, LStateValue? held)
    {
        return new LStateWritten(text, text.Length == 0 && (held?.LStateValueUncertain ?? false));
    }
}
