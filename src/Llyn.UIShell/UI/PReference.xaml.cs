using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PReference : UserControl
{
    private PWindow _pReferenceHost = null!;

    private LEngine _lEngine = null!;

    private PObserver? _pReferenceObserver;

    public PReference()
    {
        InitializeComponent();
    }

    internal void PReferenceAttach(PWindow host, LEngine engine)
    {
        _pReferenceHost = host;
        _lEngine = engine;

        PShelf.ItemsSource = _pShelfList;
        PFootnote.ItemsSource = _pFootnoteList;

        PImprint.PImprintAttach(host, engine, this);
        PDisplay.PDisplayAttach(host, engine);
        PEditor.PEditorAttach(host, engine, "Reference", null);
        PEditor.PEditorChangeNotice = changed => PReferenceStore.IsEnabled = changed;
        PImprint.PImprintChangeNotice = changed =>
        {
            if (PEditor.Visibility != Visibility.Visible)
            {
                PReferenceStore.IsEnabled = changed;
            }
        };

        _pReferenceObserver = new PObserver(this, PReferenceBulletinHandle);
        engine.LEngineObserverAttach(_pReferenceObserver);
    }

    private string PReferenceKindShow(LReferenceKind kind)
    {
        return _pReferenceHost.PLocalizationTextRead(PReferenceKindRead(kind));
    }

    internal static string PReferenceKindRead(LReferenceKind kind)
    {
        return kind switch
        {
            LReferenceKind.LReferenceKindUnknown => "Source.KindUnknown",
            LReferenceKind.LReferenceKindBook => "Source.KindBook",
            LReferenceKind.LReferenceKindJournal => "Source.KindJournal",
            LReferenceKind.LReferenceKindArticle => "Source.KindArticle",
            LReferenceKind.LReferenceKindWeb => "Source.KindWeb",
            LReferenceKind.LReferenceKindVideo => "Source.KindVideo",
            LReferenceKind.LReferenceKindAudio => "Source.KindAudio",
            LReferenceKind.LReferenceKindPicture => "Source.KindPicture",
            LReferenceKind.LReferenceKindOther => "Source.KindOther",
            _ => "Source.KindUnspecified",
        };
    }

    internal void PReferenceReset()
    {
        PReferenceClear();
        PShelfFind(PSurvey.Text ?? string.Empty);
    }

    internal bool PReferenceChangeCheck()
    {
        return PEditor.Visibility == Visibility.Visible
            ? PEditor.PEditorChangeCheck()
            : PImprint.PImprintChangeCheck();
    }

    internal bool PReferenceDraftFinish(bool store)
    {
        return PEditor.Visibility == Visibility.Visible
            ? PEditor.PEditorDraftFinish(store)
            : PImprint.PImprintDraftFinish(store);
    }

    internal void PReferenceClose()
    {
        if (_pReferenceObserver is not null)
        {
            _lEngine.LEngineObserverDetach(_pReferenceObserver);
            _pReferenceObserver = null;
        }

        PImprint.PImprintClose();
        PEditor.PEditorClose();
        PDisplay.PDisplayClose();
        PGradeDropdown.IsOpen = false;
        PTrellisDropdown.IsOpen = false;
    }

    private void PReferencePressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = (_pDisplayEntry is not null && PDisplay.Visibility == Visibility.Visible)
            || (_pColophonReference is not null && PColophon.Visibility == Visibility.Visible);
    }

    private async void PReferencePressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (_pDisplayEntry is long entry && PDisplay.Visibility == Visibility.Visible)
        {
            await _pReferenceHost.PWindowPressRun(
                ticket => _lEngine.LEnginePortraitPrint(entry, _pReferenceHost.PWindowLabelRead(), ticket));
            return;
        }

        if (_pColophonReference is long shown && PColophon.Visibility == Visibility.Visible)
        {
            await _pReferenceHost.PWindowPressRun(ticket => _lEngine.LEnginePortraitPrint(
                shown, LOwner.LOwnerReference, _pReferenceHost.PWindowLegendRead("Source"), ticket));
        }
    }
}
