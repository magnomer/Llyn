using System.Windows.Controls;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PTaxonomy : UserControl
{
    private PWindow _pTaxonomyHost = null!;

    private LEngine _lEngine = null!;

    private PObserver? _pTaxonomyObserver;

    public PTaxonomy()
    {
        InitializeComponent();
    }

    internal void PTaxonomyAttach(PWindow host, LEngine engine)
    {
        _pTaxonomyHost = host;
        _lEngine = engine;

        PDirectory.ItemsSource = _pDirectoryList;
        PMembership.ItemsSource = _pMembershipList;

        _pTaxonomyObserver = new PObserver(this, PTaxonomyBulletinHandle);
        engine.LEngineObserverAttach(_pTaxonomyObserver);

        PDisplay.PDisplayAttach(host, engine);

        PEditor.PEditorAttach(host, engine, "Taxonomy", null);
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
        if (_pTaxonomyObserver is not null)
        {
            _lEngine.LEngineObserverDetach(_pTaxonomyObserver);
            _pTaxonomyObserver = null;
        }

        PEditor.PEditorClose();
        PDisplay.PDisplayClose();
    }
}
