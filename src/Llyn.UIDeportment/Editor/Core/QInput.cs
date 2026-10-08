using System;
using System.Windows;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QInput
{
    private readonly QEditor _qInputEditor;

    internal QInput(FrameworkElement surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qInputEditor = new QEditor(QContract.QContractFind<FrameworkElement>(surface, "PEditor"));
    }

    internal void QInputIntroduce(CAtelier atelier, CEnvoy envoy, QVolume volume, QMentionMenu mentionMenu)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        _qInputEditor.QEditorIntroduce(atelier, envoy, volume, mentionMenu, atelier.CAtelierInputCreate(envoy));
    }

    internal void QInputExitRefine()
    {
        _qInputEditor.QEditorExitRefine();
    }
}
