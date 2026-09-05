using System.Windows.Controls;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PPhonology : UserControl
{
    private PWindow _pPhonologyHost = null!;

    private LEngine _lEngine = null!;

    public PPhonology()
    {
        InitializeComponent();
    }

    internal void PPhonologyAttach(PWindow host, LEngine engine)
    {
        _pPhonologyHost = host;
        _lEngine = engine;

        PDisplay.PDisplayAttach(host, engine);

        PEditor.PEditorAttach(host, engine, "Phonology", null);

        PEditor.PEditorStoreDispatcher = PInventoryEntryUpdate;

        PEditor.PEditorDiscardDispatcher = PEditorEntryRestore;

        PArticulation.PArticulationAttach(PEditor.PPronunciation);
    }

    internal void PPhonologyReset()
    {
        PPhonologyClear();
        PInventoryFind(PProbe.Text ?? string.Empty);
    }

    internal bool PPhonologyDraftFinish(bool store)
    {
        return PEditor.PEditorDraftFinish(store);
    }

    internal bool PPhonologyChangeCheck()
    {
        return PEditor.Visibility == System.Windows.Visibility.Visible && PEditor.PEditorChangeCheck();
    }

    internal void PPhonologyClose()
    {
        PEditor.PEditorClose();
        PDisplay.PDisplayClose();
    }
}
