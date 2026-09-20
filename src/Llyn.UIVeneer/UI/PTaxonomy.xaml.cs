using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.ShellEngine;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PTaxonomy : UserControl
{
    private PWindow _pTaxonomyHost = null!;

    private LTaxonomy _lTaxonomy = null!;

    private LEditor _lEditor = null!;

    public PTaxonomy()
    {
        InitializeComponent();
    }

    internal void PTaxonomyAttach(PWindow host, LEngine engine)
    {
        _pTaxonomyHost = host;
        _lTaxonomy = new LTaxonomy(engine, engine, engine);

        PDirectory.ItemsSource = _pDirectoryList;
        PMembership.ItemsSource = _pMembershipList;

        _lEditor = new LEditor(engine, engine, engine, engine, host.PWindowUnreadableConfirm);
        PDisplay.PDisplayAttach(host, _lEditor.LEditorDisplay);
        _lEditor.LEditorStateChanged += PTaxonomyStoreUpdate;
        PEditor.PEditorAttach(host, _lEditor);
    }

    private void PTaxonomyStoreUpdate()
    {
        PTaxonomyStore.IsEnabled = _lEditor.LEditorStorable;
    }

    internal void PTaxonomyReset()
    {
        PTaxonomyClear();
        PDirectoryReset();
        PDirectoryFind();
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

    private void PTaxonomyPressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _lTaxonomy.LTaxonomyMembershipChosen is not null && PDisplay.Visibility == Visibility.Visible;
    }

    private async void PTaxonomyPressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (_lTaxonomy.LTaxonomyMembershipChosen is not null)
        {
            if (PDisplay.Visibility == Visibility.Visible)
            {
                await _pTaxonomyHost.PWindowPressRun(
                    ticket => _lTaxonomy.LTaxonomyPortraitPrint(_pTaxonomyHost.PWindowLabelRead(), ticket));
            }
        }
    }

    private async void PTaxonomyPortraitHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (_lTaxonomy.LTaxonomyMembershipChosen is not null)
        {
            if (PDisplay.Visibility == Visibility.Visible)
            {
                await _pTaxonomyHost.PWindowPortraitExport(
                    _lTaxonomy.LTaxonomyFileRead(), _lTaxonomy.LTaxonomyPortraitExport);
            }
        }
    }
}
