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

        PColophon.PColophonAttach(host);
        PImprint.PImprintAttach(host, engine, this);
        PDisplay.PDisplayAttach(host, engine);
        PEditor.PEditorAttach(host, engine, "Reference", null);
        PEditor.PEditorChangeNotice = changed => PReferenceStore.IsEnabled = changed;
        PEditor.PEditorChronicleNotice = PReferenceChronicleUpdate;
        PImprint.PImprintChronicleNotice = PReferenceChronicleUpdate;
        PImprint.PImprintChangeNotice = changed =>
        {
            if (PEditor.Visibility != Visibility.Visible)
            {
                PReferenceStore.IsEnabled = changed;
            }
        };
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
        PShelfFind();
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
        PImprint.PImprintClose();
        PEditor.PEditorClose();
        PDisplay.PDisplayClose();
        PGradeDropdown.IsOpen = false;
        PTrellisDropdown.IsOpen = false;
    }

    private void PReferencePressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = (_pFootnoteVista?.LVistaChosen is not null && PDisplay.Visibility == Visibility.Visible)
            || (_pReferenceVista?.LVistaChosen is not null && PColophon.Visibility == Visibility.Visible);
    }

    private async void PReferencePressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (_pFootnoteVista?.LVistaChosen is long entry && PDisplay.Visibility == Visibility.Visible)
        {
            await _pReferenceHost.PWindowPressRun(
                ticket => _lEngine.LEnginePortraitPrint(entry, _pReferenceHost.PWindowLabelRead(), ticket));
            return;
        }

        if (_pReferenceVista?.LVistaChosen is long shown && PColophon.Visibility == Visibility.Visible)
        {
            await _pReferenceHost.PWindowPressRun(ticket => _lEngine.LEnginePortraitPrint(
                shown, LOwner.LOwnerReference, _pReferenceHost.PWindowLegendRead("Source"), ticket));
        }
    }

    private void PReferencePortraitCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _pFootnoteVista?.LVistaChosen is not null && PDisplay.Visibility == Visibility.Visible;
    }

    private async void PReferencePortraitHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (_pFootnoteVista?.LVistaChosen is long entry && PDisplay.Visibility == Visibility.Visible)
        {
            await _pReferenceHost.PWindowPortraitExport(entry);
        }
    }
}
