using System.Windows.Controls;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PTaxonomy : UserControl
{
    private PWindow _pTaxonomyHost = null!;

    private LEngine _lEngine = null!;

    public PTaxonomy()
    {
        InitializeComponent();
    }

    internal void PTaxonomyAttach(PWindow host, LEngine engine)
    {
        _pTaxonomyHost = host;
        _lEngine = engine;

        PDisplay.PDisplayAttach(host, engine);

        PEditor.PEditorAttach(host, engine, "Taxonomy", null);

        PEditor.PEditorStoreDispatcher = PMembershipEntryUpdate;

        PEditor.PEditorDiscardDispatcher = PEditorEntryRestore;
    }

    internal void PTaxonomyReset()
    {
        PTaxonomyClear();
        PDirectoryReset();
        PDirectoryFind(PExploration.Text ?? string.Empty);
    }

    internal bool PTaxonomyDraftFinish(bool store)
    {
        return PEditor.PEditorDraftFinish(store);
    }

    internal bool PTaxonomyChangeCheck()
    {
        return PEditor.Visibility == System.Windows.Visibility.Visible && PEditor.PEditorChangeCheck();
    }

    internal void PTaxonomyClose()
    {
        PEditor.PEditorClose();
        PDisplay.PDisplayClose();
    }
}
