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
    internal static CParadigmForm CParadigmFormCreate(LParadigmForm form)
    {
        ArgumentNullException.ThrowIfNull(form);

        return new CParadigmForm(
            form.LParadigmFormText,
            form.LParadigmFormMarks.Select(CParadigmMark.CParadigmMarkCreate).ToArray(),
            form.LParadigmFormTip,
            form.LParadigmFormSplit);
    }
}
