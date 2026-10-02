using System;
using System.Windows;

namespace Llyn.UIDeportment;

internal sealed class QInput
{
    private readonly QEditor _qInputEditor;

    internal QInput(FrameworkElement surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qInputEditor = new QEditor(QContract.QContractFind<FrameworkElement>(surface, "PEditor"));
    }

    internal void QInputIntroduce(QWindow host)
    {
        _qInputEditor.QEditorIntroduce(host, host.QWindowAtelier.CAtelierInputCreate(host.QWindowEnvoy));
    }

    internal void QInputExitRefine()
    {
        _qInputEditor.QEditorExitRefine();
    }
}
