using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Conduct;

public sealed record CParadigmForm(
    string CParadigmFormText,
    IReadOnlyList<CParadigmMark> CParadigmFormMarks,
    string? CParadigmFormTip,
    int CParadigmFormSplit = 0)
{
    internal static CParadigmForm CParadigmFormCreate(LParadigmForm form, bool held)
    {
        ArgumentNullException.ThrowIfNull(form);

        CParadigmForm shown = CParadigmFormResolve(form.LParadigmFormStatus, form.LParadigmFormText, held);
        return form.LParadigmFormStatus == LParadigmStatus.LParadigmStatusText
            ? shown with
            {
                CParadigmFormMarks = form.LParadigmFormMarks.Select(CParadigmMark.CParadigmMarkCreate).ToList(),
                CParadigmFormSplit = form.LParadigmFormSplit,
            }
            : shown;
    }

    internal static CParadigmForm CParadigmFormResolve(LParadigmStatus status, string text, bool held)
    {
        ArgumentNullException.ThrowIfNull(text);

        return status switch
        {
            LParadigmStatus.LParadigmStatusText => new CParadigmForm(text, [], null),
            LParadigmStatus.LParadigmStatusUnknown => new CParadigmForm("—", [], "Paradigm.Unknown"),
            LParadigmStatus.LParadigmStatusPending => new CParadigmForm("…", [], "Paradigm.Pending"),
            LParadigmStatus.LParadigmStatusLost => new CParadigmForm("…", [], held ? "Paradigm.Held" : "Paradigm.Lost"),
            LParadigmStatus.LParadigmStatusAbsent => new CParadigmForm("…", [], "Paradigm.Absent"),
            _ => throw new ArgumentOutOfRangeException(nameof(status)),
        };
    }
}
