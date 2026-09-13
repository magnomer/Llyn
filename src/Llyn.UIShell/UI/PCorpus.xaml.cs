using System.Windows;
using System.Windows.Controls;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PCorpus : UserControl
{
    private PWindow _pCorpusHost = null!;

    private LEngine _lEngine = null!;

    private PObserver? _pCorpusObserver;

    public PCorpus()
    {
        InitializeComponent();
    }

    internal void PCorpusAttach(PWindow host, LEngine engine)
    {
        _pCorpusHost = host;
        _lEngine = engine;

        PAnthology.ItemsSource = _pAnthologyList;
        PQuotation.ItemsSource = _pQuotationList;
        PCitationList.ItemsSource = _pCitationCatalog;
        PLanguageList.ItemsSource = _pLanguageItem;
        PTranscriptMentionLine.ItemsSource = _pTranscriptChip.PMentionLineChip;
        PTranscriptGlossLine.ItemsSource = _pTranscriptGloss;
        PExcerptGloss.ItemsSource = _pExcerptGloss;

        PDisplay.PDisplayAttach(host, engine);
        PEditor.PEditorAttach(host, engine, PTranscriptOrigin, null);
        PEditor.PEditorChangeNotice = changed => PCorpusStore.IsEnabled = changed;

        _pCorpusObserver = new PObserver(this, PCorpusBulletinHandle);
        engine.LEngineObserverAttach(_pCorpusObserver);
    }

    internal void PCorpusReset()
    {
        PCorpusClear();
        PAnthologyFind(PQuery.Text ?? string.Empty);
    }

    internal bool PCorpusChangeCheck()
    {
        return PEditor.Visibility == Visibility.Visible
            ? PEditor.PEditorChangeCheck()
            : PTranscriptChangeCheck();
    }

    internal bool PCorpusDraftFinish(bool store)
    {
        return PEditor.Visibility == Visibility.Visible
            ? PEditor.PEditorDraftFinish(store)
            : PTranscriptDraftFinish(store);
    }

    internal void PCorpusClose()
    {
        if (_pCorpusObserver is not null)
        {
            _lEngine.LEngineObserverDetach(_pCorpusObserver);
            _pCorpusObserver = null;
        }

        PEditor.PEditorClose();
        PDisplay.PDisplayClose();
        PCitationDrawer.IsOpen = false;
        PLanguage.IsOpen = false;
        PRankDropdown.IsOpen = false;
        PGauzeDropdown.IsOpen = false;
    }
}
