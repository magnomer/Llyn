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

        Resources.MergedDictionaries.Add(new PSenseTemplate(this));
        Resources.MergedDictionaries.Add(new PCollocationTemplate(this));
        Resources.MergedDictionaries.Add(new PLangcodeTemplate(this));
        Resources.MergedDictionaries.Add(new PSpeechTemplate(this));
        Resources.MergedDictionaries.Add(new PLookupTemplate(this));
        Resources.MergedDictionaries.Add(new PDownloaderTemplate(this));

        PSenseList.ItemsSource = _pSenseList;
        PCollocationList.ItemsSource = _pCollocationList;
        PLookupMenuList.ItemsSource = _pLookupCandidate;
        PDownloaderMenuList.ItemsSource = _pDownloaderRecording;
        PLangcodeMenuList.ItemsSource = _pLangcodeItem;
        PSpeechMenuList.ItemsSource = _pSpeechItem;
        PHeadword.TextChanged += PHeadwordHandle;
    }

    internal Action<string>? PEditorStoreDispatcher { get; set; }

    internal Action? PEditorDiscardDispatcher { get; set; }

    internal void PEditorAttach(PWindow host, LEngine engine, string? entry)
    {
        _pEditorHost = host;
        _lEngine = engine;

        if (entry is null)
        {
            PEditorReset();
        }
        else
        {
            PEditorEntryShow(entry);
        }

        PLangcodeLoad();
        PSpeechLoad();
    }

    internal void PEditorClose()
    {
        PLookupCancel();
        PDownloaderCancel();
        _pDownloaderPlayer.Close();
    }
}
