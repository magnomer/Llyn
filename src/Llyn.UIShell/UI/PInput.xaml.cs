using System.Windows.Controls;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PInput : UserControl
{
    public PInput()
    {
        InitializeComponent();
    }

    internal void PInputAttach(PWindow host, LEngine engine)
    {
        PEditor.PEditorAttach(host, engine, null);

        PEditor.PEditorDiscardDispatcher = PEditor.PEditorReset;
    }

    internal void PInputReset()
    {
        PEditor.PEditorReset();
    }

    internal bool PInputChangeCheck()
    {
        return PEditor.PEditorChangeCheck();
    }

    internal void PInputClose()
    {
        PEditor.PEditorClose();
    }
}
