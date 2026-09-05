using System;
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
        Resources.MergedDictionaries.Add(new PImageTemplate(this));
        Resources.MergedDictionaries.Add(new PVideoTemplate(this));
        Resources.MergedDictionaries.Add(new PLabelTemplate(this));
        Resources.MergedDictionaries.Add(new PLinkTemplate(this));
        Resources.MergedDictionaries.Add(new PProspectTemplate(this));
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
        PCategoryList.ItemsSource = _pCategoryItem;
        PMarkerList.ItemsSource = _pMarkerChip;
        PHeadword.TextChanged += PHeadwordHandle;
        PMarkerField.KeyDown += PMarkerFieldHandle;
        AddHandler(TextBoxBase.TextChangedEvent, new TextChangedEventHandler(PEditorTextHandle));
    }

    internal Action<string>? PEditorStoreDispatcher { get; set; }

    internal Action? PEditorDiscardDispatcher { get; set; }

    internal void PEditorAttach(PWindow host, LEngine engine, string origin, string? entry)
    {
        _pEditorHost = host;
        _lEngine = engine;
        _pEditorOrigin = origin;

        PSentenceLoad();

        if (entry is null)
        {
            PEditorReset();
        }
        else
        {
            PEditorEntryShow(entry);
        }

        PSpeakerLoad();
        PCategoryLoad();
    }

    internal void PEditorClose()
    {
        PEditorChangeStop();
        PNotationCancel();
        PClipCancel();
        PVideoClose();
        _pDownloaderPlayer.Close();
    }
}
