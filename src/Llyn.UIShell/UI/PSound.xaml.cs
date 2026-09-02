using System.Windows.Controls;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PSound : UserControl
{
    private PWindow _pSoundHost = null!;

    private LEngine _lEngine = null!;

    public PSound()
    {
        InitializeComponent();
    }

    internal void PSoundAttach(PWindow host, LEngine engine)
    {
        _pSoundHost = host;
        _lEngine = engine;

        PDisplay.PDisplayAttach(engine);

        PEditor.PEditorAttach(host, engine, null);

        PEditor.PEditorStoreDispatcher = PInventoryEntryUpdate;

        PEditor.PEditorDiscardDispatcher = PEditorEntryRestore;

        PArticulation.PArticulationAttach(PEditor.PPronunciation);
    }

    internal void PSoundReset()
    {
        PSoundClear();
        PInventoryFind(PProbe.Text ?? string.Empty);
    }

    internal bool PSoundChangeCheck()
    {
        return PEditor.Visibility == System.Windows.Visibility.Visible && PEditor.PEditorChangeCheck();
    }

    internal void PSoundClose()
    {
        PEditor.PEditorClose();
        PDisplay.PDisplayClose();
    }
}
