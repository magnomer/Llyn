using System;
using System.Windows.Controls;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

/// <summary>
/// The shared editable entry view as a control: what it is made of, and when it starts and stops. Its
/// behaviour is split across the files beside this one — the card lists, the drag, the menus, the form
/// as a value; this file only assembles the parts, takes the engine the window opened, and shuts the
/// editor down.
///
/// One control serves every panel that edits an entry. The input panel mounts it standing on no entry
/// and never puts it on one, which is a form that only creates them; a browse-style panel mounts it
/// over an entry it loaded, which is a form that modifies that one. What differs between them is not the editing structure but what a
/// store and a discard mean afterwards, and each host says that through the two seams below.
/// </summary>
public partial class PEditor : UserControl
{
    // The window this editor sits in. An editor does not put up its own dialogs: a failure is the
    // program speaking, so it is asked for through the window.
    private PWindow _pEditorHost = null!;

    private LEngine _lEngine = null!;

    public PEditor()
    {
        InitializeComponent();

        // Each card and menu row is a template in a dictionary of its own, so this control's markup
        // stays its own layout. A template still raises this control's events, which is why the
        // dictionaries are built here against this instance rather than merged from markup.
        Resources.MergedDictionaries.Add(new PSenseTemplate(this));
        Resources.MergedDictionaries.Add(new PCollocationTemplate(this));
        Resources.MergedDictionaries.Add(new PLangcodeTemplate(this));
        Resources.MergedDictionaries.Add(new PLookupTemplate(this));
        Resources.MergedDictionaries.Add(new PDownloaderTemplate(this));

        PSenseList.ItemsSource = _pSenseList;
        PCollocationList.ItemsSource = _pCollocationList;
        PLookupMenuList.ItemsSource = _pLookupCandidate;
        PDownloaderMenuList.ItemsSource = _pDownloaderRecording;
        PLangcodeMenuList.ItemsSource = _pLangcodeItem;
        PHeadword.TextChanged += PHeadwordHandle;
    }

    /// <summary>
    /// What the host does once an entry has been stored, given the id it was stored under. A
    /// browse-style panel re-reads its catalog, since a headword it lists may have just changed; the
    /// input panel has nothing to do, since its form is already empty for the next entry.
    /// </summary>
    internal Action<string>? PEditorStoreDispatcher { get; set; }

    /// <summary>
    /// What the host does when the discard button is pressed. Discarding is the one decision the two
    /// kinds of host answer differently — an input form throws the typing away and comes up empty, a
    /// browse-style panel puts the selected entry back as it is stored — so neither is assumed here.
    /// </summary>
    internal Action? PEditorDiscardDispatcher { get; set; }

    /// <summary>
    /// Puts the editor to work on <paramref name="engine"/>, the workspace the window opened, and
    /// opens it on <paramref name="entry"/> — the entry the host wants edited, or nothing at all for a
    /// form that creates one. The entry is shown before the language menu is built, so the language it
    /// carries is already the chosen one when that menu decides whether a fallback is needed.
    /// </summary>
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
    }

    /// <summary>
    /// Stops the editor: searches in flight are called off and playback is released.
    /// </summary>
    internal void PEditorClose()
    {
        PLookupCancel();
        PDownloaderCancel();
        _pDownloaderPlayer.Close();
    }
}
