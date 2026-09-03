using System;
using System.Windows.Controls;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PEditor : UserControl
{
    private PWindow _pEditorHost = null!;

    private LEngine _lEngine = null!;

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
        Resources.MergedDictionaries.Add(new PSenseTemplate(this));
        Resources.MergedDictionaries.Add(new PCollocationTemplate(this));
        Resources.MergedDictionaries.Add(new PTongueTemplate(this));
        Resources.MergedDictionaries.Add(new PSpeechTemplate(this));
        Resources.MergedDictionaries.Add(new PCategoryTemplate(this));
        Resources.MergedDictionaries.Add(new PPhoneticTemplate(this));
        Resources.MergedDictionaries.Add(new PClipTemplate(this));

        PSenseList.ItemsSource = _pSenseList;
        PCollocationList.ItemsSource = _pCollocationList;
        PPhoneticList.ItemsSource = _pPhoneticItem;
        PClipList.ItemsSource = _pClipItem;
        PTongueList.ItemsSource = _pTongueItem;
        PProspectList.ItemsSource = _pProspectItem;
        PCategoryList.ItemsSource = _pCategoryItem;
        PSpeechList.ItemsSource = _pSpeechChip;
        PHeadword.TextChanged += PHeadwordHandle;
        PSpeechContents.KeyDown += PSpeechContentsHandle;
    }

    internal Action<string>? PEditorStoreDispatcher { get; set; }

    internal Action? PEditorDiscardDispatcher { get; set; }

    internal void PEditorAttach(PWindow host, LEngine engine, string? entry)
    {
        _pEditorHost = host;
        _lEngine = engine;

        PSentenceLoad();

        if (entry is null)
        {
            PEditorReset();
        }
        else
        {
            PEditorEntryShow(entry);
        }

        PLanguageLoad();
        PCategoryLoad();

        PEditorChangeStart();
    }

    internal void PEditorClose()
    {
        PEditorChangeStop();
        PPhoneticCancel();
        PClipCancel();
        PVideoClose();
        _pDownloaderPlayer.Close();
    }
}
