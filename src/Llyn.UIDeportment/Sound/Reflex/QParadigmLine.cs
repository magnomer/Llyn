using System;
using System.Collections.Generic;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QParadigmLine
{
    private QParadigmLine(string group, string label, IReadOnlyList<QParadigmForm> forms)
    {
        QParadigmLineGroup = group;
        QParadigmLineLabel = label;
        QParadigmLineForms = forms;
    }

    public string QParadigmLineGroup { get; }

    public string QParadigmLineLabel { get; }

    public IReadOnlyList<QParadigmForm> QParadigmLineForms { get; }

    internal static QParadigmLine QParadigmLineCreate(CParadigmLine line)
    {
        ArgumentNullException.ThrowIfNull(line);

        List<QParadigmForm> forms = new(line.CParadigmLineForms.Count);
        foreach (CParadigmForm form in line.CParadigmLineForms)
        {
            forms.Add(QParadigmForm.QParadigmFormCreate(form));
        }

        return new QParadigmLine(line.CParadigmLineGroup, line.CParadigmLineLabel, forms);
    }
}
