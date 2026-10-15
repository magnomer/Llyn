using System;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QStudy
{
    private readonly UserControl _qStudySurface;

    internal QStudy(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qStudySurface = surface;
    }

    private TextBlock QStudyEmpty => QContract.QContractFind<TextBlock>(_qStudySurface, "PStudyEmpty");

    internal void QStudyIntroduce()
    {
        CStudy study = new();
        QStudyEmpty.SetResourceReference(TextBlock.TextProperty, study.CStudyEmptyKey);
    }
}
