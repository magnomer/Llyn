using System;
using System.Collections.Generic;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QParadigmForm
{
    private QParadigmForm(string text, IReadOnlyList<QParadigmMark> marks, string? tip, int split)
    {
        QParadigmFormText = text;
        QParadigmFormMarks = marks;
        QParadigmFormTip = tip;
        QParadigmFormSplit = split;
    }

    public string QParadigmFormText { get; }

    public IReadOnlyList<QParadigmMark> QParadigmFormMarks { get; }

    public string? QParadigmFormTip { get; }

    public int QParadigmFormSplit { get; }

    internal static QParadigmForm QParadigmFormCreate(CParadigmForm form)
    {
        ArgumentNullException.ThrowIfNull(form);

        List<QParadigmMark> marks = new(form.CParadigmFormMarks.Count);
        foreach (CParadigmMark mark in form.CParadigmFormMarks)
        {
            marks.Add(QParadigmMark.QParadigmMarkCreate(mark));
        }

        return new QParadigmForm(form.CParadigmFormText, marks, form.CParadigmFormTip, form.CParadigmFormSplit);
    }
}
