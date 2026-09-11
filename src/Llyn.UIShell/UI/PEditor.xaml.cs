using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PEditor : UserControl
{
    private PWindow _pEditorHost = null!;

    private LEngine _lEngine = null!;

    private string _pEditorOrigin = string.Empty;

    public PEditor()
    {
        InitializeComponent();

        Resources.MergedDictionaries.Add(new PSentenceTemplate(this));
        Resources.MergedDictionaries.Add(new PContextTemplate(this));
        Resources.MergedDictionaries.Add(new PRegisterTemplate(this));
        Resources.MergedDictionaries.Add(new PImageTemplate(this));
        Resources.MergedDictionaries.Add(new PVideoTemplate(this));
        Resources.MergedDictionaries.Add(new PLabelTemplate(this));
        Resources.MergedDictionaries.Add(new PLinkTemplate(this));
        Resources.MergedDictionaries.Add(new PProspectTemplate(this));
        Resources.MergedDictionaries.Add(new PCandidateTemplate(this));
        Resources.MergedDictionaries.Add(new PSlateTemplate(this));
        Resources.MergedDictionaries.Add(new PMeaningTemplate(this));
        Resources.MergedDictionaries.Add(new PCollocationTemplate(this));
        Resources.MergedDictionaries.Add(new PLanguageTemplate(this));
        Resources.MergedDictionaries.Add(new PMarkerTemplate(this));
        Resources.MergedDictionaries.Add(new PCategoryTemplate(this));
        Resources.MergedDictionaries.Add(new PNotationTemplate(this));
        Resources.MergedDictionaries.Add(new PClipTemplate(this));

        PMeaningList.ItemsSource = _pMeaningList;
        PCollocationList.ItemsSource = _pCollocationList;
        PNotationList.ItemsSource = _pNotationItem;
        PClipList.ItemsSource = _pClipItem;
        PLanguageList.ItemsSource = _pLanguageItem;
        PProspectList.ItemsSource = _pProspectItem;
        PCandidateList.ItemsSource = _pCandidateItem;
        PCandidate.CustomPopupPlacementCallback = PCandidatePlace;
        PSlateList.ItemsSource = _pSlateItem;
        PSlate.CustomPopupPlacementCallback = PSlatePlace;
        PCategoryList.ItemsSource = _pCategoryItem;
        PMarkerList.ItemsSource = _pMarkerChip;
        PHeadword.TextChanged += PHeadwordHandle;
        PMarkerField.KeyDown += PMarkerFieldHandle;
        PMarkerField.TextChanged += PMarkerTextHandle;
        AddHandler(TextBoxBase.TextChangedEvent, new TextChangedEventHandler(PEditorTextHandle));
        PVolumeAttach();
    }

    internal void PEditorAttach(PWindow host, LEngine engine, string origin, long? entry)
    {
        _pEditorHost = host;
        _lEngine = engine;
        _pEditorOrigin = origin;

        Visibility owned = string.Equals(origin, "Input", StringComparison.Ordinal)
            ? Visibility.Visible
            : Visibility.Collapsed;
        PEditorCommand.Visibility = owned;

        PSentenceLoad();

        if (entry is null)
        {
            PEditorReset();
        }
        else
        {
            PEditorEntryShow(entry.Value);
        }

        PSpeakerLoad();
        PCategoryLoad();
        PVolumeLoad();
    }

    internal void PEditorClose()
    {
        PEditorChangeStop();
        PNotationCancel();
        PClipCancel();
        _pDownloaderPlayer.Close();
    }
}
