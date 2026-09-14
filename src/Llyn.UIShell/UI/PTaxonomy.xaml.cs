using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        PEditor.PEditorChangeNotice = changed => PTaxonomyStore.IsEnabled = changed;
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

    private void PTaxonomyPressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _pDisplayEntry is not null && PDisplay.Visibility == Visibility.Visible;
    }

    private async void PTaxonomyPressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (_pDisplayEntry is long entry && PDisplay.Visibility == Visibility.Visible)
        {
            await _pTaxonomyHost.PWindowPressRun(
                ticket => _lEngine.LEnginePortraitPrint(entry, _pTaxonomyHost.PWindowLabelRead(), ticket));
        }
    }

    private async void PTaxonomyPortraitHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (_pDisplayEntry is long entry && PDisplay.Visibility == Visibility.Visible)
        {
            await _pTaxonomyHost.PWindowPortraitExport(entry);
        }
    }
}
