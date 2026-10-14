using System;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QEtymologyChoice
{
    internal QEtymologyChoice(CAncestorChoice choice)
    {
        ArgumentNullException.ThrowIfNull(choice);

        QEtymologyChoiceAncestor = choice;
    }

    public CAncestorChoice QEtymologyChoiceAncestor { get; }

    public override string ToString()
    {
        return QEtymologyChoiceAncestor.CAncestorChoiceKey.Length == 0
            ? QEtymologyChoiceAncestor.CAncestorChoiceName
            : QLocalizationCatalog.QLocalizationTextRead(QEtymologyChoiceAncestor.CAncestorChoiceKey);
    }
}
